//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Path
{
    public struct PathPoint : IEquatable<PathPoint>
    {
        public static PathPoint Invalid = new PathPoint(true);

        public PathPoint(Dimension x, Dimension y)
        {
            X = x;
            Y = y;
            IsInvalid = false;
        }

        private PathPoint(bool invalid)
        {
            X = new Dimension(0, DimensionUnits.Pixel);
            Y = new Dimension(0, DimensionUnits.Pixel);
            IsInvalid = invalid;
        }

        public PathPoint(JToken json)
        {
            X = Dimension.Parse((string)json[JsonNames.X]);
            Y = Dimension.Parse((string)json[JsonNames.Y]);
            IsInvalid = false;
        }

        public JToken JsonSave()
        {
            var result = new JObject()
            {
                new JProperty(JsonNames.X, X.ToString()),
                new JProperty(JsonNames.Y, Y.ToString())
            };

            return result;
        }

        public PathPoint Round()
        {
            if (IsInvalid)
            {
                return this;
            }
            else
            {
                return new PathPoint(X.Round(), Y.Round());
            }
        }

        public override bool Equals(object obj)
        {
            return obj is PathPoint point && Equals(point);
        }

        public bool Equals([AllowNull] PathPoint other)
        {
            return IsInvalid == other.IsInvalid &&
                   X.Equals(other.X) &&
                   Y.Equals(other.Y);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(IsInvalid, X, Y);
        }

        public Dimension X { get; }

        public Dimension Y { get; }

        public bool IsInvalid { get; }

        public static bool operator ==(PathPoint left, PathPoint right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PathPoint left, PathPoint right)
        {
            return !(left == right);
        }
    }
}
