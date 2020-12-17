//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MFulfillment_ShipmentEvent
    {
        public MFulfillment_ShipmentEventTypes EventType { get; set; }
        public string UnitOfWork { get; set; }
        public long ShipmentId { get; set; }
        public IList<long> ShipmentRequestIds { get; set; }
    }
}
