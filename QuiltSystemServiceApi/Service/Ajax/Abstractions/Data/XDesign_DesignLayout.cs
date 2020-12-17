//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Service.Ajax.Abstractions.Data
{
    public class XDesign_DesignLayout
    {
#pragma warning disable IDE1006 // Naming Styles
        public string className
        {
            get { return nameof(XDesign_DesignLayout); }
            set { if (value != nameof(XDesign_DesignLayout)) throw new ArgumentException("Value does not match class name."); }
        }

        public string layoutCategory { get; set; }
        public string layoutName { get; set; }
        public XDesign_FabricStyle[] fabricStyles { get; set; }
        public int rowCount { get; set; }
        public int columnCount { get; set; }
        public int blockCount { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }
}
