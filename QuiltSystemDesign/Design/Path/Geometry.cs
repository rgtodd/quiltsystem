//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Drawing;

using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Path
{
    internal static class Geometry
    {
        public static double Angle(PointF from, PointF midpoint, PointF to)
        {
            return Angle(
                new PathPoint(
                    new Dimension(from.X, DimensionUnits.Pixel),
                    new Dimension(from.Y, DimensionUnits.Pixel)),
                new PathPoint(
                    new Dimension(midpoint.X, DimensionUnits.Pixel),
                    new Dimension(midpoint.Y, DimensionUnits.Pixel)),
                new PathPoint(
                    new Dimension(to.X, DimensionUnits.Pixel),
                    new Dimension(to.Y, DimensionUnits.Pixel)));
        }

        public static double Angle(PathPoint from, PathPoint midpoint, PathPoint to)
        {
            // Convert to standard cartesian coordinates.
            //
            from = new PathPoint(from.X, -from.Y);
            midpoint = new PathPoint(midpoint.X, -midpoint.Y);
            to = new PathPoint(to.X, -to.Y);

            var vector1 = new PathPoint(from.X - midpoint.X, from.Y - midpoint.Y);
            var vector2 = new PathPoint(to.X - midpoint.X, to.Y - midpoint.Y);

            var angle = Math.Atan2(vector2.Y.Value, vector2.X.Value) - Math.Atan2(vector1.Y.Value, vector1.X.Value);

            if (angle < 0) angle += 2.0 * Math.PI;

            return angle;
        }

        public static PathPoint Intersection(PathPoint line1From, PathPoint line1To, PathPoint line2From, PathPoint line2To)
        {
            var line1DeltaX = line1To.X.Value - line1From.X.Value;
            var line1DeltaY = line1To.Y.Value - line1From.Y.Value;
            var line2DeltaX = line2To.X.Value - line2From.X.Value;
            var line2DeltaY = line2To.Y.Value - line2From.Y.Value;
            var fromDeltaX = line2From.X.Value - line1From.X.Value;
            var fromDeltaY = line2From.Y.Value - line1From.Y.Value;

            var det = (line2DeltaX * line1DeltaY) - (line2DeltaY * line1DeltaX);
            if (Math.Abs(det) < 0.000001)
            {
                return PathPoint.Invalid;
            }

            var detinv = 1.0 / det;
            var s = ((line2DeltaX * fromDeltaY) - (line2DeltaY * fromDeltaX)) * detinv;
            var t = ((line1DeltaX * fromDeltaY) - (line1DeltaY * fromDeltaX)) * detinv;

            if (s < 0 || s > 1 || t < 0 || t > 1)
            {
                // return PathPoint.Invalid;
            }
            if (s < 0) s = 0;
            if (s > 1) s = 1;

            return new PathPoint(line1From.X + (line1DeltaX * s), line1From.Y + (line1DeltaY * s));
        }

        public static bool Intersects(PathPoint line1From, PathPoint line1To, PathPoint line2From, PathPoint line2To)
        {
            var line1DeltaX = line1To.X.Value - line1From.X.Value;
            var line1DeltaY = line1To.Y.Value - line1From.Y.Value;
            var line2DeltaX = line2To.X.Value - line2From.X.Value;
            var line2DeltaY = line2To.Y.Value - line2From.Y.Value;
            var fromDeltaX = line2From.X.Value - line1From.X.Value;
            var fromDeltaY = line2From.Y.Value - line1From.Y.Value;

            var det = (line2DeltaX * line1DeltaY) - (line2DeltaY * line1DeltaX);
            if (Math.Abs(det) < 0.000001)
            {
                return false;
            }

            var detinv = 1.0 / det;
            var s = ((line2DeltaX * fromDeltaY) - (line2DeltaY * fromDeltaX)) * detinv;
            var t = ((line1DeltaX * fromDeltaY) - (line1DeltaY * fromDeltaX)) * detinv;

            return s >= 0 && s <= 1 && t >= 0 && t <= 1;
        }

        public static double Length(PointF point)
        {
            return Math.Sqrt((point.X * point.X) + (point.Y * point.Y));
        }

        public static PointF Normalize(PointF point)
        {
            var length = Length(point);
            return new PointF((float)(point.X / length), (float)(point.Y / length));
        }
    }
}