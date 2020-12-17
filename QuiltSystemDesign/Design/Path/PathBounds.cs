//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Diagnostics.CodeAnalysis;

using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Path
{
    public struct PathBounds : IEquatable<PathBounds>
    {
        private readonly Dimension m_minimumX;
        private readonly Dimension m_maximumX;
        private readonly Dimension m_minimumY;
        private readonly Dimension m_maximumY;

        public PathBounds(Dimension minimumX, Dimension maximumX, Dimension minimumY, Dimension maximumY)
        {
            m_minimumX = minimumX;
            m_maximumX = maximumX;
            m_minimumY = minimumY;
            m_maximumY = maximumY;
        }

        public bool Empty
        {
            get
            {
                return MinimumX == MaximumX && MinimumY == MaximumY;
            }
        }

        public Dimension MinimumX
        {
            get
            {
                return m_minimumX;
            }
        }

        public Dimension MaximumX
        {
            get
            {
                return m_maximumX;
            }
        }

        public Dimension MinimumY
        {
            get
            {
                return m_minimumY;
            }
        }

        public Dimension MaximumY
        {
            get
            {
                return m_maximumY;
            }
        }

        public bool Contains(PathPoint point)
        {
            return
                point.X >= MinimumX &&
                point.X <= MaximumX &&
                point.Y >= MinimumY &&
                point.Y <= MaximumY;
        }

        public override bool Equals(object obj)
        {
            return obj is PathBounds bounds && Equals(bounds);
        }

        public bool Equals([AllowNull] PathBounds other)
        {
            return m_minimumX.Equals(other.m_minimumX) &&
                   m_maximumX.Equals(other.m_maximumX) &&
                   m_minimumY.Equals(other.m_minimumY) &&
                   m_maximumY.Equals(other.m_maximumY);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(m_minimumX, m_maximumX, m_minimumY, m_maximumY);
        }

        public PathBounds Shift(int dx, int dy)
        {
            return new PathBounds(
                new Dimension(MinimumX.Value + dx, MinimumX.Unit),
                new Dimension(MaximumX.Value + dx, MaximumX.Unit),
                new Dimension(MinimumY.Value + dy, MinimumY.Unit),
                new Dimension(MaximumY.Value + dy, MaximumY.Unit));
        }

        public static bool operator ==(PathBounds left, PathBounds right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PathBounds left, PathBounds right)
        {
            return !(left == right);
        }
    }
}
