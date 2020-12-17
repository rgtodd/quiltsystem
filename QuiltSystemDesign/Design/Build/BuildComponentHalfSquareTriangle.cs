//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Text;

using RichTodd.QuiltSystem.Design.Nodes;
using RichTodd.QuiltSystem.Design.Nodes.Standard;
using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Build
{
    internal class BuildComponentHalfSquareTriangle : BuildComponent
    {
        private readonly Area m_area;
        private readonly FabricStyle[] m_fabricStyles;
        private readonly HalfSquareTriangleLayoutNode m_halfSquareTriangleLayoutNode;
        private readonly string m_styleKey;
        private readonly bool m_trim;

        public BuildComponentHalfSquareTriangle(string id, HalfSquareTriangleLayoutNode halfSquareTriangleLayoutNode, bool trim)
            : base(id)
        {
            if (halfSquareTriangleLayoutNode == null) throw new ArgumentNullException(nameof(halfSquareTriangleLayoutNode));

            m_halfSquareTriangleLayoutNode = (HalfSquareTriangleLayoutNode)halfSquareTriangleLayoutNode.Clone();
            m_trim = trim;

            var fabricStyle1 = ((ShapeNode)halfSquareTriangleLayoutNode.LayoutSites[0].Node).FabricStyle;
            var fabricStyle2 = ((ShapeNode)halfSquareTriangleLayoutNode.LayoutSites[1].Node).FabricStyle;
            if (fabricStyle1.Sku.CompareTo(fabricStyle2.Sku) <= 0)
            {
                // No change
            }
            else
            {
                var temp = fabricStyle2;
                fabricStyle2 = fabricStyle1;
                fabricStyle1 = temp;
            }
            m_fabricStyles = new FabricStyle[] { fabricStyle1, fabricStyle2 };

            var nodeBounds = halfSquareTriangleLayoutNode.Path.GetBounds();
            var width = nodeBounds.MaximumX - nodeBounds.MinimumX;
            var height = nodeBounds.MaximumY - nodeBounds.MinimumY;
            m_area = new Area(width, height).Round();

            m_styleKey = CreateStyleKey(fabricStyle1, fabricStyle2, m_area);
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
                return "Half-Square Triangle";
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

        public HalfSquareTriangleLayoutNode HalfSquareTriangleLayoutNode
        {
            get
            {
                return m_halfSquareTriangleLayoutNode;
            }
        }

        public override Node Node
        {
            get
            {
                return m_halfSquareTriangleLayoutNode;
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

        public static string CreateStyleKey(FabricStyle fabricStyle1, FabricStyle fabricStyle2, Area area)
        {
            var sb = new StringBuilder();

            sb.Append(typeof(BuildComponentHalfSquareTriangle).Name);
            sb.Append(StyleKeyDelimiter);
            sb.Append(fabricStyle1.Sku.CompareTo(fabricStyle2.Sku) <= 0 ? fabricStyle1.Sku : fabricStyle2.Sku);
            sb.Append(StyleKeyDelimiter);
            sb.Append(fabricStyle1.Sku.CompareTo(fabricStyle2.Sku) <= 0 ? fabricStyle2.Sku : fabricStyle1.Sku);
            sb.Append(StyleKeyDelimiter);
            sb.Append(area.LargestDimension.ToString());
            sb.Append(StyleKeyDelimiter);
            sb.Append(area.SmallestDimension.ToString());

            return sb.ToString();
        }

        protected override IBuildComponent Clone(BuildComponentFactory factory)
        {
            return factory.CreateBuildComponentHalfSquareTriangle(m_halfSquareTriangleLayoutNode, Trim);
        }
    }
}