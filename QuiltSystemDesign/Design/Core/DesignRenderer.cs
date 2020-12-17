//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;

using RichTodd.QuiltSystem.Design.Nodes;
using RichTodd.QuiltSystem.Design.Nodes.Standard;
using RichTodd.QuiltSystem.Design.Path;
using RichTodd.QuiltSystem.Design.Primitives;
using RichTodd.QuiltSystem.Properties;

namespace RichTodd.QuiltSystem.Design.Core
{
    public class DesignRenderer
    {
        private const int KIT_THUMBNAIL_BACKING_WIDTH = 10;
        private const int KIT_THUMBNAIL_BINDING_WIDTH = 5;
        private const int KIT_THUMBNAIL_PADDING = 10;
        private static readonly Image s_blankImage = new Bitmap(1, 1);

        public Image CreateBitmap(Design design, int size, bool enableTexture)
        {
            if (design.LayoutComponent == null)
            {
                return s_blankImage;
            }

            var designSize = design.GetStandardSizes().Where(r => r.Preferred).Single();

            // Determine scale that will resize design to the specified maximum pixel dimension.
            //
            var maxDimension = designSize.Width > designSize.Height ? designSize.Width : designSize.Height;
            var scale = new DimensionScale(maxDimension.Value, maxDimension.Unit, size - 1, DimensionUnits.Pixel);

            // Rescale design.
            //
            var pageLayoutNode = new PageLayoutNode(designSize.Width * scale, designSize.Height * scale);
            pageLayoutNode.LayoutSites[0].Node = design.LayoutComponent.Expand(true);
            pageLayoutNode.UpdateBounds(PathOrientation.CreateDefault(), scale);

            var image = CreateBitmap(pageLayoutNode, size, enableTexture);

            return image;
        }

        public Image CreateBitmap(Node node, DimensionScale scale, bool enableTexture)
        {
            // Determine scale that will resize design to the specified maximum pixel dimension.
            //
            var nodeBounds = node.Path.GetBounds();
            var nodeWidth = nodeBounds.MaximumX - nodeBounds.MinimumX;
            var nodeHeight = nodeBounds.MaximumY - nodeBounds.MinimumY;

            // Rescale design.
            //
            var pageLayoutNode = new PageLayoutNode(nodeWidth * scale, nodeHeight * scale);
            pageLayoutNode.LayoutSites[0].Node = node.Clone();
            pageLayoutNode.UpdateBounds(PathOrientation.CreateDefault(), scale);

            var image = CreateBitmap(pageLayoutNode, 0, enableTexture);

            return image;
        }

        public Image CreateBitmap(Kit kit, int size)
        {
            var height = size;
            var width = size;

            var bindingLeft = 0;
            if (kit.KitSpecification.BindingWidth.Value > 0)
            {
                width += KIT_THUMBNAIL_PADDING;
                bindingLeft = width;
                width += KIT_THUMBNAIL_BINDING_WIDTH;
            }

            var backingLeft = 0;
            if (kit.KitSpecification.HasBacking)
            {
                width += KIT_THUMBNAIL_PADDING;
                backingLeft = width;
                width += KIT_THUMBNAIL_BACKING_WIDTH;
            }

            var image = new Bitmap(width + 1, height + 1);
            using (var graphics = Graphics.FromImage(image))
            {
                RenderKit(graphics, kit, size - 1);

                if (kit.KitSpecification.BindingWidth.Value > 0)
                {
                    using Brush brush = new SolidBrush(ToDrawingColor(kit.KitSpecification.BindingFabricStyle.Color));

                    var points = new PointF[]
{
                            new PointF(bindingLeft, 0),
                            new PointF(bindingLeft + KIT_THUMBNAIL_BINDING_WIDTH - 1, 0),
                            new PointF(bindingLeft + KIT_THUMBNAIL_BINDING_WIDTH - 1, height - 1),
                            new PointF(bindingLeft, height - 1)
};
                    graphics.FillPolygon(brush, points);
                }

                if (kit.KitSpecification.HasBacking)
                {
                    using Brush brush = new SolidBrush(ToDrawingColor(kit.KitSpecification.BackingFabricStyle.Color));

                    var points = new PointF[]
{
                            new PointF(backingLeft, 0),
                            new PointF(backingLeft + KIT_THUMBNAIL_BACKING_WIDTH - 1, 0),
                            new PointF(backingLeft + KIT_THUMBNAIL_BACKING_WIDTH - 1, height - 1),
                            new PointF(backingLeft, height - 1)
};
                    graphics.FillPolygon(brush, points);
                }
            }

            return image;
        }

