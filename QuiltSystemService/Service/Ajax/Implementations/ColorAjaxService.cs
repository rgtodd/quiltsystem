//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using RichTodd.QuiltSystem.Service.Admin.Abstractions;
using RichTodd.QuiltSystem.Service.Ajax.Abstractions;
using RichTodd.QuiltSystem.Service.Ajax.Abstractions.Data;
using RichTodd.QuiltSystem.Service.Base;
using RichTodd.QuiltSystem.Service.Core.Abstractions;

namespace RichTodd.QuiltSystem.Service.Ajax.Implementations
{
    internal class ColorAjaxService : BaseService, IColorAjaxService
    {
        private IInventoryAdminService InventoryService { get; }

        public ColorAjaxService(
             IApplicationRequestServices requestServices,
             ILogger<ColorAjaxService> logger,
             IInventoryAdminService inventoryService)
             : base(requestServices, logger)
        {
            InventoryService = inventoryService ?? throw new ArgumentNullException(nameof(inventoryService));
        }

        public async Task<XColor_ColorPalette> CreateColorPaletteAsync(string webColor)
        {
            var rowCount = 22;
            var columnCount = 21;

            var color = webColor == null ? Design.Primitives.Color.FromAhsb(255, 0.0, 0.5, 1.0) : Design.Primitives.Color.FromWebColor(webColor);
            var result = await CreateColorPaletteModelAsync(color, rowCount, columnCount).ConfigureAwait(false);

            return result;
        }

        private XColor_Color[] CreateAllHues(int hueCount, int hueDegrees)
        {
            var hues = new XColor_Color[hueCount];
            for (var idx = 0; idx < hueCount; ++idx)
            {
                var adjustedHue = Design.Primitives.Color.AdjustedHueToHue(idx * hueDegrees);
                var adjustedColor = Design.Primitives.Color.FromAhsb(255, adjustedHue, 1, 0.5);

                hues[idx] = CreateColorData(adjustedColor);
            }

            return hues;
        }

        private XColor_Color CreateColorData(FabricColor fabricColor)
        {
            return new XColor_Color()
            {
                WebColor = fabricColor.Color.WebColor,
                Hue = fabricColor.Color.Hue,
                Saturation = fabricColor.Color.Saturation,
                Lightness = fabricColor.Color.Brightness,
                Sku = fabricColor.Sku,
                Name = fabricColor.Name
            };
        }

        private XColor_Color CreateColorData(Design.Primitives.Color color)
        {
            return new XColor_Color()
            {
                WebColor = color.WebColor,
                Hue = color.Hue,
                Saturation = color.Saturation,
                Lightness = color.Brightness,
                Sku = "Unknown",
                Name = "Unknown"
            };
        }

        private async Task<XColor_ColorPalette> CreateColorPaletteModelAsync(Design.Primitives.Color hueColor, int rowCount, int columnCount)
        {
            var fabricColors = await GetFabricColors(hueColor).ConfigureAwait(false);

            var hueCount = 45;
            var hueDegrees = 8;

            var model = new XColor_ColorPalette()
            {
                SelectedHue = hueColor.WebColor,
                PreviousHue = Design.Primitives.Color.FromAhsb(255, RoundHue((int)hueColor.Hue - hueDegrees), 0.5, 1.0).WebColor,
                NextHue = Design.Primitives.Color.FromAhsb(255, RoundHue((int)hueColor.Hue + hueDegrees), 0.5, 1.0).WebColor,
                SelectedColor = CreateColorData(hueColor),
                AllColors = CreateAllHues(hueCount, hueDegrees),
                RelatedColors = CreateRelatedHues(hueColor, hueDegrees, fabricColors),
                Palette = CreatePalette((int)hueColor.Hue, rowCount, columnCount, fabricColors),
                LighterSaturatedColors = CreateLighterSaturatedHues(hueColor, fabricColors),
                LighterDesaturatedColors = CreateLighterDesaturatedHues(hueColor, fabricColors),
                DarkerSaturatedColors = CreateDarkerSaturatedHues(hueColor, fabricColors),
                DarkerDesaturatedColors = CreateDarkerDesaturatedHues(hueColor, fabricColors)
            };

            return model;
        }

        private XColor_Color[] CreateDarkerDesaturatedHues(Design.Primitives.Color hueColor, IList<FabricColor> fabricColors)
        {
            var selectedFabricColors = new List<FabricColor>();
            foreach (var fabricColor in fabricColors)
            {
                if (fabricColor.Color.Brightness < hueColor.Brightness &&
                    fabricColor.Color.Saturation < hueColor.Saturation)
                {
                    selectedFabricColors.Add(fabricColor);
                }
            }
            selectedFabricColors.Sort((lhs, rhs) => -lhs.Color.Brightness.CompareTo(rhs.Color.Brightness));
            //var d1 = Design.Primitives.Color.DistanceSquared(lhs, hueColor);
            //var d2 = Design.Primitives.Color.DistanceSquared(rhs, hueColor);
            //return d1.CompareTo(d2);

            var selectedColorModels = new List<XColor_Color>();
            foreach (var desaturatedColor in selectedFabricColors)
            {
                selectedColorModels.Add(CreateColorData(desaturatedColor));
            }

            return selectedColorModels.ToArray();
        }

