//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Service.Ajax.Abstractions.Data
{
    public class XDesign_FabricStyle
    {
#pragma warning disable IDE1006 // Naming Styles
        public string className
        {
            get { return nameof(XDesign_FabricStyle); }
            set { if (value != nameof(XDesign_FabricStyle)) throw new ArgumentException("Value does not match class name."); }
        }

        public string sku { get; set; }
        public XDesign_Color color { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }
}
