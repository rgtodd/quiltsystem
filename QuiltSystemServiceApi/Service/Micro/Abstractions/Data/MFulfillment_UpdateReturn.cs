//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MFulfillment_UpdateReturn
    {
        public DateTime CreateDateTimeUtc { get; set; }

        public IList<MFulfillment_UpdateReturnItem> UpdateReturnItems { get; set; }
    }

    public class MFulfillment_UpdateReturnItem
    {
        public long ReturnItemId { get; set; }
        public int Quantity { get; set; }
    }
}
