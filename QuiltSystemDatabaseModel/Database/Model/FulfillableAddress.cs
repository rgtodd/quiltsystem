//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class FulfillableAddress
    {
        public long FulfillableId { get; set; }
        public string ShipToAddressLine1 { get; set; }
        public string ShipToAddressLine2 { get; set; }
        public string ShipToCity { get; set; }
        public string ShipToCountryCode { get; set; }
        public string ShipToStateCode { get; set; }
        public string ShipToPostalCode { get; set; }
        public string ShipToName { get; set; }

        public virtual Fulfillable Fulfillable { get; set; }
    }
}
