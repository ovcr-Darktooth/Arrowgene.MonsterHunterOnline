﻿using System;
using Arrowgene.MonsterHunterOnline.Service.System.ItemSystem;
using Arrowgene.MonsterHunterOnline.Service.System.ItemSystem.Constant;

namespace Arrowgene.MonsterHunterOnline.Service.System.ClientAssetSystem
{
    public class EquipDataCsv : CsvReaderWriter<ItemData>
    {
        protected override int NumExpectedItems => 26;

        protected override ItemData CreateInstance(string[] properties)
        {
            if (!int.TryParse(properties[0], out int itemId)) return null;
            string name = properties[1];
            if (!Enum.TryParse(properties[9], out ItemClass mainClass)) return null;
            if (!Enum.TryParse(properties[10], out ItemCategory category)) return null;
            if (!uint.TryParse(properties[11], out uint subCategory)) return null;
            if (!uint.TryParse(properties[12], out uint bindingType)) return null;
            if (!uint.TryParse(properties[18], out uint price)) return null;
            if (!uint.TryParse(properties[19], out uint salePrice)) return null;

            return new ItemData
            {
                ItemId = itemId,
                Name = name,
                MainClass = mainClass,
                Category = category,
                SubCategory = subCategory,
                BindingType = bindingType,
                PortableLimit = 0,
                StackLimit = 1,
                Price = price,
                SalePrice = salePrice,
            };
        }
    }
}