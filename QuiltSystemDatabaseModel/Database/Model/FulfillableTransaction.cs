﻿//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class FulfillableTransaction
    {
        public FulfillableTransaction()
        {
            FulfillableEvents = new HashSet<FulfillableEvent>();
            FulfillableTransactionItems = new HashSet<FulfillableTransactionItem>();
        }

        public long FulfillableTransactionId { get; set; }
        public DateTime TransactionDateTimeUtc { get; set; }
        public string Description { get; set; }
        public string UnitOfWork { get; set; }

        public virtual ICollection<FulfillableEvent> FulfillableEvents { get; set; }
        public virtual ICollection<FulfillableTransactionItem> FulfillableTransactionItems { get; set; }
    }
}
