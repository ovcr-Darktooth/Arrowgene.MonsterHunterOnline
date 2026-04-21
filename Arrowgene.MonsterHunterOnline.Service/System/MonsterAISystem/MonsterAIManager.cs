using System;
using System.Collections.Generic;
using System.Threading;
using Arrowgene.Logging;
using Arrowgene.MonsterHunterOnline.Protocol.Old.Structures;
using Arrowgene.MonsterHunterOnline.Protocol.Structures;
using Arrowgene.MonsterHunterOnline.Service.CsProto;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Core;
using Arrowgene.MonsterHunterOnline.Service.Data;

namespace Arrowgene.MonsterHunterOnline.Service.System.MonsterAISystem
{
    public class MonsterAIManager
    {
        private static readonly ILogger Logger = LogProvider.Logger(typeof(MonsterAIManager));

        // NetIds for server-spawned monsters start at 1000 to avoid colliding with player IDs
        private static uint _nextNetId = 1000;

        private ClientManager _clientManager;
        private SequenceManager _sequenceManager;
        private MonsterDefinitionTable _monsterDefinitions;
        private PartsTable _partsTable;
        private readonly Dictionary<uint, MonsterAI> _monsters = new();
        private readonly object _lock = new();

        public MonsterAIManager(ClientManager clientManager, SequenceManager sequenceManager, MonsterDefinitionTable monsterDefinitions, PartsTable partsTable)
        {
            _clientManager = clientManager;
            _sequenceManager = sequenceManager;
            _monsterDefinitions = monsterDefinitions;
            _partsTable = partsTable;
        }

        public uint NextNetId() => Interlocked.Increment(ref _nextNetId);

        /// <summary>Registers a monster and starts its AI loop.</summary>
        public MonsterAI Spawn(uint netId, uint renderNetId, int monsterInfoId, CSVec3 position)
        {
            int maxHp = 0;
            string defName = null;
            if (_monsterDefinitions != null && _monsterDefinitions.TryGet(monsterInfoId, out var def))
            {
                maxHp = def.MaxHealth;
                defName = def.EntityName;
            }

            var monster = new MonsterAI(netId, renderNetId, monsterInfoId, position, this, _sequenceManager, maxHp, _partsTable);
            lock (_lock)
            {
                _monsters[netId] = monster;
            }
            Logger.Info($"MonsterAI spawned: netId={netId} renderNetId={renderNetId} infoId={monsterInfoId} name={defName ?? "?"} hp={monster.CurrentHp}/{monster.MaxHp} pos=({position.x:F1},{position.y:F1},{position.z:F1})");
            return monster;
        }

        public bool TryGet(uint netId, out MonsterAI monster)
        {
            lock (_lock)
            {
                return _monsters.TryGetValue(netId, out monster);
            }
        }

        /// <summary>
        /// Returns the nearest live monster (CurrentHp &gt; 0) to the given position, or null if none exist.
        /// Used as a fallback when the client reports a CryEngine runtime entity ID that we can't
        /// cross-reference to our server-assigned NetId.
        /// </summary>
        public MonsterAI FindNearestLiveMonster(CSVec3 from)
        {
            MonsterAI nearest = null;
            float minDist = float.MaxValue;
            lock (_lock)
            {
                foreach (var m in _monsters.Values)
                {
                    if (m.CurrentHp <= 0) continue;
                    float d = Distance(from, m.Position);
                    if (d < minDist)
                    {
                        minDist = d;
                        nearest = m;
                    }
                }
            }
            return nearest;
        }

        /// <summary>Stops the AI loop and removes the monster.</summary>
        public void Despawn(uint netId)
        {
            MonsterAI monster;
            lock (_lock)
            {
                if (!_monsters.TryGetValue(netId, out monster))
                    return;
                _monsters.Remove(netId);
            }
            monster.Dispose();
            Logger.Info($"MonsterAI despawned: netId={netId}");
        }

        /// <summary>Returns the nearest connected client with a known position, and the distance to it.</summary>
        public (Client client, float dist) FindNearestPlayer(CSVec3 from)
        {
            Client nearest = null;
            float minDist = float.MaxValue;

            foreach (Client c in _clientManager.GetAll())
            {
                if (c.State?.Position == null) continue;
                float dist = Distance(from, c.State.Position);
                
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = c;
                }
                Logger.Info($"Player check = {c.State.Position.x} from identity = {c.Identity}");
            }

