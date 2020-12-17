//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Linq;

using RichTodd.QuiltSystem.Database.Domain;

namespace RichTodd.QuiltSystem.Database.Model.Extensions
{
    public static class InventoryItemExtensions
    {
        public static string Manufacturer(this InventoryItem inventoryItem)
        {
            var dbInventoryItemTag = inventoryItem.InventoryItemTags.Where(r => r.Tag.TagTypeCode == TagTypeCodes.Manufacturer).SingleOrDefault();
            return dbInventoryItemTag?.Tag.Value;
        }

        public static string Collection(this InventoryItem inventoryItem)
        {
            var dbInventoryItemTag = inventoryItem.InventoryItemTags.Where(r => r.Tag.TagTypeCode == TagTypeCodes.Collection).SingleOrDefault();
            return dbInventoryItemTag?.Tag.Value;
        }
    }
}
