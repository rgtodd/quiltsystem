//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Reflection;
using System.Text;

using Newtonsoft.Json.Linq;

using RichTodd.QuiltSystem.Design.Path;

namespace RichTodd.QuiltSystem.Design.Nodes
{
    public class NodeConstructor
    {
        private readonly Type m_type;

        private readonly NodeAttribute m_attribute;
        private readonly ConstructorInfo m_defaultConstructor;
        private readonly ConstructorInfo m_jsonConstructor;

        public NodeConstructor(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            object[] attributes = type.GetCustomAttributes(typeof(NodeAttribute), false);
            if (attributes.Length != 1)
            {
                throw new ArgumentException("NodeAttribute not found.", nameof(type));
            }

            m_type = type;

            m_attribute = (NodeAttribute)attributes[0];
            m_defaultConstructor = type.GetConstructor(Type.EmptyTypes);
            m_jsonConstructor = type.GetConstructor(new Type[] { typeof(JToken) });
        }

        public string TypeName
        {
            get { return m_type.Name; }
        }

        public Type Type
        {
            get
            {
                return m_type;
            }
        }

        public IPathGeometry PathGeometry
        {
            get
            {
                return PathGeometries.Lookup(m_attribute.PathGeometryName);
            }
        }

        public bool HasDefaultConstructor
        {
            get
            {
                return m_defaultConstructor != null;
            }
        }

        public Node Create()
        {
            var result = (Node)m_defaultConstructor.Invoke(null);

            return result;
        }

        public Node Create(JToken json)
        {
            var result = (Node)m_jsonConstructor.Invoke(new object[] { json });

            return result;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append(m_type.Name);
            sb.Append(" (");
            sb.Append(m_attribute.PathGeometryName);
            sb.Append(")");

            return sb.ToString();
        }
    }
}
