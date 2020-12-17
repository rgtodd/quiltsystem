//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

using RichTodd.QuiltSystem.Design.Core;
using RichTodd.QuiltSystem.Design.Nodes;
using RichTodd.QuiltSystem.Design.Nodes.Standard;
using RichTodd.QuiltSystem.Design.Path;
using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Build
{
    internal class BuildComponentQuilt : BuildComponent
    {
        private readonly Core.Design m_design;
        private readonly KitSpecification m_kitSpecification;
        private readonly PageLayoutNode m_pageLayoutNode;
        private readonly string m_styleKey;

        public BuildComponentQuilt(string id, KitSpecification kitSpecification, Core.Design design)
            : base(id)
        {
            m_kitSpecification = kitSpecification ?? throw new ArgumentNullException(nameof(kitSpecification));
            m_design = design ?? throw new ArgumentNullException(nameof(design));

            m_styleKey = GetType().Name + StyleKeyDelimiter + Guid.NewGuid().ToString();

            m_pageLayoutNode = new PageLayoutNode(
                kitSpecification.Width + (kitSpecification.BorderWidth * 2),
                kitSpecification.Height + (kitSpecification.BorderWidth * 2));
            m_pageLayoutNode.LayoutSites[0].Node = kitSpecification.Expand(design);
            m_pageLayoutNode.UpdateBounds(PathOrientation.CreateDefault(), new DimensionScale(1, DimensionUnits.Inch, 1, DimensionUnits.Inch));
        }

        public override Area Area
        {
            get
            {
                var bounds = m_pageLayoutNode.Path.GetBounds();
                return new Area(bounds.MaximumX - bounds.MinimumX, bounds.MaximumY - bounds.MinimumY);
            }
        }

        public override string ComponentSubtype
        {
            get
            {
                return null;
            }
        }

        public override string ComponentType
        {
            get
            {
                return BuildComponentTypes.Quilt;
            }
        }

        public Core.Design Design
        {
            get
            {
                return m_design;
            }
        }

        public override IReadOnlyList<FabricStyle> FabricStyles
        {
            get
            {
                return s_emptyFabricStyleList;
            }
        }

        public KitSpecification KitSpecification
        {
            get
            {
                return m_kitSpecification;
            }
        }

        public override Node Node
        {
            get
            {
                return m_pageLayoutNode;
            }
        }

        public PageLayoutNode PageLayoutNode
        {
            get
            {
                return m_pageLayoutNode;
            }
        }

        public override string StyleKey
        {
            get
            {
                return m_styleKey;
            }
        }

        protected override IBuildComponent Clone(BuildComponentFactory factory)
        {
            return factory.CreateBuildComponentQuilt(KitSpecification, Design);
        }
    }
}