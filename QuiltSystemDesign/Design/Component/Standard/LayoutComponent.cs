//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

using Newtonsoft.Json.Linq;

using RichTodd.QuiltSystem.Design.Nodes;
using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Component.Standard
{
    [Component(TypeName)]
    public class LayoutComponent : Component
    {
        public const string TypeName = "Layout";

        public const string RowCountParameter = "RowCount";
        public const string ColumnCountParameter = "ColumnCount";
        public const string BlockCountParameter = "BlockCount";

        private static Configuration s_configuration;

        private readonly ComponentList m_children;

        public static void Configure(INodeFactory nodeFactory)
        {
            s_configuration = new Configuration()
            {
                NodeFactory = nodeFactory
            };
        }

        public static LayoutComponent Create(string category, string name, FabricStyleList fabricStyles, int rowCount, int columnCount, int blockCount)
        {
            if (rowCount < 1) throw new ArgumentOutOfRangeException(nameof(rowCount));
            if (columnCount < 1) throw new ArgumentOutOfRangeException(nameof(columnCount));
            if (blockCount < 1) throw new ArgumentOutOfRangeException(nameof(blockCount));

            var parameters = new ComponentParameterCollection();
            parameters[RowCountParameter] = rowCount.ToString();
            parameters[ColumnCountParameter] = columnCount.ToString();
            parameters[BlockCountParameter] = blockCount.ToString();

            return new LayoutComponent(category, name, fabricStyles, parameters);
        }

        private LayoutComponent(string category, string name, FabricStyleList fabricStyles, ComponentParameterCollection parameters) : base(TypeName, category, name, fabricStyles, parameters)
        {
            m_children = new ComponentList();
        }

        public LayoutComponent(JToken json) : base(json)
        {
            if (Type != TypeName)
            {
                throw new ArgumentException("TypeName attribute mismatch.", nameof(json));
            }

            m_children = new ComponentList(json[JsonNames.Children]);
        }

        protected LayoutComponent(LayoutComponent prototype) : base(prototype)
        {
            m_children = prototype.m_children.Clone();
        }

        public override JToken JsonSave()
        {
            var result = base.JsonSave();

            result[JsonNames.TypeName] = TypeName;
            result[JsonNames.Children] = m_children.JsonSave();

            return result;
        }

        public override Component Clone()
        {
            return new LayoutComponent(this);
        }

        public ComponentList Children
        {
            get
            {
                return m_children;
            }
        }

        public int RowCount
        {
            get
            {
                return int.Parse(Parameters[RowCountParameter]);
            }
        }

        public int ColumnCount
        {
            get
            {
                return int.Parse(Parameters[ColumnCountParameter]);
            }
        }

        public int BlockCount
        {
            get
            {
                return int.Parse(Parameters[BlockCountParameter]);
            }
        }

        public override Node Expand(bool includeChildren)
        {
            return includeChildren 
                ? Expand(Children) 
                : Expand(null);
        }

        private Node Expand(ComponentList children)
        {
            var childNodes = new NodeList();

            if (children != null)
            {
                foreach (var child in children)
                {
                    var childNode = child.Expand(true);
                    childNodes.Add(childNode);
                }
            }

            var node = s_configuration.NodeFactory.Create(this, childNodes);

            return node;
        }

        private class Configuration
        {
            public INodeFactory NodeFactory { get; set; }
        }
    }
}