        private XColor_Color[] CreateDarkerSaturatedHues(Design.Primitives.Color hueColor, IList<FabricColor> fabricColors)
        {
            var selectedFabricColors = new List<FabricColor>();
            foreach (var fabricColor in fabricColors)
            {
                if (fabricColor.Color.Brightness < hueColor.Brightness &&
                    fabricColor.Color.Saturation >= hueColor.Saturation)
                {
                    selectedFabricColors.Add(fabricColor);
                }
            }
            selectedFabricColors.Sort((lhs, rhs) => -lhs.Color.Brightness.CompareTo(rhs.Color.Brightness));
            //var d1 = Design.Primitives.Color.DistanceSquared(lhs, hueColor);
            //var d2 = Design.Primitives.Color.DistanceSquared(rhs, hueColor);
            //return d1.CompareTo(d2);

            var selectedColorModels = new List<XColor_Color>();
            foreach (var darkerColor in selectedFabricColors)
            {
                selectedColorModels.Add(CreateColorData(darkerColor));
            }

            return selectedColorModels.ToArray();
        }

        private FabricColor CreateFabricColor(Design.Primitives.Color color, string sku, string name)
        {
            return new FabricColor()
            {
                Color = color,
                Sku = sku,
                Name = name
            };
        }

        private XColor_Color[] CreateLighterDesaturatedHues(Design.Primitives.Color hueColor, IList<FabricColor> fabricColors)
        {
            var selectedFabricColors = new List<FabricColor>();
            foreach (var fabricColor in fabricColors)
            {
                if (fabricColor.Color.Brightness >= hueColor.Brightness &&
                    fabricColor.Color.Saturation < hueColor.Saturation)
                {
                    selectedFabricColors.Add(fabricColor);
                }
            }
            selectedFabricColors.Sort((lhs, rhs) => lhs.Color.Brightness.CompareTo(rhs.Color.Brightness));
            //var d1 = Design.Primitives.Color.DistanceSquared(lhs, hueColor);
            //var d2 = Design.Primitives.Color.DistanceSquared(rhs, hueColor);
            //return d1.CompareTo(d2);

            var selectedColorModels = new List<XColor_Color>();
            foreach (var lighterColor in selectedFabricColors)
            {
                selectedColorModels.Add(CreateColorData(lighterColor));
            }

            return selectedColorModels.ToArray();
        }

        private XColor_Color[] CreateLighterSaturatedHues(Design.Primitives.Color hueColor, IList<FabricColor> fabricColors)
        {
            var selectedFabricColors = new List<FabricColor>();
            foreach (var fabricColor in fabricColors)
            {
                if (fabricColor.Color.Brightness >= hueColor.Brightness &&
                    fabricColor.Color.Saturation >= hueColor.Saturation)
                {
                    selectedFabricColors.Add(fabricColor);
                }
            }
            selectedFabricColors.Sort((lhs, rhs) => lhs.Color.Brightness.CompareTo(rhs.Color.Brightness));
            //var d1 = Design.Primitives.Color.DistanceSquared(lhs, hueColor);
            //var d2 = Design.Primitives.Color.DistanceSquared(rhs, hueColor);
            //return d1.CompareTo(d2);

            var selectedColorModels = new List<XColor_Color>();
            foreach (var saturatedColor in selectedFabricColors)
            {
                selectedColorModels.Add(CreateColorData(saturatedColor));
            }

            return selectedColorModels.ToArray();
        }

        private XColor_Color[,] CreatePalette(int hue, int rowCount, int columnCount, IList<FabricColor> fabricColors)
        {
            var paletteColors = new XColor_Color[rowCount, columnCount];

            var colorPalette = Design.Primitives.Color.CreateRectangleColorPalette(hue, rowCount, columnCount);

            for (var row = 0; row < rowCount; ++row)
            {
                for (var column = 0; column < columnCount; ++column)
                {
                    var paletteColor = colorPalette[row, column];

                    if (paletteColor != null)
                    {
                        var fabricColor = paletteColor.Closest(fabricColors, F => F.Color);

                        paletteColors[row, column] = CreateColorData(fabricColor);
                    }
                }
            }

            return paletteColors;
        }

        private XColor_Color[] CreateRelatedHues(Design.Primitives.Color hueColor, int hueDegrees, IList<FabricColor> fabricColors)
        {
            var selectedFabricColors = new List<FabricColor>();
            foreach (var color in fabricColors)
            {
                if (Math.Abs(color.Color.Hue - hueColor.Hue) < hueDegrees)
                {
                    selectedFabricColors.Add(color);
                }
            }
            selectedFabricColors.Sort((lhs, rhs) =>
            {
                var d1 = Design.Primitives.Color.DistanceSquared(lhs.Color, hueColor);
                var d2 = Design.Primitives.Color.DistanceSquared(rhs.Color, hueColor);
                return d1.CompareTo(d2);
            });

            var selectedColorModels = new List<XColor_Color>();
            foreach (var relatedHue in selectedFabricColors)
            {
                selectedColorModels.Add(CreateColorData(relatedHue));
            }

            return selectedColorModels.ToArray();
        }

        private async Task<List<FabricColor>> GetFabricColors(Design.Primitives.Color color)
        {
            var fabricColors = new List<FabricColor>();

            var inventoryItems = await InventoryService.GetItemsAsync().ConfigureAwait(false);
            foreach (var inventoryItem in inventoryItems)
            {
                var fabricColor = Design.Primitives.Color.FromWebColor(inventoryItem.Color.WebColor);
                var fabricHue = fabricColor.Hue;
                var hueDelta = Math.Abs(color.Hue - fabricHue);
                if (hueDelta < 8 || (360 - hueDelta) < 8)
                {
                    fabricColors.Add(CreateFabricColor(fabricColor, inventoryItem.Sku, inventoryItem.Name));
                }
            }

            return fabricColors;
        }

        private int RoundHue(int hue)
        {
            if (hue < 0)
            {
                hue += 360;
            }
            else if (hue >= 360)
            {
                hue -= 360;
            }

            return hue;
        }

        #region Private Classes

        private class FabricColor
        {

            public Design.Primitives.Color Color { get; set; }
            public string Name { get; set; }
            public string Sku { get; set; }

        }

        #endregion Private Classes
    }
}