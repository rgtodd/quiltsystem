//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Design.Component
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    sealed class ComponentAttribute : Attribute
    {
        private string m_typeName;

        public ComponentAttribute(string typeName)
        {
            m_typeName = typeName ?? throw new ArgumentNullException(nameof(typeName));
        }

        public string TypeName
        {
            get
            {
                return m_typeName;
            }

            set
            {
                m_typeName = value ?? throw new ArgumentNullException(nameof(value));
            }
        }
    }
}
