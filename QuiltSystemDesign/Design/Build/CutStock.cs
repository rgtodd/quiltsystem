//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Build
{
    internal class CutStock
    {
        private readonly Area m_area;
        private readonly AreaSizes m_areaSize;

        public CutStock(AreaSizes areaSize)
        {
            m_areaSize = areaSize;
            m_area = Area.Create(areaSize);
        }

        public Area Area
        {
            get
            {
                return m_area;
            }
        }

        public AreaSizes AreaSize
        {
            get
            {
                return m_areaSize;
            }
        }
    }
}