            return (nearest, minDist);
        }

        /// <summary>
        /// Broadcasts CS_CMD_MONSTER_ACTIVE (528) to all clients.
        /// activeState=1 : monster is alive and active.
        /// activeState=0 : monster is deactivated (dead/despawned).
        /// </summary>
        public void BroadcastMonsterActiveState(uint netId, uint activeState, CSVec3 pos, long syncTime)
        {
            foreach (Client c in _clientManager.GetAll())
            {
                try
                {
                    CsCsProtoStructurePacket<MonsterActiveState> packet = CsProtoResponse.MonsterActiveState;
                    packet.Structure.SyncTime   = syncTime;
                    packet.Structure.ActiveState = activeState;
                    packet.Structure.MonsterId  = netId;
                    packet.Structure.Position   = new XYZPosition { x = pos.x, y = pos.y, z = pos.z };
                    packet.Structure.Rotation   = new Quaternion  { x = 0, y = 0, z = 0, w = 1 };
                    c.SendCsProtoStructurePacket(packet);
                }
                catch (Exception ex)
                {
                    Logger.Error($"BroadcastMonsterActiveState to {c.Identity}: {ex.Message}");
                }
            }
        }

        /// <summary>Sends a MonsterLocomotion packet to every connected client.</summary>
        public void BroadcastLcm(CSMonsterLocomotion lcm)
        {
            var packet = NewCsPacket.MonsterLCM(lcm);
            foreach (Client c in _clientManager.GetAll())
            {
                try { c.SendCsPacket(packet); }
                catch (Exception ex) { Logger.Error($"BroadcastLcm to {c.Identity}: {ex.Message}"); }
            }
        }

        public void BroadcastMovestate(CSMonsterMovestate movestate)
        {
            var packet = NewCsPacket.MonsterMovestate(movestate);
            foreach (Client c in _clientManager.GetAll())
            {
                try { c.SendCsPacket(packet); }
                catch (Exception ex) { Logger.Error($"BroadcastMovestate to {c.Identity}: {ex.Message}"); }
            }
        }

        public void BroadcastSequenceState(CSMonsterSequenceState seqState)
        {
            var packet = NewCsPacket.MonsterSequenceState(seqState);
            foreach (Client c in _clientManager.GetAll())
            {
                try { c.SendCsPacket(packet); }
                catch (Exception ex) { Logger.Error($"BroadcastSequenceState to {c.Identity}: {ex.Message}"); }
            }
        }

        /// <summary>
        /// Broadcasts instance finish response (end of mission) to all clients.
        /// </summary>
        public void BroadcastInstanceFinish(CSInstanceFinishRsp rsp)
        {
            var packet = NewCsPacket.InstanceFinishRsp(rsp);
            foreach (Client c in _clientManager.GetAll())
            {
                try { c.SendCsPacket(packet); }
                catch (Exception ex) { Logger.Error($"BroadcastInstanceFinish to {c.Identity}: {ex.Message}"); }
            }
        }

        public void PlayerMoved(Client client)
        {
            // ex : réveiller les monstres proches, recalculer cible, etc.
            // pour l'instant on peut juste logger ou mettre à jour un index
            //Logger.Info($"PlayerMoved: {client.Identity} pos=({client.State.Position.x},{client.State.Position.y},{client.State.Position.z})");
        }

        private static float Distance(CSVec3 a, CSVec3 b)
        {
            float dx = a.x - b.x;
            float dy = a.y - b.y;
            float dz = a.z - b.z;
            return MathF.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        /// <summary>Maps MonsterInfoId to its CryEngine behavior tree path.</summary>
        public static string GetBTState(int monsterInfoId)
        {
            // TODO: build a full mapping from CSV static data
            return monsterInfoId switch
            {
                39002 => @"Em001\em001.xml",
                39003 => @"Em002\em002.xml",
                39004 => @"Em003\em003.xml",
                _ => string.Empty,
            };
        }
    }
}