        private static PointF[] CreatePointArray(IPath path, Point origin)
        {
            var points = new PointF[path.SegmentCount];
            for (var idx = 0; idx < path.SegmentCount; ++idx)
            {
                var point = path.GetSegment(idx).Origin;
                points[idx] = new PointF((int)point.X.Value + origin.X, (int)point.Y.Value + origin.Y);
            }

            return points;
        }

#pragma warning disable IDE0051 // Remove unused private members
        private static PointF[] CreatePointArray(PathBounds viewBounds)
#pragma warning restore IDE0051 // Remove unused private members
        {
            var points = new PointF[4];
            points[0] = new PointF((float)viewBounds.MinimumX.Value, (float)viewBounds.MinimumY.Value);
            points[1] = new PointF((float)viewBounds.MaximumX.Value - 1, (float)viewBounds.MinimumY.Value);
            points[2] = new PointF((float)viewBounds.MaximumX.Value - 1, (float)viewBounds.MaximumY.Value - 1);
            points[3] = new PointF((float)viewBounds.MinimumX.Value, (float)viewBounds.MaximumY.Value - 1);

            return points;
        }

        private Bitmap CreateBitmap(Node node, int size, bool enableTexture)
        {
            // Create bitmap.
            //
            var viewBounds = node.Path.GetBounds();
            var designWidth = (int)(viewBounds.MaximumX - viewBounds.MinimumX).Value + 1;
            var designHeight = (int)(viewBounds.MaximumY - viewBounds.MinimumY).Value + 1;

            int imageWidth;
            int imageHeight;
            if (size == 0)
            {
                imageWidth = designWidth;
                imageHeight = designHeight;
            }
            else
            {
                imageWidth = size;
                imageHeight = size;
            }

            var origin = new Point((imageWidth - designWidth) / 2, (imageHeight - designHeight) / 2);

            var image = new Bitmap(imageWidth, imageHeight);

            using (var graphics = Graphics.FromImage(image))
            {
                graphics.Clear(System.Drawing.Color.FromArgb(240, 240, 240));

                var shapeNodes = new List<ShapeNode>();
                node.AddShapeNodesTo(shapeNodes);

                if (enableTexture)
                {
                    foreach (var shapeNode in shapeNodes)
                    {
                        RenderShapeNode(graphics, shapeNode, origin);
                    }

                    foreach (var shapeNode in shapeNodes)
                    {
                        RenderShapeNodeHighlight(graphics, shapeNode, origin);
                    }
                    graphics.ResetClip();

                    foreach (var shapeNode in shapeNodes)
                    {
                        RenderShapeNodeStitch(graphics, shapeNode, origin);
                    }

                    RenderTexture(graphics, imageWidth, imageHeight);
                }
                else
                {
                    foreach (var shapeNode in shapeNodes)
                    {
                        RenderShapeNode(graphics, shapeNode, origin);
                    }
                    foreach (var shapeNode in shapeNodes)
                    {
                        RenderShapeNodeStitch(graphics, shapeNode, origin);
                    }
                }
            }

            return image;
        }

        private void RenderKit(Graphics graphics, Kit kit, int size)
        {
            if (kit.Design.LayoutComponent != null)
            {
                var maxDimension = kit.KitSpecification.Width > kit.KitSpecification.Height ? kit.KitSpecification.Width : kit.KitSpecification.Height;
                var origin = new Point();

                var scale = new DimensionScale(maxDimension.Value, maxDimension.Unit, size, DimensionUnits.Pixel);

                var pageLayoutNode = new PageLayoutNode(kit.KitSpecification.Width * scale, kit.KitSpecification.Height * scale);
                pageLayoutNode.LayoutSites[0].Node = kit.Expand();
                pageLayoutNode.UpdateBounds(PathOrientation.CreateDefault(), scale);

                var shapeNodes = new List<ShapeNode>();
                pageLayoutNode.AddShapeNodesTo(shapeNodes);
                foreach (var shapeNode in shapeNodes)
                {
                    RenderShapeNode(graphics, shapeNode, origin);
                }
            }
        }

        private void RenderShapeNode(Graphics graphics, ShapeNode shapeNode, Point origin)
        {
            if (shapeNode == null) throw new ArgumentNullException(nameof(shapeNode));

            using var brush = new SolidBrush(ToDrawingColor(shapeNode.FabricStyle.Color));

            var points = CreatePointArray(shapeNode.Path, origin);
            graphics.FillPolygon(brush, points);
        }

