//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class Fulfillable
    {
        public Fulfillable()
        {
            FulfillableItems = new HashSet<FulfillableItem>();
        }

        public long FulfillableId { get; set; }
        public string FulfillableReference { get; set; }
        public string Name { get; set; }
        public DateTime CreateDateTimeUtc { get; set; }
        public string FulfillableStatusCode { get; set; }
        public DateTime FulfillableStatusDateTimeUtc { get; set; }
        public DateTime UpdateDateTimeUtc { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual FulfillableAddress FulfillableAddress { get; set; }
        public virtual ICollection<FulfillableItem> FulfillableItems { get; set; }
    }
}
