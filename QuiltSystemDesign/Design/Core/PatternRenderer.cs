//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Drawing;

using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Core
{
    public class PatternRenderer
    {
        public Image CreateBitmap(Pattern pattern, int width, int height, DimensionScale scale)
        {
            var image = new Bitmap(width + 1, height + 1);
            using (Graphics graphics = Graphics.FromImage(image))
            {
                Render(graphics, pattern, width, height, scale);
            }

            return image;
        }

        public Image CreateBitmap(Pattern pattern, DimensionScale scale)
        {
            var width = (int)(pattern.FabricSize.Width * scale).Value;
            var height = (int)(pattern.FabricSize.Height * scale).Value;

            var image = new Bitmap(width + 1, height + 1);
            using (Graphics graphics = Graphics.FromImage(image))
            {
                Render(graphics, pattern, width, height, scale);
            }

            return image;
        }

        private void Render(Graphics graphics, Pattern pattern, int width, int height, DimensionScale scale)
        {
            graphics.FillRectangle(Brushes.LightGray, 0, 0, width, height);
            graphics.DrawRectangle(Pens.Black, 0, 0, width, height);

            using var stringFormat = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            foreach (var region in pattern.PatternElements)
            {
                var sourceX = (int)(region.Source.X * scale).Value;
                var sourceY = (int)(region.Source.Y * scale).Value;
                var targetX = (int)(region.Target.X * scale).Value;
                var targetY = (int)(region.Target.Y * scale).Value;

                var regionWidth = targetX - sourceX;
                var regionHeight = targetY - sourceY;

                graphics.FillRectangle(Brushes.White, sourceX, sourceY, regionWidth, regionHeight);
                graphics.DrawRectangle(Pens.Black, sourceX, sourceY, regionWidth, regionHeight);
                graphics.DrawString(region.Id, SystemFonts.DefaultFont, Brushes.Black, new RectangleF(sourceX, sourceY, regionWidth, regionHeight), stringFormat);
            }
        }
    }
}