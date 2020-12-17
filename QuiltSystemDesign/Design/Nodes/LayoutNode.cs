//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using RichTodd.QuiltSystem.Design.Path;

namespace RichTodd.QuiltSystem.Design.Nodes
{
    public abstract class LayoutNode : Node
    {
        protected LayoutNode(IPathGeometry pathGeometry) : base(pathGeometry)
        { }

        protected LayoutNode(JToken json) : base(json)
        { }

        protected LayoutNode(LayoutNode prototype) : base(prototype)
        { }

        public abstract IReadOnlyList<LayoutSite> LayoutSites { get; }

        public LayoutSite GetLayoutSite(PathPoint point)
        {
            foreach (var layoutSite in LayoutSites)
            {
                if (layoutSite.Path.Contains(point))
                {
                    if (layoutSite.Node is LayoutNode childLayout)
                    {
                        return childLayout.GetLayoutSite(point);
                    }

                    return layoutSite;
                }
            }

            return null;
        }

        public override Node GetNode(PathPoint point)
        {
            foreach (var layoutSite in LayoutSites)
            {
                if (layoutSite.Path.Contains(point))
                {
                    var node = layoutSite.Node;
                    if (node != null)
                    {
                        return node.GetNode(point);
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            return null;
        }

        internal virtual void AddChildrenTo(List<Node> nodes)
        {
            foreach (var layoutSite in LayoutSites)
            {
                if (layoutSite.Node != null)
                {
                    nodes.Add(layoutSite.Node);
                }
            }
        }

        internal override void AddShapeNodesTo(List<ShapeNode> shapeNodes)
        {
            foreach (var layoutSite in LayoutSites)
            {
                if (layoutSite.Node != null)
                {
                    layoutSite.Node.AddShapeNodesTo(shapeNodes);
                }
            }
        }

        internal override void AddUnboundLayoutSitesTo(List<LayoutSite> layoutSites)
        {
            foreach (var layoutSite in LayoutSites)
            {
                if (layoutSite.Node == null)
                {
                    layoutSites.Add(layoutSite);
                }
                else
                {
                    layoutSite.Node.AddUnboundLayoutSitesTo(layoutSites);
                }
            }
        }
    }
}