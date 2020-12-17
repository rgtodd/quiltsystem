//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Text;

using RichTodd.QuiltSystem.Design.Nodes;
using RichTodd.QuiltSystem.Design.Nodes.Standard;
using RichTodd.QuiltSystem.Design.Path;
using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Build
{
    internal class BuildComponentFlyingGoose : BuildComponent
    {
        private readonly Area m_area;
        private readonly FabricStyle[] m_fabricStyles;
        private readonly GridLayoutNode m_gridLayoutNode;
        private readonly string m_styleKey;
        private readonly bool m_trim;

        public BuildComponentFlyingGoose(string id, FabricStyle fabricStyleBody, FabricStyle fabricStyleCorner, Area area, bool trim)
            : base(id)
        {
            if (fabricStyleBody == null) throw new ArgumentNullException(nameof(fabricStyleBody));
            if (fabricStyleCorner == null) throw new ArgumentNullException(nameof(fabricStyleCorner));
            if (area == null) throw new ArgumentNullException(nameof(area));

            if (area.Width < area.Height)
            {
                area = new Area(area.Height, area.Width);
            }

            m_area = area;

            m_fabricStyles = new FabricStyle[] { fabricStyleBody, fabricStyleCorner };

            m_trim = trim;

            m_styleKey = CreateStyleKey(fabricStyleBody, fabricStyleCorner, area);

            var halfSquareTriangleNode1 = new HalfSquareTriangleLayoutNode();
            halfSquareTriangleNode1.LayoutSites[0].Node = new TriangleShapeNode(fabricStyleCorner);
            halfSquareTriangleNode1.LayoutSites[1].Node = new TriangleShapeNode(fabricStyleBody);

            var halfSquareTriangleNode2 = new HalfSquareTriangleLayoutNode();
            halfSquareTriangleNode2.LayoutSites[0].Node = new TriangleShapeNode(fabricStyleCorner);
            halfSquareTriangleNode2.LayoutSites[1].Node = new TriangleShapeNode(fabricStyleBody);

            var gridLayoutNode = new GridLayoutNode(1, 2);
            gridLayoutNode.GetLayoutSite(0, 0).Node = halfSquareTriangleNode1;
            gridLayoutNode.GetLayoutSite(0, 1).Node = halfSquareTriangleNode2;
            gridLayoutNode.GetLayoutSite(0, 1).PathOrientation.PointOffset = 3;

            var scale = DimensionScale.CreateIdentity(area.Width.Unit);
            gridLayoutNode.UpdatePath(PathGeometries.Rectangle.CreatePath(area.Width, area.Height), PathOrientation.CreateDefault(), scale);

            m_gridLayoutNode = gridLayoutNode;
        }

        public override Area Area
        {
            get
            {
                return m_area;
            }
        }

        public override string ComponentSubtype
        {
            get
            {
                return "Flying Goose";
            }
        }

        public override string ComponentType
        {
            get
            {
                return BuildComponentTypes.Component;
            }
        }

        public override IReadOnlyList<FabricStyle> FabricStyles
        {
            get
            {
                return m_fabricStyles;
            }
        }

        public GridLayoutNode GridLayoutNode
        {
            get
            {
                return m_gridLayoutNode;
            }
        }

        public override Node Node
        {
            get
            {
                return m_gridLayoutNode;
            }
        }

        public override string StyleKey
        {
            get
            {
                return m_styleKey;
            }
        }

        public bool Trim
        {
            get
            {
                return m_trim;
            }
        }

        public static string CreateStyleKey(FabricStyle fabricStyleBody, FabricStyle fabricStyleCorner, Area area)
        {
            var result = new StringBuilder()
                .Append(typeof(BuildComponentFlyingGoose).Name)
                .Append(StyleKeyDelimiter)
                .Append(fabricStyleBody.Sku)
                .Append(StyleKeyDelimiter)
                .Append(fabricStyleCorner.Sku)
                .Append(StyleKeyDelimiter)
                .Append(area.LargestDimension.ToString())
                .Append(StyleKeyDelimiter)
                .Append(area.SmallestDimension.ToString())
                .ToString();

            return result;
        }

        protected override IBuildComponent Clone(BuildComponentFactory factory)
        {
            return factory.CreateBuildComponentFlyingGooose(FabricStyles[0], FabricStyles[1], Area, Trim);
        }
    }
}