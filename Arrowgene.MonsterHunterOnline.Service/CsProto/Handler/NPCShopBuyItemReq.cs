using Arrowgene.Buffers;
using Arrowgene.Logging;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Core;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Enums;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Structures;
using Arrowgene.MonsterHunterOnline.Service.Tdr;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arrowgene.MonsterHunterOnline.Service.CsProto.Handler;

public class NPCShopBuyItemReq : CsProtoStructureHandler<CSNpcShopBuyItemReq>
{
    private static readonly ServiceLogger Logger =
        LogProvider.Logger<ServiceLogger>(typeof(NPCShopBuyItemReq));

    public override CS_CMD_ID Cmd => CS_CMD_ID.CS_CMD_NPCSHOP_BUYITEM_REQ;


    public override void Handle(Client client, CSNpcShopBuyItemReq req)
    {
        Logger.Info($"grid: {req.Grid}\ncolumn: {req.Column}");
        //Just the return to say purchase is ok
        CsProtoStructurePacket<CSNpcShopBuyItemRsp> buyItemRsp = CsProtoResponse.NPCShopBuyItemRsp;
        buyItemRsp.Structure.Ret = 0;
        buyItemRsp.Structure.ShopId = req.ShopId;
        buyItemRsp.Structure.SaleId = 1; //sale id = rowid from itemdata.dat
        buyItemRsp.Structure.Num = 1; // amount
        client.SendCsProtoStructurePacket(buyItemRsp);


        CsProtoStructurePacket<CSItemMgrAddItemNtf> itemAdd = CsProtoResponse.ItemMgrAddItemNtf;
        itemAdd.Structure.Reason = 0;
        itemAdd.Structure.ItemList = new List<CSItemData>();


        StreamBuffer ast = new StreamBuffer();

        //Ando Structure :

        // Make tag with ID2
        uint tag = TdrTlv.MakeTag(2, TdrTlvType.ID_8_BYTE);
        TdrBuffer.WriteVarUInt32(ast, tag);
        ast.WriteUInt64(1, Endianness.Big); // 1, we will go for ones

        // Make tag with ID 3
        tag = TdrTlv.MakeTag(3, TdrTlvType.ID_4_BYTE);
        TdrBuffer.WriteVarUInt32(ast, tag);
        ast.WriteInt32(1, Endianness.Big); // 1

        tag = TdrTlv.MakeTag(4, TdrTlvType.ID_1_BYTE);
        TdrBuffer.WriteVarUInt32(ast, tag);
        ast.WriteByte(1); // 1

        tag = TdrTlv.MakeTag(5, TdrTlvType.ID_2_BYTE);
        TdrBuffer.WriteVarUInt32(ast, tag);
        ast.WriteInt16(2, Endianness.Big); // 1

        tag = TdrTlv.MakeTag(6, TdrTlvType.ID_2_BYTE);
        TdrBuffer.WriteVarUInt32(ast, tag);
        ast.WriteInt16(2, Endianness.Big); // 1

        tag = TdrTlv.MakeTag(7, TdrTlvType.ID_1_BYTE);
        TdrBuffer.WriteVarUInt32(ast, tag);
        ast.WriteByte(1); // 1

        tag = TdrTlv.MakeTag(8, TdrTlvType.ID_1_BYTE);
        TdrBuffer.WriteVarUInt32(ast, tag);
        ast.WriteByte(1); // 1

        // Tag 9
        tag = TdrTlv.MakeTag(10, TdrTlvType.ID_4_BYTE);
        TdrBuffer.WriteVarUInt32(ast, tag);
        var data1 = new List<byte>() { 0x00, 0x00, 0x00, 0x01 };
        ast.WriteUInt32((uint)data1.Count, Endianness.Big); // Count of elements. This should be capped at 32 max.
        for (var i = 0; i < data1.Count; i++)
        {
            ast.WriteByte(data1[i]);
        }

        // Tag 10
        tag = TdrTlv.MakeTag(11, TdrTlvType.ID_4_BYTE);
        TdrBuffer.WriteVarUInt32(ast, tag);
        var data2 = new List<Int32>() { 0, 0, 0, 1 };
        ast.WriteUInt32((uint)data2.Count, Endianness.Big); // Count of elements. This should be capped at 32 max.
        for (var i = 0; i < data2.Count; i++)
        {
            ast.WriteInt32(data2[i], Endianness.Big);
        }

        // Make the TLV header.
        StreamBuffer finalData = new StreamBuffer();
        finalData.WriteByte((byte)TdrTlvMagic.NoVariant);
        var astBytes = ast.GetAllBytes();
        finalData.WriteInt32(astBytes.Length+5, Endianness.Big); // size
        finalData.WriteBytes(astBytes);

        
        CSItemData unItem = new CSItemData();
        unItem.ItemType = 1; // consumable (potion test) (4 bytes)
        unItem.ItemColumn = 1; // slot (2 bytes)
        unItem.ItemGrid = 1; // item grid, wtf is this ?? (2 bytes)
        unItem.ItemData = new CSGeneralItem()  // what should it have ? id/count ? (X bytes) ??? Name ???
        {
            Data = finalData.GetAllBytes().ToList()
        };
        itemAdd.Structure.ItemList.Add(unItem);

        client.SendCsProtoStructurePacket(itemAdd);


        /* This is what he came with
        
        (2, ID_8_BYTE): uint64 // what does need 64 ? bagid ?
        (3, ID_4_BYTE): int32  // id, goes more than 32767 so not a 16
        (4, ID_1_BYTE): uint8  // category ?
        (5, ID_2_BYTE): int16  // carrylimit ?
        (6, ID_2_BYTE): int16  // stacklimit ? max i've seen is 9999
        (7, ID_1_BYTE): uint8  // item level ? or rarity ?
        (8, ID_1_BYTE): uint8  // rarity ? or item level ?
        (10, ID_4_BYTE):       // 
            uint32 count;
            uint8 data[count]; // max 32 bytes
        (11, ID_4_BYTE):       // 
            uint32 count;
            int32 data[count]; // max 32 bytes
        */

        /// Ret values :
        /// 0 = Ok !
        /// 1 = store internal error
        /// 2 = ???
        /// 3 = insuffisiant stars
        /// 4 = backpack/storage full
        /// 5 = not enough funds / materials
        /// 6 = cannot buy more of that item
        /// 7 = ???
        /// 8 = ???
        /// 9 = not enough funds / materials
        /// 10 to 16 = ??? no more return values ?


        //Should check coins/materials

        //else check bag to see if there is a place for the item bought

        //Else check in storage

        //else if no place, do not buy


        /*
         //Start
        dataList.Add((byte)TdrTlvMagic.NoVariant);
        //8 bytes (uint64)
        dataList.Add(0x00);
        dataList.Add(0x00);
        dataList.Add(0x00);
        dataList.Add(0x00);
        dataList.Add(0x00);
        dataList.Add(0x00);
        dataList.Add(0x01);

        //4 bytes
        dataList.Add(0x00);
        dataList.Add(0x00);
        dataList.Add(0x00);
        dataList.Add(0x01);

        //1 b
        dataList.Add(0x01);

        //2 b
        dataList.Add(0x00);
        dataList.Add(0x01);

        //2 b
        dataList.Add(0x00);
        dataList.Add(0x01);

        //1 b
        dataList.Add(0x01);

        //1 b
        dataList.Add(0x01);


        //4


        //4
*/
    }
}