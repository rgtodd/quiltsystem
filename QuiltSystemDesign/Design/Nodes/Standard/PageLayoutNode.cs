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
    public class PageLayoutNode : LayoutNode
    {
        private Dimension m_height;
        private readonly LayoutSiteList m_layoutSites;
        private Dimension m_width;

        public PageLayoutNode(Dimension width, Dimension height) : base(PathGeometries.Rectangle)
        {
            if (width.Unit != height.Unit) throw new ArgumentException("Mismatched units of measure.");
            if (width < new Dimension(1, width.Unit)) throw new ArgumentOutOfRangeException(nameof(width));
            if (height < new Dimension(1, height.Unit)) throw new ArgumentOutOfRangeException(nameof(height));

            m_width = width;
            m_height = height;
            m_layoutSites = new LayoutSiteList(this)
            {
                new LayoutSite(this, PathGeometries.Rectangle)
            };
        }

        public PageLayoutNode(JToken json) : base(json)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));

            m_width = Dimension.Parse((string)json[JsonNames.Width]);
            m_height = Dimension.Parse((string)json[JsonNames.Height]);
            m_layoutSites = new LayoutSiteList(this, json[JsonNames.LayoutSites]);

            if (m_width.Unit != m_height.Unit) throw new ArgumentException("Mismatched units of measure.");
        }

        protected PageLayoutNode(PageLayoutNode prototype) : base(prototype)
        {
            if (prototype == null) throw new ArgumentNullException(nameof(prototype));

            m_width = prototype.m_width;
            m_height = prototype.m_height;
            m_layoutSites = prototype.m_layoutSites.Clone(this);
        }

        public Dimension Height
        {
            get
            {
                return m_height;
            }

            set
            {
                if (value < new Dimension(1, value.Unit)) throw new ArgumentOutOfRangeException(nameof(value));

                m_height = value;
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
                if (value < new Dimension(1, value.Unit)) throw new ArgumentOutOfRangeException(nameof(value));

                m_width = value;
            }
        }

        public override Node Clone()
        {
            return new PageLayoutNode(this);
        }

        public override JToken JsonSave()
        {
            var result = base.JsonSave();

            result[JsonNames.Width] = m_width.ToString();
            result[JsonNames.Height] = m_height.ToString();
            result[JsonNames.LayoutSites] = m_layoutSites.JsonSave();

            return result;
        }

        public void UpdateBounds(PathOrientation pathOrientation, DimensionScale scale)
        {
            var zero = new Dimension(0, m_width.Unit);

            var path = PathGeometries.Rectangle.CreatePath(
                new PathPoint[] {
                    new PathPoint(zero, zero),
                    new PathPoint(m_width, zero),
                    new PathPoint(m_width, m_height),
                    new PathPoint(zero, m_height)
                });

            UpdatePath(path, pathOrientation, scale);
        }

        public override void UpdatePath(IPath path, PathOrientation pathOrientation, DimensionScale scale)
        {
            base.UpdatePath(path, pathOrientation, scale);

            LayoutSite layoutSite = LayoutSites[0];
            layoutSite.UpdatePath(Path, scale);
        }
    }
}