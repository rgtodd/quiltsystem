//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Service.Ajax.Abstractions.Data
{
    // Describes a block.  Referenced by quilt.block-list.js.
    //
    public class XDesign_Block
    {
#pragma warning disable IDE1006 // Naming Styles
        public string className
        {
            get { return nameof(XDesign_Block); }
            set { if (value != nameof(XDesign_Block)) throw new ArgumentException("Value does not match class name."); }
        }

        public string id { get; set; }
        public string blockCategory { get; set; }
        public string blockGroup { get; set; }
        public string blockName { get; set; }
        public XDesign_FabricStyle[] fabricStyles { get; set; }
        public XDesign_BlockPreview preview { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }
}
