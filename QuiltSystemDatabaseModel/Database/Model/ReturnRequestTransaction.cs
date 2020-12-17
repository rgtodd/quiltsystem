//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class ReturnRequestTransaction
    {
        public ReturnRequestTransaction()
        {
            ReturnRequestEvents = new HashSet<ReturnRequestEvent>();
        }

        public long ReturnRequestTransactionId { get; set; }
        public long ReturnRequestId { get; set; }
        public string ReturnRequestStatusCode { get; set; }
        public DateTime TransactionDateTimeUtc { get; set; }
        public string Description { get; set; }
        public string UnitOfWork { get; set; }

        public virtual ReturnRequest ReturnRequest { get; set; }
        public virtual ICollection<ReturnRequestEvent> ReturnRequestEvents { get; set; }
    }
}
