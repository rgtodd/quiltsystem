//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MKit_KitSpecificationOptions
    {
        public IList<MKit_Size> StandardSizes { get; set; }
        public IList<MKit_Dimension> StandardBorderWidths { get; set; }
        public IList<MKit_Dimension> StandardBindingWidths { get; set; }
    }
}
