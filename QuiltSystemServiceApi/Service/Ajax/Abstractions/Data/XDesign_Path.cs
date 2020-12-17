//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Service.Ajax.Abstractions.Data
{
    public class XDesign_Path
    {
#pragma warning disable IDE1006 // Naming Styles
        public string className
        {
            get { return nameof(XDesign_Path); }
            set { if (value != nameof(XDesign_Path)) throw new ArgumentException("Value does not match class name."); }
        }

        public XDesign_Point[] points { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }
}
