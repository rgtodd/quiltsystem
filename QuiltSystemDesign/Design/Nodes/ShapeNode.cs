//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using RichTodd.QuiltSystem.Design.Path;
using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Nodes
{
    public abstract class ShapeNode : Node
    {
        private FabricStyle m_fabricStyle;

        protected ShapeNode(FabricStyle fabricStyle, IPathGeometry pathGeometry) : base(pathGeometry)
        {
            if (pathGeometry == null) throw new ArgumentNullException(nameof(pathGeometry));

            m_fabricStyle = fabricStyle ?? throw new ArgumentNullException(nameof(fabricStyle));
        }

        protected ShapeNode(JToken json) : base(json)
        {
            var jsonFabricStyle = json[JsonNames.FabricStyle];
            if (jsonFabricStyle != null)
            {
                m_fabricStyle = new FabricStyle(jsonFabricStyle);
            }
            else
            {
                m_fabricStyle = FabricStyle.Default;
            }
        }

        protected ShapeNode(ShapeNode prototype)
            : base(prototype)
        {
            m_fabricStyle = prototype.m_fabricStyle.Clone();
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

        public override JToken JsonSave()
        {
            var result = base.JsonSave();

            result[JsonNames.FabricStyle] = m_fabricStyle.JsonSave();

            return result;
        }

        internal override void AddShapeNodesTo(List<ShapeNode> shapeNodes)
        {
            shapeNodes.Add(this);
        }

        internal override void AddUnboundLayoutSitesTo(List<LayoutSite> layoutSites)
        { }
    }
}