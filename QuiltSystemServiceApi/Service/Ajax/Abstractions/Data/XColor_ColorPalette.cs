//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Service.Ajax.Abstractions.Data
{
    public class XColor_ColorPalette
    {
        public string SelectedHue { get; set; }
        public string NextHue { get; set; }
        public string PreviousHue { get; set; }
        public bool ShowBaseColors { get; set; }
        public XColor_Color SelectedColor { get; set; }
        public XColor_Color[] RelatedColors { get; set; }
        public XColor_Color[] LighterSaturatedColors { get; set; }
        public XColor_Color[] LighterDesaturatedColors { get; set; }
        public XColor_Color[] DarkerSaturatedColors { get; set; }
        public XColor_Color[] DarkerDesaturatedColors { get; set; }
        public XColor_Color[,] Palette { get; set; }
        public XColor_Color[] AllColors { get; set; }
    }
}