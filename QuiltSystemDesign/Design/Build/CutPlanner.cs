//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Build
{
    internal static class CutPlanner
    {
        public static CutPlan Plan(IReadOnlyCollection<ICutShape> shapes)
        {
            var unlimitedCutStockFactory = new UnlimitedCutStockFactory();
            var cutPlan = Plan(shapes, unlimitedCutStockFactory);
            if (cutPlan == null)
            {
                return null;
            }

            var cutStocks = Simplify(cutPlan.CutStocks);
            while (cutStocks != null)
            {
                var cutStockFactory = new CutStockFactory(cutStocks);
                var simplifiedCutPlan = Plan(shapes, cutStockFactory);
                if (simplifiedCutPlan == null)
                {
                    return cutPlan;
                }

                cutPlan = simplifiedCutPlan;
                cutStocks = Simplify(cutPlan.CutStocks);
            }

            return cutPlan;
        }

        private static ICutShape GetLargestShape(ICollection<ICutShape> shapes)
        {
            Debug.Assert(shapes.Count > 0);

            ICutShape largestShape = null;
            Dimension largestShapeDimension = new Dimension(0, DimensionUnits.Inch);

            foreach (var shape in shapes)
            {
                var shapeDimension = GetNormalizedShapeDimension(shape).Item1; // First dimension is always largest.
                if (shapeDimension > largestShapeDimension)
                {
                    largestShape = shape;
                    largestShapeDimension = shapeDimension;
                }
            }

            return largestShape;
        }

#pragma warning disable IDE0051 // Remove unused private members
        private static Tuple<Dimension, Dimension> GetNormalizedRegionDimension(CutRegion region)
#pragma warning restore IDE0051 // Remove unused private members
        {
            if (region.Width >= region.Height)
            {
                return new Tuple<Dimension, Dimension>(region.Width, region.Height);
            }
            else
            {
                return new Tuple<Dimension, Dimension>(region.Height, region.Width);
            }
        }

        private static Tuple<Dimension, Dimension> GetNormalizedShapeDimension(ICutShape shape)
        {
            var shapeArea = shape.Area;
            if (shapeArea.Width >= shapeArea.Height)
            {
                return new Tuple<Dimension, Dimension>(shapeArea.Width, shapeArea.Height);
            }
            else
            {
                return new Tuple<Dimension, Dimension>(shapeArea.Height, shapeArea.Width);
            }
        }

        private static CutRegion GetSmallestContainingCutRegion(ICollection<CutRegion> cutRegions, ICutShape cutShape)
        {
            CutRegion currentCutRegion = null;

            var dimension = GetNormalizedShapeDimension(cutShape);

            foreach (var cutRegion in cutRegions)
            {
                if (cutRegion.Contains(dimension.Item1, dimension.Item2))
                {
                    if (currentCutRegion == null || cutRegion.Area < currentCutRegion.Area)
                    {
                        currentCutRegion = cutRegion;
                    }
                }
            }

            return currentCutRegion;
        }

        private static CutPlan Plan(IReadOnlyCollection<ICutShape> shapes, ICutStockFactory cutStockFactory)
        {
            CutPlan cutPlan = new CutPlan();

            IList<CutRegion> unusedCutRegions = new List<CutRegion>();

            var pendingShapes = new List<ICutShape>(shapes);
            while (pendingShapes.Count > 0)
            {
                var cutShape = GetLargestShape(pendingShapes);
                Debug.Assert(cutShape != null);
                pendingShapes.Remove(cutShape);

                var cutShapeArea = cutShape.Area;

                var cutRegion = GetSmallestContainingCutRegion(unusedCutRegions, cutShape);
                if (cutRegion == null)
                {
                    var cutStock = cutStockFactory.GetCutStock(cutShapeArea);
                    if (cutStock == null)
                    {
                        // Insufficient amount of stock material.  Cannot create build plan.
                        //
                        return null;
                    }

                    cutPlan.CutStocks.Add(cutStock);

                    unusedCutRegions.Add(
                        new CutRegion(
                            cutStock,
                            new Dimension(0, cutStock.Area.Height.Unit),
                            new Dimension(0, cutStock.Area.Width.Unit),
                            cutStock.Area.Width,
                            cutStock.Area.Height));

                    cutRegion = GetSmallestContainingCutRegion(unusedCutRegions, cutShape);
                    Debug.Assert(cutRegion != null);
                }
                unusedCutRegions.Remove(cutRegion);

                CutRegion planCutRegion;

                if (cutShapeArea.Width == cutRegion.Width && cutShapeArea.Height == cutRegion.Height)
                {
                    planCutRegion = cutRegion;
                }
                else if (cutShapeArea.Width == cutRegion.Height && cutShapeArea.Height == cutRegion.Width)
                {
                    planCutRegion = cutRegion;
                }
                else if (cutShapeArea.Width == cutRegion.Width && cutShapeArea.Height < cutRegion.Height)
                {
                    var subregions = cutRegion.CutHorizontal(cutShapeArea.Height);

                    planCutRegion = subregions.Item1;
                    unusedCutRegions.Add(subregions.Item2);
                }
                else if (cutShapeArea.Height == cutRegion.Width && cutShapeArea.Width < cutRegion.Height)
                {
                    var subregions = cutRegion.CutHorizontal(cutShapeArea.Width);

                    planCutRegion = subregions.Item1;
                    unusedCutRegions.Add(subregions.Item2);
                }
                else if (cutShapeArea.Height == cutRegion.Height && cutShapeArea.Width < cutRegion.Width)
                {
                    var subregions = cutRegion.CutVertical(cutShapeArea.Width);

                    planCutRegion = subregions.Item1;
                    unusedCutRegions.Add(subregions.Item2);
                }
                else if (cutShapeArea.Width == cutRegion.Height && cutShapeArea.Height < cutRegion.Width)
                {
                    var subregions = cutRegion.CutVertical(cutShapeArea.Height);

                    planCutRegion = subregions.Item1;
                    unusedCutRegions.Add(subregions.Item2);
                }
                else if (cutShapeArea.Width < cutRegion.Width && cutShapeArea.Height < cutRegion.Height)
                {
                    var subregions1 = cutRegion.CutVertical(cutShapeArea.Width);
                    var subregions2 = subregions1.Item1.CutHorizontal(cutShapeArea.Height);

                    planCutRegion = subregions2.Item1;
                    unusedCutRegions.Add(subregions1.Item2);
                    unusedCutRegions.Add(subregions2.Item2);
                }
                else if (cutShapeArea.Height < cutRegion.Width && cutShapeArea.Width < cutRegion.Height)
                {
                    var subregions1 = cutRegion.CutVertical(cutShapeArea.Height);
                    var subregions2 = subregions1.Item1.CutHorizontal(cutShapeArea.Width);

                    planCutRegion = subregions2.Item1;
                    unusedCutRegions.Add(subregions1.Item2);
                    unusedCutRegions.Add(subregions2.Item2);
                }
                else
                {
                    throw new InvalidOperationException("Cannot cut region to fit shape.");
                }

                planCutRegion.CutShape = cutShape;

                cutPlan.CutRegions.Add(planCutRegion);
            }

            return cutPlan;
        }

        private static List<CutStock> Simplify(List<CutStock> cutStocks)
        {
            if (cutStocks.Count(r => r.AreaSize == AreaSizes.HalfYard) >= 2)
            {
                var result = new List<CutStock>(cutStocks);

                var cutStock = result.Where(r => r.AreaSize == AreaSizes.HalfYard).First();
                result.Remove(cutStock);
                cutStock = result.Where(r => r.AreaSize == AreaSizes.HalfYard).First();
                result.Remove(cutStock);

                result.Add(new CutStock(AreaSizes.Yard));

                return result;
            }

            if (cutStocks.Count(r => r.AreaSize == AreaSizes.FatQuarter) >= 2)
            {
                var result = new List<CutStock>(cutStocks);

                var cutStock = result.Where(r => r.AreaSize == AreaSizes.FatQuarter).First();
                result.Remove(cutStock);
                cutStock = result.Where(r => r.AreaSize == AreaSizes.FatQuarter).First();
                result.Remove(cutStock);

                result.Add(new CutStock(AreaSizes.HalfYard));

                return result;
            }

            return null;
        }
    }
}