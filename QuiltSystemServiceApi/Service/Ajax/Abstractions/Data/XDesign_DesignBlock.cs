//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Service.Ajax.Abstractions.Data
{
    public class XDesign_DesignBlock
    {
        public static XDesign_DesignBlock[] EmptyArray { get; } = Array.Empty<XDesign_DesignBlock>();

#pragma warning disable IDE1006 // Naming Styles
        public string className
        {
            get { return nameof(XDesign_DesignBlock); }
            set { if (value != nameof(XDesign_DesignBlock)) throw new ArgumentException("Value does not match class name."); }
        }

        public string blockCategory { get; set; }
        public string blockName { get; set; }
        public XDesign_FabricStyle[] fabricStyles { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }
}
