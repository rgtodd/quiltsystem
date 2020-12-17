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
    [Node(PathGeometryNames.TRIANGLE)]
    internal class SplitTriangleLayoutNode : LayoutNode
    {
        private readonly LayoutSiteList m_layoutSites;

        public SplitTriangleLayoutNode() : base(PathGeometries.Triangle)
        {
            m_layoutSites = new LayoutSiteList(this);

            for (int idx = 0; idx < 2; ++idx)
            {
                m_layoutSites.Add(new LayoutSite(this, PathGeometries.Triangle));
            }
        }

        public SplitTriangleLayoutNode(JToken json) : base(json)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));

            m_layoutSites = new LayoutSiteList(this, json[JsonNames.LayoutSites]);
        }

        protected SplitTriangleLayoutNode(SplitTriangleLayoutNode prototype) : base(prototype)
        {
            if (prototype == null) throw new ArgumentNullException(nameof(prototype));

            m_layoutSites = prototype.m_layoutSites.Clone(this);
        }

        public override IReadOnlyList<LayoutSite> LayoutSites
        {
            get
            {
                return m_layoutSites;
            }
        }

        public override Node Clone()
        {
            return new SplitTriangleLayoutNode(this);
        }

        public override JToken JsonSave()
        {
            var result = base.JsonSave();

            result[JsonNames.LayoutSites] = m_layoutSites.JsonSave();

            return result;
        }

        public override void UpdatePath(IPath path, PathOrientation pathOrientation, DimensionScale scale)
        {
            base.UpdatePath(path, pathOrientation, scale);

            //Trace.TraceInformation("SplitTriangleLayout::ResizeChildren");

            PathPoint bottomMidpoint = Path.Interpolate(2, 0.5);

            // Upper left triangle
            {
                IPath pathUpperLeft = PathGeometries.Triangle.CreatePath(new PathPoint[] { Path.GetSegment(0).Origin, Path.GetSegment(1).Origin, bottomMidpoint });

                //Trace.TraceInformation("Left = {0}", path.ToString());

                LayoutSite layoutSite = LayoutSites[0];
                layoutSite.UpdatePath(pathUpperLeft, scale);
            }

            // Lower right triangle.
            //
            {
                IPath pathLowerRight = PathGeometries.Triangle.CreatePath(new PathPoint[] { bottomMidpoint, Path.GetSegment(1).Origin, Path.GetSegment(2).Origin });

                //Trace.TraceInformation("Right = {0}", path.ToString());

                LayoutSite layoutSite = LayoutSites[1];
                layoutSite.UpdatePath(pathLowerRight, scale);
            }
        }
    }
}