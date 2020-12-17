//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Text;

using Newtonsoft.Json.Linq;

using RichTodd.QuiltSystem.Design.Path;
using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Nodes
{
    public class LayoutSite
    {
        private readonly LayoutNode m_parent;
        private readonly IPath m_path;
        private readonly PathOrientation m_pathOrientation;
        private Node m_node;
        private string m_style;

        public LayoutSite(LayoutNode parent, IPathGeometry pathGeometry)
        {
            if (pathGeometry == null) throw new ArgumentNullException(nameof(pathGeometry));

            m_parent = parent ?? throw new ArgumentNullException(nameof(parent));
            m_path = pathGeometry.Prototype.Clone();

            m_style = null;
            m_pathOrientation = PathOrientation.CreateDefault();
            m_node = null;
        }

        public LayoutSite(LayoutNode parent, JToken json)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));

            m_parent = parent ?? throw new ArgumentNullException(nameof(parent));
            m_path = PathFactory.Create(json[JsonNames.Bounds]);

            m_style = (string)json[JsonNames.Style];
            m_pathOrientation = new PathOrientation(json[JsonNames.PathOrientation]);

            var jsonNode = json[JsonNames.Node];
            if (jsonNode != null)
            {
                m_node = NodeFactory.Singleton.Create(jsonNode);
            }
        }

        protected LayoutSite(LayoutNode parent, LayoutSite prototype)
        {
            if (prototype == null) throw new ArgumentNullException(nameof(prototype));

            m_parent = parent ?? throw new ArgumentNullException(nameof(parent));
            m_path = prototype.m_path.Clone();

            m_style = prototype.m_style;
            m_pathOrientation = prototype.m_pathOrientation.Clone();
            m_node = prototype.m_node?.Clone();
        }

        public Node Node
        {
            get
            {
                return m_node;
            }

            set
            {
                if (value != m_node)
                {
                    if (value != null && value.Path.PathGeometry != Path.PathGeometry)
                    {
                        throw new ArgumentException("Mismatched path geometries.", nameof(value));
                    }

                    m_node = value;
                }
            }
        }

        public LayoutNode Parent
        {
            get
            {
                return m_parent;
            }
        }

        public IPath Path
        {
            get
            {
                return m_path;
            }
        }

        public PathOrientation PathOrientation
        {
            get
            {
                return m_pathOrientation;
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

        public LayoutSite Clone(LayoutNode layout)
        {
            return new LayoutSite(layout, this);
        }

        public void Flip()
        {
            PathOrientation.Flip();
        }

        public JToken JsonSave()
        {
            var result = new JObject()
            {
                new JProperty(JsonNames.Style, m_style),
                new JProperty(JsonNames.Bounds, m_path.JsonSave()),
                new JProperty(JsonNames.PathOrientation, m_pathOrientation.JsonSave())
            };

            if (m_node != null)
            {
                result[JsonNames.Node] = m_node.JsonSave();
            }

            return result;
        }

        public void RotateLeft()
        {
            PathOrientation.RotateLeft(Path.SegmentCount);
        }

        public void RotateRight()
        {
            PathOrientation.RotateRight(Path.SegmentCount);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            _ = sb.Append(Path.PathGeometry.ToString());

            var node = Node;
            if (node != null)
            {
                _ = sb.Append(" -> ");
                _ = sb.Append(node.ToString());
            }

            return sb.ToString();
        }

        public void UpdatePath(IPath path, DimensionScale scale)
        {
            Path.Copy(path);
            UpdateNodeBounds(scale);
        }

        private void UpdateNodeBounds(DimensionScale scale)
        {
            var node = Node;
            if (node != null)
            {
                node.UpdatePath(Path, PathOrientation, scale);
            }
        }
    }
}