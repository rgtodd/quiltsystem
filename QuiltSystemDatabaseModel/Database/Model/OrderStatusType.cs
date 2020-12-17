//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class OrderStatusType
    {
        public OrderStatusType()
        {
            OrderTransactions = new HashSet<OrderTransaction>();
            Orders = new HashSet<Order>();
        }

        public string OrderStatusCode { get; set; }
        public string Name { get; set; }

        public virtual ICollection<OrderTransaction> OrderTransactions { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
