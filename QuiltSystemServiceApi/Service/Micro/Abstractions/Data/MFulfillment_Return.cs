//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MFulfillment_Return
    {
        public long ReturnId { get; set; }
        public string ReturnNumber { get; set; }
        public MFulfillment_ReturnStatus ReturnStatus { get; set; }
        public DateTime StatusDateTimeUtc { get; set; }
        public DateTime CreateDateTimeUtc { get; set; }
        public bool CanEdit { get; set; }
        public bool CanPost { get; set; }
        public bool CanProcess { get; set; }
        public bool CanCancel { get; set; }
        public IList<MFulfillment_ReturnItem> ReturnItems { get; set; }
    }

    public class MFulfillment_ReturnItem
    {
        public long ReturnItemId { get; set; }
        public long FulfillableItemId { get; set; }
        public string FulfillableItemReference { get; set; }
        public int Quantity { get; set; }

        // Convienence properties
        //
        public long FulfillableId { get; set; }
        public long ReturnRequestId { get; set; }
        public string ReturnRequestNumber { get; set; }
        public long ReturnRequestItemId { get; set; }
    }
}
