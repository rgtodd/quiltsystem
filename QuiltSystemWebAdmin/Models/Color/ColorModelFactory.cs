//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

using RichTodd.QuiltSystem.Service.Ajax.Abstractions.Data;
using RichTodd.QuiltSystem.Web;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Color
{
    public class ColorModelFactory : ApplicationModelFactory
    {

        public ColorPaletteModel CreateColorPaletteModel(XColor_ColorPalette svcColorPalette)
        {
            var model = new ColorPaletteModel()
            {
                AllColors = CreateColorModelArray(svcColorPalette.AllColors),
                DarkerDesaturatedColors = CreateColorModelArray(svcColorPalette.DarkerDesaturatedColors),
                DarkerSaturatedColors = CreateColorModelArray(svcColorPalette.DarkerSaturatedColors),
                LighterDesaturatedColors = CreateColorModelArray(svcColorPalette.LighterDesaturatedColors),
                LighterSaturatedColors = CreateColorModelArray(svcColorPalette.LighterSaturatedColors),
                NextHue = svcColorPalette.NextHue,
                Palette = CreateColorModelArray(svcColorPalette.Palette),
                PreviousHue = svcColorPalette.PreviousHue,
                RelatedColors = CreateColorModelArray(svcColorPalette.RelatedColors),
                SelectedColor = CreateColorModel(svcColorPalette.SelectedColor),
                SelectedHue = svcColorPalette.SelectedHue,
                ShowBaseColors = svcColorPalette.ShowBaseColors
            };

            return model;
        }

        private ColorModel CreateColorModel(XColor_Color svcColor)
        {
            var model = new ColorModel()
            {
                Hue = svcColor.Hue,
                Lightness = svcColor.Lightness,
                Name = svcColor.Name,
                Saturation = svcColor.Saturation,
                Sku = svcColor.Sku,
                WebColor = svcColor.WebColor
            };

            return model;
        }

        private ColorModel[,] CreateColorModelArray(XColor_Color[,] svcColors)
        {
            int rowCount = svcColors.GetLength(0);
            int columnCount = svcColors.GetLength(1);

            var model = new ColorModel[rowCount, columnCount]; ;

            for (var row = 0; row < rowCount; ++row)
            {
                for (var column = 0; column < columnCount; ++column)
                {
                    model[row, column] = CreateColorModel(svcColors[row, column]);
                }
            }

            return model;
        }

        private ColorModel[] CreateColorModelArray(IEnumerable<XColor_Color> svcColors)
        {
            var model = new List<ColorModel>();

            foreach (var svcColor in svcColors)
            {
                model.Add(CreateColorModel(svcColor));
            }

            return model.ToArray();
        }

    }
}