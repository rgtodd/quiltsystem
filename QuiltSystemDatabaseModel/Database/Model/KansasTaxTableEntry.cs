//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class KansasTaxTableEntry
    {
        public long KansasTaxTableEntryId { get; set; }
        public long KansasTaxTableId { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public decimal InsideCityTaxRate { get; set; }
        public string InsideCityJurisdictionCode { get; set; }
        public string County { get; set; }
        public decimal OutsideCityTaxRate { get; set; }
        public string OutsideCityJurisdictionCode { get; set; }

        public virtual KansasTaxTable KansasTaxTable { get; set; }
    }
}
