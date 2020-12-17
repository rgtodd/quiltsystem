﻿//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class ShipmentRequestToShipmentView
    {
        public long ShipmentRequestId { get; set; }
        public long ShipmentId { get; set; }
    }
}
