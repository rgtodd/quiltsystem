//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class State
    {
        public State()
        {
            OrderBillingAddresses = new HashSet<OrderBillingAddress>();
            OrderShippingAddresses = new HashSet<OrderShippingAddress>();
            UserAddresses = new HashSet<UserAddress>();
        }

        public string StateCode { get; set; }
        public string CountryCode { get; set; }
        public string Name { get; set; }

        public virtual Country CountryCodeNavigation { get; set; }
        public virtual ICollection<OrderBillingAddress> OrderBillingAddresses { get; set; }
        public virtual ICollection<OrderShippingAddress> OrderShippingAddresses { get; set; }
        public virtual ICollection<UserAddress> UserAddresses { get; set; }
    }
}
