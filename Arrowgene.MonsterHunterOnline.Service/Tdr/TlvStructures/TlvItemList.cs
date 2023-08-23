﻿using System;
using System.Collections.Generic;
using Arrowgene.Buffers;

namespace Arrowgene.MonsterHunterOnline.Service.Tdr.TlvStructures;

public class TlvItemList : TlvStructure
{
    private const int ItemsMaxSize = 0x9C4;

    public TlvItemList()
    {
        UnknownA = 0;
        UnknownB = 0;
        Items = new List<TlvItem>();
    }

    public short UnknownA { get; set; }
    public short UnknownB { get; set; }
    public List<TlvItem> Items { get; }

    public override void Write(IBuffer buffer)
    {
        WriteByte(buffer, (byte)TlvMagic.NoVariant);
        int startPos = buffer.Position;
        WriteInt32(buffer, 0);

        // case 1
        WriteTlvInt16(buffer, 1, UnknownA);

        // case 2
        int maxItems = Math.Min(Items.Count, ItemsMaxSize);
        if (maxItems > 0)
        {
            WriteTlvTag(buffer, 2, TlvType.ID_4_BYTE);
            int subStartPos = buffer.Position;
            WriteInt32(buffer, 0);
            for (int i = 0; i < maxItems; i++)
            {
                WriteTlvStructure(buffer, Items[i]);
            }

            int subEndPos = buffer.Position;
            int subSize = buffer.Position - subStartPos - 4;
            buffer.Position = subStartPos;
            buffer.WriteInt32(subSize, Endianness.Big);
            buffer.Position = subEndPos;
        }

        // case 3
        WriteTlvInt16(buffer, 3, UnknownA);

        int endPos = buffer.Position;
        int size = endPos - startPos + 1;
        buffer.Position = startPos;
        WriteInt32(buffer, size);
        buffer.Position = endPos;
    }

    public override void Read(IBuffer buffer)
    {
        throw new NotImplementedException();
    }
}