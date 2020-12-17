﻿//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class InventoryItemUnit
    {
        public long InventoryItemId { get; set; }
        public string UnitOfMeasureCode { get; set; }

        public virtual InventoryItem InventoryItem { get; set; }
        public virtual UnitOfMeasure UnitOfMeasureCodeNavigation { get; set; }
    }
}
