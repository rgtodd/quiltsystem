//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RichTodd.QuiltSystem.Design.Nodes;
using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Build
{
    internal class BuildComponentLayout : BuildComponent
    {
        private readonly LayoutNode m_layoutNode;
        private readonly string m_styleKey;
        private readonly bool m_trimTriangles;

        public BuildComponentLayout(string id, LayoutNode layoutNode, bool trimTriangles)
            : base(id)
        {
            if (layoutNode == null) throw new ArgumentNullException(nameof(layoutNode));

            if (string.IsNullOrEmpty(layoutNode.ComponentType)) throw new ArgumentException("ComponentType is null.", nameof(layoutNode));
            if (string.IsNullOrEmpty(layoutNode.ComponentName)) throw new ArgumentException("ComponentName is null.", nameof(layoutNode));

            m_layoutNode = (LayoutNode)layoutNode.Clone();
            m_trimTriangles = trimTriangles;

            m_styleKey = CreateStyleKey(m_layoutNode.ComponentType, m_layoutNode.ComponentName, m_layoutNode.GetFabricStyles());
        }

        public override Area Area
        {
            get
            {
                var bounds = m_layoutNode.Path.GetBounds();
                return new Area(bounds.MaximumX - bounds.MinimumX, bounds.MaximumY - bounds.MinimumY);
            }
        }

        public override string ComponentSubtype
        {
            get
            {
                return m_layoutNode.ComponentName;
            }
        }

        public override string ComponentType
        {
            get
            {
                return BuildComponentTypes.Block;
            }
        }

        public override IReadOnlyList<FabricStyle> FabricStyles
        {
            get
            {
                return s_emptyFabricStyleList;
            }
        }

        public LayoutNode LayoutNode
        {
            get
            {
                return m_layoutNode;
            }
        }

        public override Node Node
        {
            get
            {
                return m_layoutNode;
            }
        }

        public override string StyleKey
        {
            get
            {
                return m_styleKey;
            }
        }

        public bool TrimTriangles
        {
            get
            {
                return m_trimTriangles;
            }
        }

        public static string CreateStyleKey(string componentType, string componentName, IList<FabricStyle> fabricStyles)
        {
            var sb = new StringBuilder();

            _ = sb.Append(typeof(BuildComponentLayout).Name)
                .Append(StyleKeyDelimiter)
                .Append(componentType)
                .Append(StyleKeyDelimiter)
                .Append(componentName);

            foreach (var fabricStyle in fabricStyles.OrderBy(r => r.Sku))
            {
                _ = sb.Append(StyleKeyDelimiter)
                    .Append(fabricStyle.Sku);
            }

            return sb.ToString();
        }

        protected override IBuildComponent Clone(BuildComponentFactory factory)
        {
            return factory.CreateBuildComponentLayout(LayoutNode, TrimTriangles);
        }
    }
}