        private void RenderShapeNodeHighlight(Graphics graphics, ShapeNode shapeNode, Point origin)
        {
            if (shapeNode == null) throw new ArgumentNullException(nameof(shapeNode));

            var shapeColor = ToDrawingColor(shapeNode.FabricStyle.Color);

            // Specify the angle of the light in radians.
            //
            var lightAngle = 135.0 * Math.PI / 180.0;
            var lightVector = new PointF(
                (float)(Math.Cos(lightAngle) * 100f),
                -(float)(Math.Sin(lightAngle) * 100f));

            var gradientWidth = 10.0;

            var points = CreatePointArray(shapeNode.Path, origin);

            var isClockwise = Geometry.Angle(points[0], points[1], points[2]) <= Math.PI;

            using var graphicsPath = new GraphicsPath();

            graphicsPath.StartFigure();
            graphicsPath.AddLines(points);
            graphicsPath.CloseFigure();

            graphics.SetClip(graphicsPath);

            for (var idx = 0; idx < points.Length; ++idx)
            {
                var point1 = points[idx];
                var point2 = points[(idx + 1) % points.Length];

                var pointLight = new PointF(point2.X + lightVector.X, point2.Y + lightVector.Y);
                var angle = Geometry.Angle(point1, point2, pointLight);

                var vector = Geometry.Normalize(new PointF(point2.X - point1.X, point2.Y - point1.Y));
                vector = new PointF((float)(vector.X * gradientWidth), (float)(vector.Y * gradientWidth));
                var gradientRight = new PointF(point1.X - vector.Y, point1.Y + vector.X);
                var gradientLeft = new PointF(point1.X + vector.Y, point1.Y - vector.X);

                // Convert to degrees
                //
                angle = angle * 180.0 / Math.PI;

                var A = 1.0;
                var P = 180.0;
                var intensityFactor = A / P * (P - Math.Abs(((angle + 90.0) % (2.0 * P)) - P));
                var intensityRight = intensityFactor;
                var intensityLeft = 1.0 - intensityRight;

                //Debug.WriteLine("angle = {0}, intensity = {1}", angle, intensity);

                Primitives.Color colorRight;
                Primitives.Color colorLeft;
                if (isClockwise)
                {
                    colorRight = Primitives.Color.FromAhsb(255, shapeColor.GetHue(), shapeColor.GetSaturation(), shapeColor.GetBrightness());
                    colorLeft = Primitives.Color.FromAhsb(255, 0, 0, intensityLeft);
                }
                else
                {
                    colorRight = Primitives.Color.Orange;
                    colorLeft = Primitives.Color.FromAhsb(255, shapeColor.GetHue(), shapeColor.GetSaturation(), shapeColor.GetBrightness());
                }

                using var brush =
                    new LinearGradientBrush(
                        gradientLeft,
                        gradientRight,
                        System.Drawing.Color.FromArgb(colorLeft.ToArgb(100)),
                        System.Drawing.Color.FromArgb(colorRight.ToArgb(100)));

                using var pen = new Pen(brush, ((float)gradientWidth * 2) - 2);

                pen.SetLineCap(LineCap.Triangle, LineCap.Triangle, DashCap.Triangle);

                graphics.DrawLine(pen, point1, point2);
            }
        }

        private void RenderShapeNodeStitch(Graphics graphics, ShapeNode shapeNode, Point origin)
        {
            if (shapeNode == null) throw new ArgumentNullException(nameof(shapeNode));

            var points = CreatePointArray(shapeNode.Path, origin);
            graphics.DrawPolygon(Pens.Black, points);
        }

        private void RenderTexture(Graphics graphics, int imageWidth, int imageHeight)
        {
            var colormatrix = new ColorMatrix
            {
                Matrix33 = 0.125f
            };

            var imgAttribute = new ImageAttributes();
            imgAttribute.SetColorMatrix(colormatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            imgAttribute.SetWrapMode(WrapMode.Tile);

            var img = Resources.FabricTexture1;

            using var brush = new TextureBrush(img, new Rectangle(0, 0, img.Width, img.Height), imgAttribute);

            graphics.FillRectangle(brush, new Rectangle(0, 0, imageWidth, imageHeight));
        }

        private System.Drawing.Color ToDrawingColor(Primitives.Color color)
        {
            return System.Drawing.Color.FromArgb(color.R, color.G, color.B);
        }
    }
}