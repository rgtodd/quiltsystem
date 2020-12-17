//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

using RichTodd.QuiltSystem.Design.Nodes;
using RichTodd.QuiltSystem.Design.Nodes.Standard;
using RichTodd.QuiltSystem.Design.Path;
using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Build
{
    internal abstract class BuildStep : IBuildStep
    {
        private static readonly Dimension s_seamAllowance = new Dimension(0.5, DimensionUnits.Inch);

        private readonly List<IBuildComponent> m_consumes;
        private readonly List<IBuildComponent> m_produces;
        private int m_stepNumber;

        public BuildStep()
        {
            m_consumes = new List<IBuildComponent>();
            m_produces = new List<IBuildComponent>();
        }

        public IReadOnlyList<IBuildComponent> Consumes
        {
            get
            {
                return m_consumes;
            }
        }

        public string Description
        {
            get
            {
                return GetType().Name;
            }
        }

        public IReadOnlyList<IBuildComponent> Produces
        {
            get
            {
                return m_produces;
            }
        }

        public int StepNumber
        {
            get
            {
                return m_stepNumber;
            }
            set
            {
                m_stepNumber = value;
            }
        }

        public void AddInput(IBuildComponent component)
        {
            if (component.ConsumedBy != null)
            {
                throw new InvalidOperationException("Component is already consumed by a build step.");
            }

            component.ConsumedBy = this;
            m_consumes.Add(component);
        }

        public void AddOutput(IBuildComponent component)
        {
            if (component.ProducedBy != null)
            {
                throw new InvalidOperationException("Component is already produced by a build step.");
            }

            if (CanProduceQuantity(component.StyleKey) == 0)
            {
                throw new InvalidOperationException("Build step does not produce desired style.");
            }

            component.ProducedBy = this;
            m_produces.Add(component);
        }

        public abstract int CanProduceQuantity(string styleKey);

        public abstract void ComputeInputs(BuildComponentFactory factory);

        protected void AddLayoutNodeInputs(BuildComponentFactory factory, LayoutNode layoutNode, bool trimTriangles, bool isTopLevel)
        {
            if (!isTopLevel && !string.IsNullOrEmpty(layoutNode.ComponentName) && !string.IsNullOrEmpty(layoutNode.ComponentType))
            {
                AddComponentInputs(factory, layoutNode, trimTriangles);
            }
            else if (layoutNode is GridLayoutNode)
            {
                AddGridInputs(factory, (GridLayoutNode)layoutNode, trimTriangles);
            }
            else if (layoutNode is HalfSquareTriangleLayoutNode)
            {
                AddHalfSquareTriangleInputs(factory, (HalfSquareTriangleLayoutNode)layoutNode, trimTriangles);
            }
            else
            {
                var children = new List<Node>();
                layoutNode.AddChildrenTo(children);

                foreach (var child in children)
                {
                    AddNodeInputs(factory, child, trimTriangles);
                }
            }
        }

        private void AddComponentInputs(BuildComponentFactory factory, LayoutNode layoutNode, bool trimTriangles)
        {
            var matchingComponent = FindLayout(BuildComponentLayout.CreateStyleKey(layoutNode.ComponentType, layoutNode.ComponentName, layoutNode.GetFabricStyles()));
            if (matchingComponent != null)
            {
                matchingComponent.Quantity += 1;
            }
            else
            {
                var component = factory.CreateBuildComponentLayout(layoutNode, trimTriangles);
                AddInput(component);
            }
        }

        private void AddFlyingGooseInputs(BuildComponentFactory factory, FabricStyle fabricStyleBody, FabricStyle fabricStyleCorner, Area area, bool trimTriangles)
        {
            var matchingComponent = FindFlyingGoose(BuildComponentFlyingGoose.CreateStyleKey(fabricStyleBody, fabricStyleCorner, area));
            if (matchingComponent != null)
            {
                matchingComponent.Quantity += 1;
            }
            else
            {
                var component = factory.CreateBuildComponentFlyingGooose(fabricStyleBody, fabricStyleCorner, area, trimTriangles);
                AddInput(component);
            }
        }

        private void AddGridInputs(BuildComponentFactory factory, GridLayoutNode grid, bool trimTriangles)
        {
            List<Tuple<int, int>> consumedNodes = new List<Tuple<int, int>>();

            for (int row = 0; row < grid.RowCount; ++row)
            {
                for (int column = 0; column < grid.ColumnCount; ++column)
                {
                    if (!consumedNodes.Exists(r => r.Item1 == row && r.Item2 == column))
                    {
                        var layoutSite = grid.GetLayoutSite(row, column);
                        var columnSpan = grid.GetColumnSpan(row, column);
                        var rowSpan = grid.GetRowSpan(row, column);

                        var fingerprint = GetFlyingGooseFingerprint(layoutSite.Node, layoutSite.PathOrientation.PointOffset);
                        if (fingerprint != null)
                        {
                            bool flyingGooseCreated = false;

                            LayoutSite layoutSiteRight = null; // Assume layout site does not exist.
                            if (column + columnSpan < grid.ColumnCount)
                            {
                                if (grid.GetLayoutSite(row, column + columnSpan).Node != null)
                                {
                                    if (grid.GetColumnSpan(row, column + columnSpan) == columnSpan &&
                                        grid.GetRowSpan(row, column + columnSpan) == rowSpan)
                                    {
                                        layoutSiteRight = grid.GetLayoutSite(row, column + columnSpan);
                                    }
                                }
                            }

                            if (layoutSiteRight != null)
                            {
                                var matchingRightFingerprint = GetFlyingGooseMatchingHorizontalFingerprint(layoutSite.Node, layoutSite.PathOrientation.PointOffset);

                                var rightFingerprint = GetFlyingGooseFingerprint(layoutSiteRight.Node, layoutSiteRight.PathOrientation.PointOffset);

                                if (rightFingerprint == matchingRightFingerprint)
                                {
                                    var halfSquareTriangle = (HalfSquareTriangleLayoutNode)layoutSite.Node;

                                    FabricStyle fabricStyleBody;
                                    FabricStyle fabricStyleCorner;
                                    switch (layoutSite.PathOrientation.PointOffset)
                                    {
                                        case 0:
                                        case 1:
                                            fabricStyleCorner = ((ShapeNode)halfSquareTriangle.LayoutSites[0].Node).FabricStyle;
                                            fabricStyleBody = ((ShapeNode)halfSquareTriangle.LayoutSites[1].Node).FabricStyle;
                                            break;

                                        case 2:
                                        case 3:
                                            fabricStyleCorner = ((ShapeNode)halfSquareTriangle.LayoutSites[1].Node).FabricStyle;
                                            fabricStyleBody = ((ShapeNode)halfSquareTriangle.LayoutSites[0].Node).FabricStyle;
                                            break;

                                        default:
                                            throw new InvalidOperationException("Unexpected point offset.");
                                    }

                                    var nodeBounds = halfSquareTriangle.Path.GetBounds();
                                    var width = nodeBounds.MaximumX - nodeBounds.MinimumX;
                                    var height = nodeBounds.MaximumY - nodeBounds.MinimumY;
                                    var area = new Area(width * 2, height).Round();

                                    AddFlyingGooseInputs(factory, fabricStyleBody, fabricStyleCorner, area, trimTriangles);

                                    consumedNodes.Add(new Tuple<int, int>(row, column));
                                    consumedNodes.Add(new Tuple<int, int>(row, column + columnSpan));

                                    flyingGooseCreated = true;
                                }
                            }

                            LayoutSite layoutSiteDown = null; // Assume layout site does not exist.
                            if (row + rowSpan < grid.RowCount)
                            {
                                if (grid.GetLayoutSite(row + rowSpan, column).Node != null)
                                {
                                    if (grid.GetColumnSpan(row + rowSpan, column) == columnSpan &&
                                        grid.GetRowSpan(row + rowSpan, column) == rowSpan)
                                    {
                                        layoutSiteDown = grid.GetLayoutSite(row + rowSpan, column);
                                    }
                                }
                            }

                            if (!flyingGooseCreated && layoutSiteDown != null)
                            {
                                var matchingDownFingerprint = GetFlyingGooseMatchingVerticalFingerprint(layoutSite.Node, layoutSite.PathOrientation.PointOffset);

                                var downFingerprint = GetFlyingGooseFingerprint(layoutSiteDown.Node, layoutSiteDown.PathOrientation.PointOffset);

                                if (downFingerprint == matchingDownFingerprint)
                                {
                                    var halfSquareTriangle = (HalfSquareTriangleLayoutNode)layoutSite.Node;

                                    FabricStyle fabricStyleBody;
                                    FabricStyle fabricStyleCorner;
                                    switch (layoutSite.PathOrientation.PointOffset)
                                    {
                                        case 0:
                                        case 3:
                                            fabricStyleCorner = ((ShapeNode)halfSquareTriangle.LayoutSites[0].Node).FabricStyle;
                                            fabricStyleBody = ((ShapeNode)halfSquareTriangle.LayoutSites[1].Node).FabricStyle;
                                            break;

                                        case 1:
                                        case 2:
                                            fabricStyleCorner = ((ShapeNode)halfSquareTriangle.LayoutSites[1].Node).FabricStyle;
                                            fabricStyleBody = ((ShapeNode)halfSquareTriangle.LayoutSites[0].Node).FabricStyle;
                                            break;

                                        default:
                                            throw new InvalidOperationException("Unexpected point offset.");
                                    }

                                    var nodeBounds = halfSquareTriangle.Path.GetBounds();
                                    var width = nodeBounds.MaximumX - nodeBounds.MinimumX;
                                    var height = nodeBounds.MaximumY - nodeBounds.MinimumY;
                                    var area = new Area(width * 2, height).Round();

                                    AddFlyingGooseInputs(factory, fabricStyleBody, fabricStyleCorner, area, trimTriangles);

                                    consumedNodes.Add(new Tuple<int, int>(row, column));
                                    consumedNodes.Add(new Tuple<int, int>(row + rowSpan, column));
                                }
                            }
                        }
                    }
                }
            }

            for (int row = 0; row < grid.RowCount; ++row)
            {
                for (int column = 0; column < grid.ColumnCount; ++column)
                {
                    if (!consumedNodes.Exists(r => r.Item1 == row && r.Item2 == column))
                    {
                        var child = grid.GetLayoutSite(row, column).Node;
                        if (child != null)
                        {
                            AddNodeInputs(factory, child, trimTriangles);
                        }
                    }
                }
            }
        }

        private void AddHalfSquareTriangleInputs(BuildComponentFactory factory, HalfSquareTriangleLayoutNode layoutNode, bool trimTriangles)
        {
            var style1 = ((ShapeNode)layoutNode.LayoutSites[0].Node).FabricStyle;
            var style2 = ((ShapeNode)layoutNode.LayoutSites[1].Node).FabricStyle;

            var nodeBounds = layoutNode.Path.GetBounds();
            var width = nodeBounds.MaximumX - nodeBounds.MinimumX;
            var height = nodeBounds.MaximumY - nodeBounds.MinimumY;
            var area = new Area(width, height).Round();

            var matchingComponent = FindHalfSquareTriangle(BuildComponentHalfSquareTriangle.CreateStyleKey(style1, style2, area));
            if (matchingComponent != null)
            {
                matchingComponent.Quantity += 1;
            }
            else
            {
                var component = factory.CreateBuildComponentHalfSquareTriangle(layoutNode, trimTriangles);
                AddInput(component);
            }
        }

        private void AddNodeInputs(BuildComponentFactory factory, Node node, bool trimTriangles)
        {
            if (node is LayoutNode)
            {
                AddLayoutNodeInputs(factory, (LayoutNode)node, trimTriangles, false);
            }
            else if (node is ShapeNode)
            {
                AddShapeNodeInput(factory, (ShapeNode)node);
            }
            else
            {
                throw new InvalidOperationException("Unknown node type.");
            }
        }

        private void AddRectangleInput(BuildComponentFactory factory, Area area, FabricStyle fabricStyle)
        {
            var matchingComponent = FindRectangle(BuildComponentRectangle.CreateStyleKey(fabricStyle, area));
            if (matchingComponent != null)
            {
                matchingComponent.Quantity += 1;
            }
            else
            {
                var component = factory.CreateBuildComponentRectangle(fabricStyle, area);
                AddInput(component);
            }
        }

        private void AddShapeNodeInput(BuildComponentFactory factory, ShapeNode shapeNode)
        {
            if (shapeNode.Path.PathGeometry != PathGeometries.Rectangle)
            {
                throw new InvalidOperationException("Unsupported path geometry.");
            }

            var nodeBounds = shapeNode.Path.GetBounds();
            var width = nodeBounds.MaximumX - nodeBounds.MinimumX;
            var height = nodeBounds.MaximumY - nodeBounds.MinimumY;

            width += s_seamAllowance;
            height += s_seamAllowance;

            var area = new Area(width, height).Round();

            AddRectangleInput(factory, area, shapeNode.FabricStyle);
        }

        private BuildComponentFlyingGoose FindFlyingGoose(string styleKey)
        {
            foreach (var component in Consumes)
            {
                if (component.StyleKey == styleKey)
                {
                    return (BuildComponentFlyingGoose)component;
                }
            }

            return null;
        }

        private BuildComponentHalfSquareTriangle FindHalfSquareTriangle(string styleKey)
        {
            foreach (var component in Consumes)
            {
                if (component.StyleKey == styleKey)
                {
                    return (BuildComponentHalfSquareTriangle)component;
                }
            }

            return null;
        }

        private BuildComponentLayout FindLayout(string styleKey)
        {
            foreach (var component in Consumes)
            {
                if (component.StyleKey == styleKey)
                {
                    return (BuildComponentLayout)component;
                }
            }

            return null;
        }

        private BuildComponentRectangle FindRectangle(string styleKey)
        {
            foreach (var component in Consumes)
            {
                if (component.StyleKey == styleKey)
                {
                    return (BuildComponentRectangle)component;
                }
            }

            return null;
        }

        private string GetFlyingGooseFingerprint(Node node, int pointOffset)
        {
            if (node == null)
            {
                return null;
            }

            if (!(node is HalfSquareTriangleLayoutNode halfSquareTriangleNode))
            {
                return null;
            }

            ShapeNode shapeNode1 = halfSquareTriangleNode.LayoutSites[0].Node as ShapeNode;
            ShapeNode shapeNode2 = halfSquareTriangleNode.LayoutSites[1].Node as ShapeNode;

            return pointOffset switch
            {
                0 => shapeNode1.FabricStyle.Sku + @"/" + shapeNode2.FabricStyle.Sku,
                1 => shapeNode1.FabricStyle.Sku + @"\" + shapeNode2.FabricStyle.Sku,
                2 => shapeNode2.FabricStyle.Sku + @"/" + shapeNode1.FabricStyle.Sku,
                3 => shapeNode2.FabricStyle.Sku + @"\" + shapeNode1.FabricStyle.Sku,
                _ => throw new InvalidOperationException("Unexpected point offset."),
            };
        }

        private string GetFlyingGooseMatchingHorizontalFingerprint(Node node, int pointOffset)
        {
            if (node == null)
            {
                return null;
            }

            if (!(node is HalfSquareTriangleLayoutNode halfSquareTriangleNode))
            {
                return null;
            }

            ShapeNode shapeNode1 = halfSquareTriangleNode.LayoutSites[0].Node as ShapeNode;
            ShapeNode shapeNode2 = halfSquareTriangleNode.LayoutSites[1].Node as ShapeNode;

            return pointOffset switch
            {
                0 => shapeNode2.FabricStyle.Sku + @"\" + shapeNode1.FabricStyle.Sku,
                1 => shapeNode2.FabricStyle.Sku + @"/" + shapeNode1.FabricStyle.Sku,
                2 => shapeNode1.FabricStyle.Sku + @"\" + shapeNode2.FabricStyle.Sku,
                3 => shapeNode1.FabricStyle.Sku + @"/" + shapeNode2.FabricStyle.Sku,
                _ => throw new InvalidOperationException("Unexpected point offset."),
            };
        }

        private string GetFlyingGooseMatchingVerticalFingerprint(Node node, int pointOffset)
        {
            if (node == null)
            {
                return null;
            }

            if (!(node is HalfSquareTriangleLayoutNode halfSquareTriangleNode))
            {
                return null;
            }

            ShapeNode shapeNode1 = halfSquareTriangleNode.LayoutSites[0].Node as ShapeNode;
            ShapeNode shapeNode2 = halfSquareTriangleNode.LayoutSites[1].Node as ShapeNode;

            return pointOffset switch
            {
                0 => shapeNode1.FabricStyle.Sku + @"\" + shapeNode2.FabricStyle.Sku,
                1 => shapeNode1.FabricStyle.Sku + @"/" + shapeNode2.FabricStyle.Sku,
                2 => shapeNode2.FabricStyle.Sku + @"\" + shapeNode1.FabricStyle.Sku,
                3 => shapeNode2.FabricStyle.Sku + @"/" + shapeNode1.FabricStyle.Sku,
                _ => throw new InvalidOperationException("Unexpected point offset."),
            };
        }

#pragma warning disable IDE0051 // Remove unused private members
        private bool IsEqual(FabricStyleList lhs, FabricStyleList rhs)
#pragma warning restore IDE0051 // Remove unused private members
        {
            if (lhs == null && rhs == null) return true;

            if (lhs == null || rhs == null) return false;

            if (lhs.Count != rhs.Count) return false;

            for (int idx = 0; idx < lhs.Count; ++idx)
            {
                if (lhs[idx].Sku != rhs[idx].Sku)
                {
                    return false;
                }
            }

            return true;
        }

#pragma warning disable IDE0051 // Remove unused private members
        private bool IsTriangleNode(Node node)
#pragma warning restore IDE0051 // Remove unused private members
        {
            return
                node != null
                    ? node is ShapeNode shapeNode
                        ? shapeNode.Path.PathGeometry == PathGeometries.Triangle
                        : false
                    : false;
        }
    }
}