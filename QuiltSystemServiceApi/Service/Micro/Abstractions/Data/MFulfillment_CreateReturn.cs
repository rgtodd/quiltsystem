//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MFulfillment_CreateReturn
    {
        public DateTime CreateDateTimeUtc { get; set; }

        public IList<MFulfillment_CreateReturnItem> CreateReturnItems { get; set; }
    }

    public class MFulfillment_CreateReturnItem
    {
        public long ReturnRequestItemId { get; set; }
        public int Quantity { get; set; }
    }
}
