//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MOrder_AllocateOrderableResponse
    {
        public long OrderableId { get; set; }
        public string OrderableReference { get; set; }
        public IList<MOrder_AllocateOrderableResponseComponent> Components { get; set; }
    }

    public class MOrder_AllocateOrderableResponseComponent
    {
        public long OrderableComponentId { get; set; }
        public string OrderableComponentReference { get; set; }
    }
}
