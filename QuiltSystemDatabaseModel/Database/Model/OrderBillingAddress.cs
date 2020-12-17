//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class OrderBillingAddress
    {
        public long OrderId { get; set; }
        public string BillToAddressLine1 { get; set; }
        public string BillToAddressLine2 { get; set; }
        public string BillToCity { get; set; }
        public string BillToCountryCode { get; set; }
        public string BillToStateCode { get; set; }
        public string BillToPostalCode { get; set; }
        public string BillToName { get; set; }

        public virtual State BillTo { get; set; }
        public virtual Order Order { get; set; }
    }
}
