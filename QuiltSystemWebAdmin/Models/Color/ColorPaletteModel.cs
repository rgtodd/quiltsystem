//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.WebAdmin.Models.Color
{
    public class ColorPaletteModel
    {
        public string SelectedHue { get; set; }
        public string NextHue { get; set; }
        public string PreviousHue { get; set; }
        public bool ShowBaseColors { get; set; }
        public ColorModel SelectedColor { get; set; }
        public ColorModel[] RelatedColors { get; set; }
        public ColorModel[] LighterSaturatedColors { get; set; }
        public ColorModel[] LighterDesaturatedColors { get; set; }
        public ColorModel[] DarkerSaturatedColors { get; set; }
        public ColorModel[] DarkerDesaturatedColors { get; set; }
        public ColorModel[,] Palette { get; set; }
        public ColorModel[] AllColors { get; set; }
    }
}