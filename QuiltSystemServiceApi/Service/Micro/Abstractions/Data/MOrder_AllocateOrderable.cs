//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MOrder_AllocateOrderable
    {
        public string OrderableReference { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public IList<MOrder_AllocateOrderableComponent> Components { get; set; }
    }

    public class MOrder_AllocateOrderableComponent
    {
        public string OrderableComponentReference { get; set; }
        public string Description { get; set; }
        public string ConsumableReference { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
