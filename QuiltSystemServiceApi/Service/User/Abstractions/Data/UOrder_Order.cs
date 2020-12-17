//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.User.Abstractions.Data
{
    public class UOrder_Order
    {
        public MOrder_Order MOrder { get; set; }
        public MFulfillment_Fulfillable MFulfillable { get; set; }
    }
}
