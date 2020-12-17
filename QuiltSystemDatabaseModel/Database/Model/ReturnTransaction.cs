//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class ReturnTransaction
    {
        public ReturnTransaction()
        {
            ReturnEvents = new HashSet<ReturnEvent>();
        }

        public long ReturnTransactionId { get; set; }
        public long ReturnId { get; set; }
        public string ReturnStatusCode { get; set; }
        public DateTime TransactionDateTimeUtc { get; set; }
        public string Description { get; set; }
        public string UnitOfWork { get; set; }

        public virtual Return Return { get; set; }
        public virtual ICollection<ReturnEvent> ReturnEvents { get; set; }
    }
}
