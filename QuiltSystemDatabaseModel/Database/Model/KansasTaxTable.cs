//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class KansasTaxTable
    {
        public KansasTaxTable()
        {
            KansasTaxTableEntries = new HashSet<KansasTaxTableEntry>();
        }

        public long KansasTaxTableId { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpirationDate { get; set; }

        public virtual ICollection<KansasTaxTableEntry> KansasTaxTableEntries { get; set; }
    }
}
