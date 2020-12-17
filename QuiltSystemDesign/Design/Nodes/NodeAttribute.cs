//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Design.Nodes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    internal sealed class NodeAttribute : Attribute
    {
        private readonly string m_pathGeometryName;

        public NodeAttribute(string pathGeometryName)
        {
            m_pathGeometryName = pathGeometryName;
        }

        public string PathGeometryName
        {
            get
            {
                return m_pathGeometryName;
            }
        }
    }
}