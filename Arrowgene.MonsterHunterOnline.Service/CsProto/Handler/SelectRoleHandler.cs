using Arrowgene.Logging;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Core;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Enums;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Structures;
using Arrowgene.MonsterHunterOnline.Service.System;
using System.Collections.Generic;

namespace Arrowgene.MonsterHunterOnline.Service.CsProto.Handler;

public class SelectRoleHandler : CsProtoStructureHandler<SelectRoleReq>
{
    private static readonly ServiceLogger Logger = LogProvider.Logger<ServiceLogger>(typeof(SelectRoleHandler));


    public override CS_CMD_ID Cmd => CS_CMD_ID.CS_CMD_SELECT_ROLE_REQ;

    private readonly CharacterManager _characterManager;

    public SelectRoleHandler(CharacterManager characterManager)
    {
        _characterManager = characterManager;
    }

    public override void Handle(Client client, SelectRoleReq req)
    {
        Character character = _characterManager.GetCharacter(client.Account.Id, (byte)req.RoleIndex);
        if (character == null)
        {
            Logger.Error(client, $"character for index:{req.RoleIndex} not found");
            return;
        }

        client.Character = character;

        CsProtoStructurePacket<SelectRoleRsp> selectRoleRsp = CsProtoResponse.SelectRoleRsp;
        selectRoleRsp.Structure.RoleIndex = req.RoleIndex;
        selectRoleRsp.Structure.ErrNo = 0;
        client.SendCsProtoStructurePacket(selectRoleRsp);

        CsProtoStructurePacket<TownSessionStart> townSessionStart = CsProtoResponse.TownSessionStart;
        townSessionStart.Structure.ErrNo = 0;
        client.SendCsProtoStructurePacket(townSessionStart);

        CsProtoStructurePacket<PlayerInitInfo> playerInitInfo = CsProtoResponse.PlayerInitInfo;
        _characterManager.PopulatePlayerInitInfo(client, client.Character, playerInitInfo.Structure);
        client.SendCsProtoStructurePacket(playerInitInfo);

        CsProtoStructurePacket<CSMonsterSizeNtf> monsterSize = CsProtoResponse.MonsterSizeNtf;

        monsterSize.Structure.Infos = new List<CSMonsterSizeInfo>();
        CSMonsterSizeInfo monsterSizeInfo = new CSMonsterSizeInfo();
        //monsterSizeInfo.MonsterID = 66;
        monsterSizeInfo.MinSize = 1000f;
        monsterSizeInfo.MaxSize = 2000f;
        monsterSizeInfo.SizeLevel = 1;

        //monsterSize.Structure.Infos.Add(monsterSizeInfo);

        //_client.SendCsProtoStructurePacket(monsterSize);

        List<int> monsterId = new List<int>()
        {
            0, // no need
            60010,
            60011,
            60012,
            60030,
            60080,
            60081,
            60170,
            60171,
            60480,
            60481,
            60420,
            60020,
            60022,
            60024,
            60060,
            60040,
            60041,
            60070,
            60071,
            60090,
            60091,
            60140,
            60141,
            60143,
            60190,
            60191,
            60050,
            60051,
            60100,
            60102,
            60150,
            60152,
            60220,
            60221,
            60222,
            60260,
            60261,
            60490,
            60110,
            60160,
            60290,
            60120,
            60121,
            60300,
            60200,
            60201,
            60202,
            60210,
            60211,
            60218,
            60230,
            60231,
            60232,
            60250,
            60251,
            60280,
            60390,
            60180,
            60181,
            60240,
            60242,
            60310,
            60311,
            60330,
            60370,
            60130,
            60131,
            60151,
            60870,
            60900,
            60134,
            60350,
            60880,
            60410,
            60491,
            60270,
            60920,
            60281,
            60910,
            60630,
            60440,
            60212,
            60391,
            60252,
            60301,
            60450,
            60620,
            60890,
            60470,
            60400,
            60580,
            60441,
            60380,
            60421,
            60340,
            60341,
            60631,
            0, // no 98th id
            60871,
            60360,
            60710,
            60361,
            60800,
            60670,
            60930
        };

        for (int i = 1; i < 105; i++)
        {
            if (i != 98)
            {
                monsterSizeInfo.MonsterID = monsterId[i];
                monsterSizeInfo.SizeLevel = i;

                monsterSize.Structure.Infos.Add(monsterSizeInfo);
            }
        }
        client.SendCsProtoStructurePacket(monsterSize);
    }
}