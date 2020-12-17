//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Build
{
    internal class BuildComponentYardageRegion
    {
        private readonly BuildComponentRectangle m_buildComponentRectangle;
        private readonly Dimension m_height;
        private readonly Dimension m_left;
        private readonly Dimension m_top;
        private readonly Dimension m_width;

        public BuildComponentYardageRegion(BuildComponentRectangle buildComponentRectangle, Dimension left, Dimension top, Dimension width, Dimension height)
        {
            m_buildComponentRectangle = buildComponentRectangle ?? throw new ArgumentNullException(nameof(buildComponentRectangle));
            m_left = left;
            m_top = top;
            m_width = width;
            m_height = height;
        }

        public BuildComponentRectangle BuildComponentRectangle
        {
            get
            {
                return m_buildComponentRectangle;
            }
        }

        public Dimension Height
        {
            get
            {
                return m_height;
            }
        }

        public Dimension Left
        {
            get
            {
                return m_left;
            }
        }

        public Dimension Top
        {
            get
            {
                return m_top;
            }
        }

        public Dimension Width
        {
            get
            {
                return m_width;
            }
        }
    }
}