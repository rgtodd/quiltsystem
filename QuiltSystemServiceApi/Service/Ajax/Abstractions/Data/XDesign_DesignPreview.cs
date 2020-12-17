//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Service.Ajax.Abstractions.Data
{
    public class XDesign_DesignPreview
    {
#pragma warning disable IDE1006 // Naming Styles
        public string className
        {
            get { return nameof(XDesign_DesignPreview); }
            set { if (value != nameof(XDesign_DesignPreview)) throw new ArgumentException("Value does not match class name."); }
        }

        public XDesign_Shape[] shapes { get; set; }
        public XDesign_LayoutSite[] layoutSites { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }
}
