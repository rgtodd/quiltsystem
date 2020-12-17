//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Build
{
    internal class CutRegion
    {
        private readonly CutStock m_cutStock;
        private readonly Dimension m_height;
        private readonly Dimension m_left;
        private readonly Dimension m_top;
        private readonly Dimension m_width;
        private ICutShape m_cutShape;

        public CutRegion(CutStock cutStock, Dimension top, Dimension left, Dimension width, Dimension height)
        {
            m_cutStock = cutStock ?? throw new ArgumentNullException(nameof(cutStock));
            m_top = top;
            m_left = left;
            m_width = width;
            m_height = height;
        }

        public Dimension Area
        {
            get
            {
                return Width * Height;
            }
        }

        public ICutShape CutShape
        {
            get
            {
                return m_cutShape;
            }
            set
            {
                m_cutShape = value;
            }
        }

        public CutStock CutStock
        {
            get
            {
                return m_cutStock;
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

        public bool Contains(Dimension dimension1, Dimension dimension2)
        {
            return (Width >= dimension1 && Height >= dimension2) ||
                    (Height >= dimension1 && Width >= dimension2);
        }

        public Tuple<CutRegion, CutRegion> CutHorizontal(Dimension distance)
        {
            if (CutShape != null)
            {
                throw new InvalidOperationException("Cannot cut region with assigned shape.");
            }

            var top = new CutRegion(CutStock, Top, Left, Width, distance);
            var bottom = new CutRegion(CutStock, Top + distance, Left, Width, Height - distance);

            return new Tuple<CutRegion, CutRegion>(top, bottom);
        }

        public Tuple<CutRegion, CutRegion> CutVertical(Dimension distance)
        {
            if (CutShape != null)
            {
                throw new InvalidOperationException("Cannot cut region with assigned shape.");
            }

            var left = new CutRegion(CutStock, Top, Left, distance, Height);
            var right = new CutRegion(CutStock, Top, Left + distance, Width - distance, Height);

            return new Tuple<CutRegion, CutRegion>(left, right);
        }
    }
}