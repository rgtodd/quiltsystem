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
    internal class RectangleOnPointLayoutNode : LayoutNode
    {
        private readonly LayoutSiteList m_layoutSites;

        public RectangleOnPointLayoutNode() : base(PathGeometries.Rectangle)
        {
            m_layoutSites = new LayoutSiteList(this);

            for (int idx = 0; idx < 4; ++idx)
            {
                m_layoutSites.Add(new LayoutSite(this, PathGeometries.Triangle));
            }
            m_layoutSites.Add(new LayoutSite(this, PathGeometries.Rectangle));
        }

        public RectangleOnPointLayoutNode(JToken json) : base(json)
        {
            m_layoutSites = new LayoutSiteList(this, json[JsonNames.LayoutSites]);
        }

        protected RectangleOnPointLayoutNode(RectangleOnPointLayoutNode prototype) : base(prototype)
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
            return new RectangleOnPointLayoutNode(this);
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

            //Trace.TraceInformation("HalfSquareTriangleLayout::ResizeChildren");

            PathPoint topMidpoint = Path.Interpolate(0, 0.5);
            PathPoint rightMidpoint = Path.Interpolate(1, 0.5);
            PathPoint bottomMidpoint = Path.Interpolate(2, 0.5);
            PathPoint leftMidpoint = Path.Interpolate(3, 0.5);

            // Upper left triangle
            {
                IPath pathUpperLeft = PathGeometries.Triangle.CreatePath(new PathPoint[] {
                    leftMidpoint,
                    Path.GetSegment(0).Origin,
                    topMidpoint });

                //Trace.TraceInformation("Upper left = {0}", path.ToString());

                LayoutSite layoutSite = LayoutSites[0];
                layoutSite.UpdatePath(pathUpperLeft, scale);
            }

            // Upper right triangle
            {
                IPath pathUpperRight = PathGeometries.Triangle.CreatePath(new PathPoint[] {
                    topMidpoint,
                    Path.GetSegment(1).Origin,
                    rightMidpoint });

                //Trace.TraceInformation("Upper right = {0}", path.ToString());

                LayoutSite layoutSite = LayoutSites[1];
                layoutSite.UpdatePath(pathUpperRight, scale);
            }

            // Lower right triangle.
            //
            {
                IPath pathLowerRight = PathGeometries.Triangle.CreatePath(new PathPoint[] {
                    rightMidpoint,
                    Path.GetSegment(2).Origin,
                    bottomMidpoint });

                //Trace.TraceInformation("Lower right = {0}", path.ToString());

                LayoutSite layoutSite = LayoutSites[2];
                layoutSite.UpdatePath(pathLowerRight, scale);
            }

            // Lower left triangle.
            //
            {
                IPath pathLowerLeft = PathGeometries.Triangle.CreatePath(new PathPoint[] {
                    bottomMidpoint,
                    Path.GetSegment(3).Origin,
                    leftMidpoint });

                //Trace.TraceInformation("Lower left = {0}", path.ToString());

                LayoutSite layoutSite = LayoutSites[3];
                layoutSite.UpdatePath(pathLowerLeft, scale);
            }

            // Center
            //
            {
                IPath pathCenter = PathGeometries.Rectangle.CreatePath(new PathPoint[] {
                    topMidpoint,
                    rightMidpoint,
                    bottomMidpoint,
                    leftMidpoint });

                //Trace.TraceInformation("Center = {0}", path.ToString());

                LayoutSite layoutSite = LayoutSites[4];
                layoutSite.UpdatePath(pathCenter, scale);
            }
        }
    }
}