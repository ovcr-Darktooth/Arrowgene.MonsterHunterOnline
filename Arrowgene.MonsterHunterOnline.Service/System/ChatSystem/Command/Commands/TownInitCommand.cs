using Arrowgene.MonsterHunterOnline.Protocol.Old.Structures;
using Arrowgene.MonsterHunterOnline.Protocol.Constant;
using Arrowgene.MonsterHunterOnline.Protocol.Structures;
using Microsoft.VisualBasic.FileIO;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Arrowgene.MonsterHunterOnline.Service.CsProto;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Core;

namespace Arrowgene.MonsterHunterOnline.Service.System.ChatSystem.Command.Commands;

/// <summary>
/// Sends CS_CMD_TOWN_SERVER_INIT_NTF packet
/// </summary>
public class TownInitCommand : ChatCommand
{
    public override AccountType Account => AccountType.User;
    public override string Key => "town_init";
    public override string HelpText => "usage: `/town_init` - Sends CS_CMD_TOWN_SERVER_INIT_NTF packet";

    public override void Execute(string[] command, Client client, ChatMessage message, List<ChatMessage> responses)
    {
        CsCsProtoStructurePacket<TownInstanceVerifyRsp> townServerInitNtf = CsProtoResponse.TownServerInitNtf;
        TownInstanceVerifyRsp verifyRsp = townServerInitNtf.Structure;
        verifyRsp.ErrNo = 0;
        verifyRsp.LineId = 0;
        verifyRsp.LevelEnterType = 0;

        InstanceInitInfo instanceInitInfo = verifyRsp.InstanceInitInfo;
        instanceInitInfo.BattleGroundId = 1;
        //instanceInitInfo.LevelId = 180101;

        if (client.State.levelId == 100403)
            instanceInitInfo.LevelId = 180101; //town
        else
            instanceInitInfo.LevelId = 100403;
        //instanceInitInfo.LevelId = 140900; //Weapon Training with NPCs
        if (command.Length > 0 && int.TryParse(command[0], out int level))
            instanceInitInfo.LevelId = level;
        else if (client.State.levelId == 0)
            instanceInitInfo.LevelId = 140900;
        else
            instanceInitInfo.LevelId = client.State.levelId;

        //instanceInitInfo.LevelId = 140501;
        //instanceInitInfo.LevelId = client.State.levelId;
        instanceInitInfo.CreateMaxPlayerCount = 4;
        instanceInitInfo.GameMode = GameMode.Town;
        //instanceInitInfo.GameMode = GameMode.Story;
        //instanceInitInfo.GameMode = GameMode.ExtremeA;
        instanceInitInfo.TimeType = TimeType.Noon;
        instanceInitInfo.WeatherType = WeatherType.Sunny;
        instanceInitInfo.Time = 1;
        instanceInitInfo.LevelRandSeed = 1;
        instanceInitInfo.WarningFlag = 0;
        instanceInitInfo.CreatePlayerMaxLv = 99;

        string staticFolder = Path.Combine(Util.ExecutingDirectory(), "Files", "Static");
        string csvSpawnPointsPath = Path.Combine(staticFolder, "SpawnPoints.csv");
        //int level = client.State.levelId;
        level = instanceInitInfo.LevelId;
        using (TextFieldParser parser = new TextFieldParser(csvSpawnPointsPath))
        {
            string level_comp = level.ToString();
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");

            // Skip the header line
            parser.ReadLine();
            while (!parser.EndOfData)
            {
                string[] fields = parser.ReadFields();
                string levelId = fields[0];
                bool isMatch = (level_comp.Contains(levelId) || levelId.Contains(level_comp));
                if (isMatch)
                {
                    string filename = fields[1];
                    string areaName = fields[2];
                    string pos = fields[3];
                    string rotate = fields[4];

                    //Logger.Info($"warp point match found: ({levelId})({filename})({areaName})({name})");
                    // Process the position (Pos) and rotation (Rotate) values
                    string[] posValues = pos.Split(',');
                    string[] rotateValues = rotate.Split(',');

                    float posX = float.Parse(posValues[0], CultureInfo.InvariantCulture);
                    float posY = float.Parse(posValues[1], CultureInfo.InvariantCulture);
                    float posZ = float.Parse(posValues[2], CultureInfo.InvariantCulture);

                    float rotateX = float.Parse(rotateValues[0], CultureInfo.InvariantCulture);
                    float rotateY = float.Parse(rotateValues[1], CultureInfo.InvariantCulture);
                    float rotateZ = float.Parse(rotateValues[2], CultureInfo.InvariantCulture);
                    float rotateW = float.Parse(rotateValues[3], CultureInfo.InvariantCulture);

                    client.SendCsPacket(NewCsPacket.PlayerTeleport(new CSPlayerTeleport()
                    {
                        SyncTime = 0,
                        NetObjId = client.Character.Id,
                        Region = client.State.levelId,
                        TargetPos = new CSQuatT()
                        {
                            q = new CSQuat()
                            {
                                v = new CSVec3() { x = rotateX, y = rotateY, z = rotateZ },
                                w = rotateW
                            },
                            t = new CSVec3() { x = (float)posX, y = (float)posY, z = (float)posZ }
                        },
                        ParentGUID = 1,
                        InitState = 1
                    }
                    ));

                    break;
                }
            }
        }

        client.SendCsProtoStructurePacket(townServerInitNtf);

        // TODO, perhaps refactor to a change level method somewhere to handle
        client.State.prevLevelId = client.State.levelId;
        client.State.levelId = instanceInitInfo.LevelId;
    }
}