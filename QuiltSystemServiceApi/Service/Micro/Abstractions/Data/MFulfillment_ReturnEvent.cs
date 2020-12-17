//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MFulfillment_ReturnEvent
    {
        public MFulfillment_ReturnEventTypes EventType { get; set; }
        public string UnitOfWork { get; set; }
        public long ReturnId { get; set; }
        public IList<long> ReturnRequestIds { get; set; }
    }
}
