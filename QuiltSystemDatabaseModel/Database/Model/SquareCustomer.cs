//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class SquareCustomer
    {
        public SquareCustomer()
        {
            SquarePayments = new HashSet<SquarePayment>();
        }

        public long SquareCustomerId { get; set; }
        public string SquareCustomerReference { get; set; }
        public DateTime UpdateDateTimeUtc { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual ICollection<SquarePayment> SquarePayments { get; set; }
    }
}
