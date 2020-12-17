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
    internal class BuildComponentRectangle : BuildComponent
    {
        private readonly Area m_area;
        private readonly RectangleShapeNode m_rectangleShapeNode;
        private readonly string m_styleKey;

        public BuildComponentRectangle(string id, FabricStyle fabricStyle, Area area)
            : base(id)
        {
            if (fabricStyle == null) throw new ArgumentNullException(nameof(fabricStyle));
            if (area == null) throw new ArgumentNullException(nameof(area));

            if (area.Width < area.Height)
            {
                area = new Area(area.Height, area.Width);
            }

            m_area = area;

            m_styleKey = CreateStyleKey(fabricStyle, area);

            var rectangleShapeNode = new RectangleShapeNode(fabricStyle);
            var scale = DimensionScale.CreateIdentity(area.Width.Unit);
            rectangleShapeNode.UpdatePath(PathGeometries.Rectangle.CreatePath(m_area.Width, m_area.Height), PathOrientation.CreateDefault(), scale);

            m_rectangleShapeNode = rectangleShapeNode;
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
                return Area.Height == Area.Width ? "Square" : "Rectangle";
            }
        }

        public override string ComponentType
        {
            get
            {
                return BuildComponentTypes.Piece;
            }
        }

        public FabricStyle FabricStyle
        {
            get
            {
                return m_rectangleShapeNode.FabricStyle;
            }
        }

        public override IReadOnlyList<FabricStyle> FabricStyles
        {
            get
            {
                return new FabricStyle[] {
                    m_rectangleShapeNode.FabricStyle
                };
            }
        }

        public override Node Node
        {
            get
            {
                return m_rectangleShapeNode;
            }
        }

        public RectangleShapeNode RectangleShapeNode
        {
            get
            {
                return m_rectangleShapeNode;
            }
        }

        public override string StyleKey
        {
            get
            {
                return m_styleKey;
            }
        }

        public static string CreateStyleKey(FabricStyle fabricStyle, Area area)
        {
            var sb = new StringBuilder()
                .Append(typeof(BuildComponentRectangle).Name)
                .Append(StyleKeyDelimiter)
                .Append(fabricStyle.Sku)
                .Append(StyleKeyDelimiter)
                .Append(area.LargestDimension.ToString())
                .Append(StyleKeyDelimiter)
                .Append(area.SmallestDimension.ToString());

            return sb.ToString();
        }

        protected override IBuildComponent Clone(BuildComponentFactory factory)
        {
            return factory.CreateBuildComponentRectangle(FabricStyle, Area);
        }
    }
}