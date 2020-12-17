//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class Tag
    {
        public Tag()
        {
            InventoryItemTags = new HashSet<InventoryItemTag>();
            ResourceTags = new HashSet<ResourceTag>();
        }

        public int TagId { get; set; }
        public string TagTypeCode { get; set; }
        public string Value { get; set; }
        public DateTime CreateDateTimeUtc { get; set; }

        public virtual TagType TagTypeCodeNavigation { get; set; }
        public virtual ICollection<InventoryItemTag> InventoryItemTags { get; set; }
        public virtual ICollection<ResourceTag> ResourceTags { get; set; }
    }
}
