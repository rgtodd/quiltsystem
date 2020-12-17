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
    public abstract class Node
    {
        private string m_componentName;
        private string m_componentType;
        private readonly IPath m_path;
        private string m_style;
        private bool m_visible;

        protected Node(IPathGeometry pathGeometry)
        {
            if (pathGeometry == null) throw new ArgumentNullException(nameof(pathGeometry));

            m_path = pathGeometry.Prototype.Clone();
            m_visible = true;
        }

        internal Node(JToken json)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));

            m_path = PathFactory.Create(json[JsonNames.Bounds]);
            m_visible = (bool)json[JsonNames.Visible];
            m_style = (string)json[JsonNames.Style];
            m_componentName = (string)json[JsonNames.ComponentName];
            m_componentType = (string)json[JsonNames.ComponentType];
        }

        protected Node(Node prototype)
        {
            m_path = prototype.m_path.Clone();
            m_visible = prototype.m_visible;
            m_style = prototype.m_style;
            m_componentName = prototype.m_componentName;
            m_componentType = prototype.m_componentType;
        }

        public string ComponentName
        {
            get
            {
                return m_componentName;
            }

            set
            {
                m_componentName = value;
            }
        }

        public string ComponentType
        {
            get
            {
                return m_componentType;
            }

            set
            {
                m_componentType = value;
            }
        }

        public IPath Path
        {
            get
            {
                return m_path;
            }
        }

        public string Style
        {
            get
            {
                return m_style;
            }

            set
            {
                m_style = value;
            }
        }

        public bool Visible
        {
            get
            {
                return m_visible;
            }
        }

        public abstract Node Clone();

        public FabricStyleList GetFabricStyles()
        {
            var fabricStyles = new FabricStyleList();
            var shapeNodes = GetShapeNodes();
            foreach (var shapeNode in shapeNodes)
            {
                var fabricStyle = shapeNode.FabricStyle;

                if (fabricStyle != null && !fabricStyles.Contains(fabricStyle))
                {
                    fabricStyles.Add(fabricStyle);
                }
            }

            return fabricStyles;
        }

        public virtual Node GetNode(PathPoint point)
        {
            return Path.Contains(point) ? this : null;
        }

        public IList<ShapeNode> GetShapeNodes()
        {
            List<ShapeNode> result = new List<ShapeNode>();

            AddShapeNodesTo(result);

            return result;
        }

        public IList<LayoutSite> GetUnboundLayoutSites()
        {
            List<LayoutSite> result = new List<LayoutSite>();

            AddUnboundLayoutSitesTo(result);

            return result;
        }

        public virtual JToken JsonSave()
        {
            var result = new JObject()
            {
                new JProperty(JsonNames.TypeName, GetType().Name),
                new JProperty(JsonNames.Bounds, m_path.JsonSave()),
                new JProperty(JsonNames.Visible, m_visible),
                new JProperty(JsonNames.Style, m_style),
                new JProperty(JsonNames.ComponentName, m_componentName),
                new JProperty(JsonNames.ComponentType, m_componentType)
            };

            return result;
        }

        public override string ToString()
        {
            return GetType().Name;
        }

        public virtual void UpdatePath(IPath path, PathOrientation pathOrientation, DimensionScale scale)
        {
            if (path != null)
            {
                Path.Copy(path, pathOrientation);
                m_visible = true;
            }
            else
            {
                m_visible = false;
            }
        }

        internal abstract void AddShapeNodesTo(List<ShapeNode> shapeNodes);

        internal abstract void AddUnboundLayoutSitesTo(List<LayoutSite> layoutSites);
    }
}