//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Web.Bootstrap
{
    public sealed class MultilineAttribute : Attribute
    {
        private readonly int m_rows;

        public MultilineAttribute(int rows)
        {
            m_rows = rows;
        }

        public int Rows
        {
            get { return m_rows; }
        }
    }
}