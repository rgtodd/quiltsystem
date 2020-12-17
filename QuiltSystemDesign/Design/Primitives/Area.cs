//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Design.Primitives
{
    public class Area
    {
        #region Members

        private static readonly Area s_fatQuarter = new Area(new Dimension(20, DimensionUnits.Inch), new Dimension(18, DimensionUnits.Inch));
        private static readonly Area s_halfYard = new Area(new Dimension(40, DimensionUnits.Inch), new Dimension(18, DimensionUnits.Inch));
        private static readonly Area s_yard = new Area(new Dimension(40, DimensionUnits.Inch), new Dimension(36, DimensionUnits.Inch));
        private static readonly Area s_twoYards = new Area(new Dimension(40, DimensionUnits.Inch), new Dimension(72, DimensionUnits.Inch));
        private static readonly Area s_threeYards = new Area(new Dimension(40, DimensionUnits.Inch), new Dimension(108, DimensionUnits.Inch));

        private readonly Dimension m_width;
        private readonly Dimension m_height;

        #endregion

        public Area(Dimension width, Dimension height)
        {
            m_width = width;
            m_height = height;
        }

        public static Area CreateHorizontalArea(Dimension width, Dimension height)
        {
            return width >= height ? new Area(width, height) : new Area(height, width);
        }

        public static Area Create(AreaSizes areaSize)
        {
            return areaSize switch
            {
                AreaSizes.FatQuarter => s_fatQuarter,
                AreaSizes.HalfYard => s_halfYard,
                AreaSizes.Yard => s_yard,
                AreaSizes.TwoYards => s_twoYards,
                AreaSizes.ThreeYards => s_threeYards,
                _ => throw new InvalidOperationException(string.Format("Unknown area size {0}", areaSize)),
            };
        }

        public static AreaSizes GetSmallestContainingStandardArea(Area area)
        {
            if (s_fatQuarter.Contains(area)) return AreaSizes.FatQuarter;
            if (s_halfYard.Contains(area)) return AreaSizes.HalfYard;
            if (s_yard.Contains(area)) return AreaSizes.Yard;
            if (s_twoYards.Contains(area)) return AreaSizes.TwoYards;
            if (s_threeYards.Contains(area)) return AreaSizes.ThreeYards;

            throw new InvalidOperationException(string.Format("No standard area contains {0}", area));
        }

        public Dimension Width
        {
            get
            {
                return m_width;
            }
        }

        public Dimension Height
        {
            get
            {
                return m_height;
            }
        }

        public Dimension LargestDimension
        {
            get
            {
                return Width > Height ? Width : Height;
            }
        }

        public Dimension SmallestDimension
        {
            get
            {
                return Width < Height ? Width : Height;
            }
        }

        public bool Contains(Area area)
        {
            return area.Width <= Width && area.Height <= Height ? true : area.Width <= Height && area.Height <= Width;
        }

        public bool Matches(Area area)
        {
            return
                (area.Width == Width && area.Height == Height) ||
                (area.Width == Height && area.Height == Width);
        }

        public Area Round()
        {
            return new Area(m_width.Round(), m_height.Round());
        }
    }
}
