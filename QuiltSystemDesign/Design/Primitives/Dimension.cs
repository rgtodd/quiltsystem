//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Design.Primitives
{
    // Immutable type
    //
    public struct Dimension : IComparable<Dimension>, IEquatable<Dimension>
    {
        private readonly double m_value;
        private readonly DimensionUnits m_unit;

        public Dimension(double value)
        {
            m_unit = DimensionUnits.Pixel; // equal to default value 0.
            m_value = value;
        }

        public Dimension(double value, DimensionUnits unit)
        {
            m_value = value;
            m_unit = unit;
        }

        public double Value
        {
            get
            {
                return m_value;
            }
        }

        public DimensionUnits Unit
        {
            get
            {
                return m_unit;
            }
        }

        public Dimension Round()
        {
            var value = (int)Math.Round(Value * 10000.0);
            var roundedValue = value / 10000.0;

            return new Dimension(roundedValue, Unit);
        }

        public static bool operator ==(Dimension lhs, Dimension rhs)
        {
            return lhs.Unit == rhs.Unit && lhs.Value == rhs.Value;
        }

        public static bool operator !=(Dimension lhs, Dimension rhs)
        {
            return !(lhs.Unit == rhs.Unit && lhs.Value == rhs.Value);
        }

        public static bool operator <(Dimension lhs, Dimension rhs)
        {
            return lhs.Unit != rhs.Unit 
                ? throw new InvalidOperationException("Unit mismatch.") 
                : lhs.Value < rhs.Value;
        }

        public static bool operator >(Dimension lhs, Dimension rhs)
        {
            return lhs.Unit != rhs.Unit 
                ? throw new InvalidOperationException("Unit mismatch.") 
                : lhs.Value > rhs.Value;
        }

        public static bool operator <=(Dimension lhs, Dimension rhs)
        {
            return lhs.Unit != rhs.Unit 
                ? throw new InvalidOperationException("Unit mismatch.") 
                : lhs.Value <= rhs.Value;
        }

        public static bool operator >=(Dimension lhs, Dimension rhs)
        {
            return lhs.Unit != rhs.Unit 
                ? throw new InvalidOperationException("Unit mismatch.") 
                : lhs.Value >= rhs.Value;
        }

        public static Dimension operator -(Dimension lhs)
        {
            return new Dimension(-lhs.Value, lhs.Unit);
        }

        public static Dimension operator +(Dimension lhs, Dimension rhs)
        {
            return lhs.Unit != rhs.Unit 
                ? throw new InvalidOperationException("Unit mismatch.") 
                : new Dimension(lhs.Value + rhs.Value, lhs.Unit);
        }

        public static Dimension operator +(Dimension lhs, double rhs)
        {
            return new Dimension(lhs.Value + rhs, lhs.Unit);
        }

        public static Dimension operator +(Dimension lhs, int rhs)
        {
            return new Dimension(lhs.Value + rhs, lhs.Unit);
        }

        public static Dimension operator -(Dimension lhs, Dimension rhs)
        {
            return lhs.Unit != rhs.Unit 
                ? throw new InvalidOperationException("Unit mismatch.") 
                : new Dimension(lhs.Value - rhs.Value, lhs.Unit);
        }

        public static Dimension operator -(Dimension lhs, double rhs)
        {
            return new Dimension(lhs.Value - rhs, lhs.Unit);
        }

        public static Dimension operator -(Dimension lhs, int rhs)
        {
            return new Dimension(lhs.Value - rhs, lhs.Unit);
        }

        public static Dimension operator *(Dimension lhs, Dimension rhs)
        {
            return lhs.Unit != rhs.Unit 
                ? throw new InvalidOperationException("Unit mismatch.") 
                : new Dimension(lhs.Value * rhs.Value, lhs.Unit);
        }

        public static Dimension operator *(Dimension lhs, double rhs)
        {
            return new Dimension(lhs.Value * rhs, lhs.Unit);
        }

        public static Dimension operator *(Dimension lhs, int rhs)
        {
            return new Dimension(lhs.Value * rhs, lhs.Unit);
        }

        public static Dimension operator *(Dimension lhs, DimensionScale rhs)
        {
            return lhs.Unit == rhs.FromUnit
                ? new Dimension(lhs.Value * rhs.ToValue / rhs.FromValue, rhs.ToUnit)
                : lhs.Unit == rhs.ToUnit
                    ? new Dimension(lhs.Value * rhs.FromValue / rhs.ToValue, rhs.FromUnit)
                    : throw new InvalidOperationException("Unit mismatch.");
        }

        public static Dimension operator /(Dimension lhs, Dimension rhs)
        {
            return lhs.Unit != rhs.Unit 
                ? throw new InvalidOperationException("Unit mismatch.") 
                : new Dimension(lhs.Value / rhs.Value, lhs.Unit);
        }

        public static Dimension operator /(Dimension lhs, double rhs)
        {
            return new Dimension(lhs.Value / rhs, lhs.Unit);
        }

        public static Dimension operator /(Dimension lhs, int rhs)
        {
            return new Dimension(lhs.Value / rhs, lhs.Unit);
        }

        public bool Equals(Dimension other)
        {
            return Unit == other.Unit && Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (!(obj is Dimension)) return false;

            var other = (Dimension)obj;

            return Unit == other.Unit && Value == other.Value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(m_value);
        }

        public int CompareTo(Dimension other)
        {
            return Unit != other.Unit 
                ? throw new InvalidOperationException("Unit mismatch.") 
                : Value.CompareTo(other.Value);
        }

        public override string ToString()
        {
            switch (m_unit)
            {
                case DimensionUnits.Inch:
                    {
                        return m_value.ToString() + @"""";
                    }

                case DimensionUnits.Pixel:
                    {
                        return m_value.ToString() + "px";
                    }

                default:
                    throw new InvalidOperationException(string.Format("Unknown unit of measure {0}.", m_unit));
            }
        }

        public static Dimension? ParseNullable(string value)
        {
            return string.IsNullOrEmpty(value) ? null : (Dimension?)Parse(value);
        }

        public static Dimension Parse(string value)
        {
            return string.IsNullOrEmpty(value)
                ? throw new ArgumentNullException(nameof(value))
                : value.EndsWith(@"""")
                    ? new Dimension(double.Parse(value[0..^1]), DimensionUnits.Inch)
                    : value.EndsWith("px")
                        ? new Dimension(double.Parse(value[0..^2]), DimensionUnits.Pixel)
                        : new Dimension(double.Parse(value), DimensionUnits.Pixel);
        }
    }
}
