//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Design.Nodes.Generator
{
    internal class Fingerprint<T> where T : Enum
    {
        private readonly T[] m_values;

        public Fingerprint(T[] values)
        {
            m_values = values;
        }

        public T[] Values
        {
            get { return m_values; }
        }
    }
}