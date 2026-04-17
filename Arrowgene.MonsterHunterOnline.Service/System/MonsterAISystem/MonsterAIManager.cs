using System;
using System.Collections.Generic;
using System.Threading;
using Arrowgene.Logging;
using Arrowgene.MonsterHunterOnline.Protocol.Old.Structures;
using Arrowgene.MonsterHunterOnline.Service.CsProto;

namespace Arrowgene.MonsterHunterOnline.Service.System.MonsterAISystem
{
    public class MonsterAIManager
    {
        private static readonly ILogger Logger = LogProvider.Logger(typeof(MonsterAIManager));

        // NetIds for server-spawned monsters start at 1000 to avoid colliding with player IDs
        private static uint _nextNetId = 1000;

        private ClientManager _clientManager;
        private readonly Dictionary<uint, MonsterAI> _monsters = new();
        private readonly object _lock = new();

        public MonsterAIManager(ClientManager clientManager)
        {
            _clientManager = clientManager;
        }

        public uint NextNetId() => Interlocked.Increment(ref _nextNetId);

        /// <summary>Registers a monster and starts its AI loop.</summary>
        public MonsterAI Spawn(uint netId, int monsterInfoId, CSVec3 position)
        {
            var monster = new MonsterAI(netId, monsterInfoId, position, this);
            lock (_lock)
            {
                _monsters[netId] = monster;
            }
            Logger.Info($"MonsterAI spawned: netId={netId} infoId={monsterInfoId} pos=({position.x:F1},{position.y:F1},{position.z:F1})");
            return monster;
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

        public void PlayerMoved(Client client)
        {
            // ex : réveiller les monstres proches, recalculer cible, etc.
            // pour l'instant on peut juste logger ou mettre à jour un index
            Logger.Info($"PlayerMoved: {client.Identity} pos=({client.State.Position.x},{client.State.Position.y},{client.State.Position.z})");
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
