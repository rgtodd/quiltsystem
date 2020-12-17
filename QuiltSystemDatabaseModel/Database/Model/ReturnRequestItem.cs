//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class ReturnRequestItem
    {
        public ReturnRequestItem()
        {
            ReturnItems = new HashSet<ReturnItem>();
        }

        public long ReturnRequestItemId { get; set; }
        public long ReturnRequestId { get; set; }
        public long FulfillableItemId { get; set; }
        public int Quantity { get; set; }

        public virtual FulfillableItem FulfillableItem { get; set; }
        public virtual ReturnRequest ReturnRequest { get; set; }
        public virtual ICollection<ReturnItem> ReturnItems { get; set; }
    }
}
