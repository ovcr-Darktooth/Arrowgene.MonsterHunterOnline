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
        Logger.Info($"\ngrid: {req.Grid}\ncolumn: {req.Column}");
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
        ast.WriteUInt64(3, Endianness.Big); // character.bagid ? character ?

        // Make tag with ID 3
        tag = TdrTlv.MakeTag(3, TdrTlvType.ID_4_BYTE);
        TdrBuffer.WriteVarUInt32(ast, tag);
        ast.WriteInt32(1, Endianness.Big);  // must be id of item for sure (so 1 = potion)

        tag = TdrTlv.MakeTag(4, TdrTlvType.ID_1_BYTE);
        TdrBuffer.WriteVarUInt32(ast, tag);
        ast.WriteByte(1);                   // category ? 256 category ? dumb to put again the category but why not. 1 = consumables

        tag = TdrTlv.MakeTag(5, TdrTlvType.ID_2_BYTE);
        TdrBuffer.WriteVarUInt32(ast, tag);
        ast.WriteInt16(1, Endianness.Big);  // carrylimit ? count ?

        tag = TdrTlv.MakeTag(6, TdrTlvType.ID_2_BYTE);
        TdrBuffer.WriteVarUInt32(ast, tag);
        ast.WriteInt16(1, Endianness.Big);  // stacklimit ? max i've seen is 9999. or count ?

        tag = TdrTlv.MakeTag(7, TdrTlvType.ID_1_BYTE);
        TdrBuffer.WriteVarUInt32(ast, tag);
        ast.WriteByte(1);                   // item level ? or rarity ? count ?

        tag = TdrTlv.MakeTag(8, TdrTlvType.ID_1_BYTE);
        TdrBuffer.WriteVarUInt32(ast, tag);
        ast.WriteByte(1);                   // item level ? or rarity ? count ?

        // Tag 10
        tag = TdrTlv.MakeTag(10, TdrTlvType.ID_4_BYTE);
        TdrBuffer.WriteVarUInt32(ast, tag);
        var data1 = new List<byte>() { /*0x00, 0x00, 0x00, 0x00*/ };
        ast.WriteUInt32((uint)data1.Count, Endianness.Big); // Count of elements. This should be capped at 32 max.
        for (var i = 0; i < data1.Count; i++)
        {
            ast.WriteByte(data1[i]);
        }

        // Tag 11
        tag = TdrTlv.MakeTag(11, TdrTlvType.ID_4_BYTE);
        TdrBuffer.WriteVarUInt32(ast, tag);
        var data2 = new List<Int32>() { /*0, 0, 0, 0*/ };
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
        unItem.ItemData = new CSGeneralItem() // data built just above
        {
            Data = finalData.GetAllBytes().ToList()
        };
        itemAdd.Structure.ItemList.Add(unItem);

        //Logger.Info($"Item Data length: {finalData.GetAllBytes().ToList().Count}");

        client.SendCsProtoStructurePacket(itemAdd);


        /* This is what Ando came with
        
        (2, ID_8_BYTE): uint64 // what does need 64 ? character.bag ? bagid ?
        (3, ID_4_BYTE): int32  // id, goes more than 32767 so not a 16
        (4, ID_1_BYTE): uint8  // category ?
        (5, ID_2_BYTE): int16  // carrylimit ? count ?
        (6, ID_2_BYTE): int16  // stacklimit ? max i've seen is 9999. or count ?
        (7, ID_1_BYTE): uint8  // item level ? or rarity ? count ?
        (8, ID_1_BYTE): uint8  // rarity ? or item level ? count ?
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

    }
}