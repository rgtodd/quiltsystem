//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using RichTodd.QuiltSystem.Design.Path;
using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Nodes.Standard
{
    [Node(PathGeometryNames.RECTANGLE)]
    public class BorderLayoutNode : LayoutNode
    {
        private readonly List<ShapeNode> m_borderShapes = new List<ShapeNode>();
        private FabricStyle m_fabricStyle;
        private readonly LayoutSiteList m_layoutSites;
        private Dimension m_width;

        public BorderLayoutNode(FabricStyle fabricStyle, Dimension width) : base(PathGeometries.Rectangle)
        {
            if (width < new Dimension(0, width.Unit)) throw new ArgumentOutOfRangeException(nameof(width));

            m_fabricStyle = fabricStyle ?? throw new ArgumentNullException(nameof(fabricStyle));
            m_width = width;
            m_layoutSites = new LayoutSiteList(this)
            {
                new LayoutSite(this, PathGeometries.Rectangle)
            };
        }

        public BorderLayoutNode(JToken json) : base(json)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));

            var jsonFabricStyle = json[JsonNames.FabricStyle];
            m_fabricStyle = jsonFabricStyle != null
                ? new FabricStyle(jsonFabricStyle)
                : FabricStyle.Default;

            m_width = Dimension.Parse((string)json[JsonNames.Width]);
            m_layoutSites = new LayoutSiteList(this, json[JsonNames.LayoutSites]);
        }

        protected BorderLayoutNode(BorderLayoutNode prototype) : base(prototype)
        {
            if (prototype == null) throw new ArgumentNullException(nameof(prototype));

            m_fabricStyle = prototype.m_fabricStyle.Clone();
            m_width = prototype.m_width;
            m_layoutSites = prototype.m_layoutSites.Clone(this);

            m_borderShapes.Clear();
            foreach (var borderShape in prototype.m_borderShapes)
            {
                m_borderShapes.Add((ShapeNode)borderShape.Clone());
            }
        }

        public FabricStyle FabricStyle
        {
            get
            {
                return m_fabricStyle;
            }

            set
            {
                m_fabricStyle = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        public override IReadOnlyList<LayoutSite> LayoutSites
        {
            get
            {
                return m_layoutSites;
            }
        }

        public Dimension Width
        {
            get
            {
                return m_width;
            }

            set
            {
                if (value.Value < 0) throw new ArgumentOutOfRangeException(nameof(value));

                m_width = value;
            }
        }

        public override Node Clone()
        {
            return new BorderLayoutNode(this);
        }

        public override JToken JsonSave()
        {
            var result = base.JsonSave();

            result[JsonNames.FabricStyle] = m_fabricStyle.JsonSave();
            result[JsonNames.Width] = m_width.ToString();
            result[JsonNames.LayoutSites] = m_layoutSites.JsonSave();

            return result;
        }

        public override void UpdatePath(IPath path, PathOrientation pathOrientation, DimensionScale scale)
        {
            base.UpdatePath(path, pathOrientation, scale);

            var scaledWidth = (m_width * scale).Value;

            // Outer points
            //
            var p1 = path.Offset(0, scaledWidth);
            var p2 = path.Offset(0, -scaledWidth);
            var p3 = path.Offset(1, scaledWidth);
            var p4 = path.Offset(1, -scaledWidth);
            var p5 = path.Offset(2, scaledWidth);
            var p6 = path.Offset(2, -scaledWidth);
            var p7 = path.Offset(3, scaledWidth);
            var p8 = path.Offset(3, -scaledWidth);

            // Inner points
            //
            var q1 = new PathPoint(p1.X, p3.Y);
            var q2 = new PathPoint(p2.X, p3.Y);
            var q3 = new PathPoint(p5.X, p4.Y);
            var q4 = new PathPoint(p6.X, p4.Y);

            // Resize layout site.
            //
            var pathInner = PathGeometries.Rectangle.CreatePath(
                            new PathPoint[] { q1, q2, q3, q4 });
            LayoutSites[0].UpdatePath(pathInner, scale);

            // Recompute border shapes.
            //
            var borderTop = PathGeometries.Rectangle.CreatePath(
                new PathPoint[] {
                    path.GetSegment(0).Origin,
                    path.GetSegment(1).Origin,
                    p3,
                    p8 });

            var borderRight = PathGeometries.Rectangle.CreatePath(
                new PathPoint[] {
                    p3,
                    p4,
                    q3,
                    q2 });

            var borderBottom = PathGeometries.Rectangle.CreatePath(
                new PathPoint[] {
                    path.GetSegment(2).Origin,
                    path.GetSegment(3).Origin,
                    p7,
                    p4 });

            var borderLeft = PathGeometries.Rectangle.CreatePath(
                new PathPoint[] {
                    p7,
                    p8,
                    q1,
                    q4 });

            var borderPaths = new List<IPath>();
            var maxBorderPieceLength = new Dimension(20, DimensionUnits.Inch) * scale;
            borderPaths.AddRange(SplitPath(borderTop, maxBorderPieceLength));
            borderPaths.AddRange(SplitPath(borderRight, maxBorderPieceLength));
            borderPaths.AddRange(SplitPath(borderBottom, maxBorderPieceLength));
            borderPaths.AddRange(SplitPath(borderLeft, maxBorderPieceLength));

            m_borderShapes.Clear();
            foreach (var borderPath in borderPaths)
            {
                var shape = new RectangleShapeNode(m_fabricStyle);
                shape.UpdatePath(borderPath, pathOrientation, scale);
                m_borderShapes.Add(shape);
            }
        }

        internal override void AddChildrenTo(List<Node> nodes)
        {
            base.AddChildrenTo(nodes);

            foreach (var node in m_borderShapes)
            {
                nodes.Add(node);
            }
        }

        internal override void AddShapeNodesTo(List<ShapeNode> shapeNodes)
        {
            base.AddShapeNodesTo(shapeNodes);

            foreach (var borderShape in m_borderShapes)
            {
                shapeNodes.Add(borderShape);
            }
        }

        private List<IPath> SplitPath(IPath pathX, Dimension maxLength)
        {
            var pathStack = new Stack<IPath>();
            var result = new List<IPath>();

            pathStack.Push(pathX);

            while (pathStack.Count > 0)
            {
                var path = pathStack.Pop();

                // Find longest length in path.
                //
                var curMaxLength = new Dimension(double.MinValue, maxLength.Unit);
                var curIdx = -1;
                for (var idx = 0; idx < path.SegmentCount; ++idx)
                {
                    var length = path.GetLength(idx);
                    if (length > curMaxLength)
                    {
                        curIdx = idx;
                        curMaxLength = length;
                    }
                }

                if (curMaxLength > maxLength)
                {
                    var p1 = path.Offset(curIdx, maxLength.Value);
                    var p2 = path.Offset(curIdx + 2, -maxLength.Value);

                    var path1 = PathGeometries.Rectangle.CreatePath(
                                new PathPoint[] { path.GetSegment(curIdx + 0).Origin, p1, p2, path.GetSegment(curIdx + 3).Origin });

                    var path2 = PathGeometries.Rectangle.CreatePath(
                                new PathPoint[] { p1, path.GetSegment(curIdx + 1).Origin, path.GetSegment(curIdx + 2).Origin, p2 });

                    result.Add(path1);
                    pathStack.Push(path2);
                }
                else
                {
                    result.Add(path);
                }
            }

            return result;
        }
    }
}