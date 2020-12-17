//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Service.Ajax.Abstractions.Data
{
    public class XDesign_Design
    {
#pragma warning disable IDE1006 // Naming Styles
        public string className { get; set; }
        //{
        //    get { return nameof(Design_DesignData); }
        //    set { if (value != nameof(Design_DesignData)) throw new ArgumentException("Value does not match class name."); }
        //}

        public string designId { get; set; }
        public string designName { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public XDesign_DesignLayout layout { get; set; }
        public XDesign_DesignBlock[] blocks { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }
}
