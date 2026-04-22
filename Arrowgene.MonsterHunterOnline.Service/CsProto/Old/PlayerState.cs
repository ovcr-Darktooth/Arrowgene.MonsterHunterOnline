using Arrowgene.Logging;
using Arrowgene.MonsterHunterOnline.Protocol.Old.Structures;

namespace Arrowgene.MonsterHunterOnline.Service;

/// <summary>
///  this is a temporary holder for central management of packet and information,
/// it can be considered the "playground" for now.
///
/// On*- function are lifecycle hooks
/// Send*- functions are to send specific data that has been consistently populated
/// </summary>
public class PlayerState
{
    private static readonly ServiceLogger Logger = LogProvider.Logger<ServiceLogger>(typeof(PlayerState));

    private readonly Client _client;

    public static Server Server;
    public int levelId { get; set; }
    public int prevLevelId { get; set; }
    public CSVec3 Position { get; set; }
    public CSVec3 PendingMonsterSpawnPos { get; set; }
    public uint? PendingMonsterNetId { get; set; }
    public uint? ActiveMonsterNetId { get; private set; }
    public CSVec3 ActiveMonsterPos { get; private set; }
    public int MainInstanceLevelId { get; set; }
    public bool SelectRoleTrigger { get; set; }

    public CSQuatT InitSpawnPose = new CSQuatT()
    {
        q = new CSQuat()
        {
            v = new CSVec3() { x = 10, y = 10, z = 10 },
            w = 10
        },
        t = new CSVec3() { x = 404.91379f, y = 396.74976f, z = 85.0f }
    };

    public CSVec3 InitSpawnPos = new CSVec3()
    {
        x = 404.91379f,
        y = 396.74976f,
        z = 85.0f
    };

    public int InitLevelId = 150101;

    public PlayerState(Client client)
    {
        _client = client;
        if (Position == null)
        {
            Position = InitSpawnPos;
        }
    }

    private static CSVec3 CloneVec(CSVec3 vec)
    {
        return new CSVec3(vec.x, vec.y, vec.z);
    }

    private static string FormatVec(CSVec3 vec)
    {
        return $"({vec.x:F3}, {vec.y:F3}, {vec.z:F3})";
    }
}
