//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class UserAddress
    {
        public long UserAddressId { get; set; }
        public long UserProfileId { get; set; }
        public string AddressTypeCode { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string CountryCode { get; set; }
        public string StateCode { get; set; }
        public string PostalCode { get; set; }

        public virtual AddressType AddressTypeCodeNavigation { get; set; }
        public virtual State State { get; set; }
        public virtual UserProfile UserProfile { get; set; }
    }
}
