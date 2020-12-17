//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Design.Primitives
{
    public class DimensionScale
    {
        private readonly double m_fromValue;
        private readonly DimensionUnits m_fromUnit;
        private readonly double m_toValue;
        private readonly DimensionUnits m_toUnit;

        public DimensionScale(double fromValue, DimensionUnits fromUnit, double toValue, DimensionUnits toUnit)
        {
            m_fromValue = fromValue;
            m_fromUnit = fromUnit;
            m_toValue = toValue;
            m_toUnit = toUnit;
        }

        public static DimensionScale CreateIdentity(DimensionUnits unit)
        {
            return new DimensionScale(1, unit, 1, unit);
        }

        public double FromValue
        {
            get
            {
                return m_fromValue;
            }
        }

        public DimensionUnits FromUnit
        {
            get
            {
                return m_fromUnit;
            }
        }

        public double ToValue
        {
            get
            {
                return m_toValue;
            }
        }

        public DimensionUnits ToUnit
        {
            get
            {
                return m_toUnit;
            }
        }
    }
}
