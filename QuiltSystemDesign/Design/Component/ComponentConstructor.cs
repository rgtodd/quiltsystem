//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Reflection;

using Newtonsoft.Json.Linq;

using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Component
{
    internal class ComponentConstructor
    {
        private readonly ComponentAttribute m_attribute;
        private readonly ConstructorInfo m_jsonConstructor;
        private readonly ConstructorInfo m_parameterConstructor;
        private readonly Type m_type;

        public ComponentConstructor(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            object[] attributes = type.GetCustomAttributes(typeof(ComponentAttribute), false);
            if (attributes.Length != 1)
            {
                throw new ArgumentException("ComponentAttribute not found.", nameof(type));
            }

            m_type = type;
            m_attribute = (ComponentAttribute)attributes[0];
            m_parameterConstructor = type.GetConstructor(new Type[] { typeof(string), typeof(string), typeof(FabricStyleList), typeof(ComponentParameterCollection) });
            m_jsonConstructor = type.GetConstructor(new Type[] { typeof(JToken) });
        }

        public Type Type
        {
            get
            {
                return m_type;
            }
        }

        public string TypeName
        {
            get { return m_attribute.TypeName; }
        }

        public Component Create(string category, string name, FabricStyleList fabricStyles, ComponentParameterCollection parameters)
        {
            return (Component)m_parameterConstructor.Invoke(new object[] { category, name, fabricStyles, parameters });
        }

        public Component Create(JToken json)
        {
            return (Component)m_jsonConstructor.Invoke(new object[] { json });
        }
    }
}