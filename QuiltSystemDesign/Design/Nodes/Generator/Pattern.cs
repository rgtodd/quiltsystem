//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace RichTodd.QuiltSystem.Design.Nodes.Generator
{
    internal class Pattern<T> where T : Enum
    {
        private readonly T[,] m_values;

        public Pattern(T[,] values)
        {
            m_values = values;
        }

        public int ColumnCount
        {
            get { return m_values.GetLength(1); }
        }

        public int RowCount
        {
            get { return m_values.GetLength(0); }
        }

        public T[,] Values
        {
            get { return m_values; }
        }

        public IEnumerable<T> GetAllValues()
        {
            for (var row = 0; row < RowCount; ++row)
            {
                for (var column = 0; column < ColumnCount; ++column)
                {
                    yield return m_values[row, column];
                }
            }
        }

        public string GetSignature()
        {
            var rowCount = RowCount;
            var columnCount = ColumnCount;

            var sb = new StringBuilder();

            for (var row = 0; row < rowCount; ++row)
            {
                for (var column = 0; column < columnCount; ++column)
                {
                    var value = ((IConvertible)m_values[row, column]).ToInt64(CultureInfo.CurrentCulture.NumberFormat);
                    _ = row == 0 && column == 0
                        ? sb.Append(value)
                        : sb.Append("-").Append(value);
                }
            }

            return sb.ToString();
        }

        public static Pattern<T> CreatePattern(Fingerprint<T> fingerprint, FingerprintMapper<T> fingerprintMapper)
        {
            var rowCount = fingerprintMapper.RowCount;
            var columnCount = fingerprintMapper.ColumnCount;

            var values = new T[rowCount, columnCount];

            for (var row = 0; row < rowCount; ++row)
            {
                for (var column = 0; column < columnCount; ++column)
                {
                    values[row, column] = fingerprintMapper.Map(fingerprint, row, column);
                }
            }

            return new Pattern<T>(values);
        }

        public static bool operator !=(Pattern<T> lhs, Pattern<T> rhs)
        {
            return ((object)lhs) == null || ((object)rhs) == null ? !Equals(lhs, rhs) : !lhs.Equals(rhs);
        }

        public static bool operator ==(Pattern<T> lhs, Pattern<T> rhs)
        {
            return ((object)lhs) == null || ((object)rhs) == null ? Equals(lhs, rhs) : lhs.Equals(rhs);
        }

        public bool Equals(Pattern<T> other)
        {
            if (other == null)
            {
                return false;
            }

            var rowCount = RowCount;
            var columnCount = ColumnCount;

            if (rowCount != other.RowCount || columnCount != other.ColumnCount)
            {
                return false;
            }

            for (var row = 0; row < rowCount; ++row)
            {
                for (var column = 0; column < columnCount; ++column)
                {
                    if (!Values[row, column].Equals(other.Values[row, column]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public override bool Equals(object other)
        {
            if (other == null)
            {
                return false;
            }

            var otherPattern = other as Pattern<T>;

            return otherPattern != null && Equals(otherPattern);
        }

        public override int GetHashCode()
        {
            var result = 0;

            foreach (var value in m_values)
            {
                result ^= value.GetHashCode();
            }

            return result;
        }
    }
}