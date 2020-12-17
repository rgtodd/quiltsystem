//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

namespace RichTodd.QuiltSystem.Design.Primitives
{
    // Note: class is immutable.
    //
    public class FabricStyle : IComparable<FabricStyle>, IEquatable<FabricStyle>
    {
        public const string UNKNOWN_SKU = "UNKNOWN";

        public FabricStyle(string sku, Color color)
        {
            Sku = sku;
            Color = color;
        }

        public FabricStyle(Color color)
        {
            Sku = UNKNOWN_SKU;
            Color = color;
        }

        public FabricStyle(JToken json)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));

            Sku = (string)json[JsonNames.Sku];
            Color = Color.FromArgb((int)json[JsonNames.Color]);
        }

        protected FabricStyle(FabricStyle prototype)
        {
            if (prototype == null) throw new ArgumentNullException(nameof(prototype));

            Sku = prototype.Sku;
            Color = prototype.Color;
        }

        public static FabricStyle Default { get; } = new FabricStyle(UNKNOWN_SKU, Color.White);

        public Color Color { get; }

        public string Sku { get; }

        public FabricStyle Clone()
        {
            return new FabricStyle(this);
        }

        public int CompareTo(FabricStyle other)
        {
            if (other == null) return 1;

            var result = Sku.CompareTo(other.Sku);

            if (result == 0)
            {
                result = Color.ToRgb().CompareTo(other.Color.ToRgb());
            }

            return result;
        }

        public JToken JsonSave()
        {
            var result = new JObject()
            {
                new JProperty(JsonNames.Sku, Sku),
                new JProperty(JsonNames.Color, Color.ToRgb())
            };

            return result;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as FabricStyle);
        }

        public bool Equals([AllowNull] FabricStyle other)
        {
            return other != null &&
                   EqualityComparer<Color>.Default.Equals(Color, other.Color) &&
                   Sku == other.Sku;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Color, Sku);
        }

        public static bool operator ==(FabricStyle left, FabricStyle right)
        {
            return EqualityComparer<FabricStyle>.Default.Equals(left, right);
        }

        public static bool operator !=(FabricStyle left, FabricStyle right)
        {
            return !(left == right);
        }

        public static bool operator <(FabricStyle left, FabricStyle right)
        {
            return left is null ? right is object : left.CompareTo(right) < 0;
        }

        public static bool operator <=(FabricStyle left, FabricStyle right)
        {
            return left is null || left.CompareTo(right) <= 0;
        }

        public static bool operator >(FabricStyle left, FabricStyle right)
        {
            return left is object && left.CompareTo(right) > 0;
        }

        public static bool operator >=(FabricStyle left, FabricStyle right)
        {
            return left is null ? right is null : left.CompareTo(right) >= 0;
        }
    }
}