//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Drawing;

using Newtonsoft.Json.Linq;

using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Core
{
    public class KitBuildItemPattern : KitBuildItem
    {
        public const string TypeName = "Pattern";

        private readonly Pattern m_pattern;

        public KitBuildItemPattern(string id, int quantity, string name, string style, FabricStyleList fabricStyles, Area area, Pattern pattern)
            : base(TypeName, id, quantity, name, style, fabricStyles, area)
        {
            m_pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
        }

        public KitBuildItemPattern(JToken json) : base(json)
        {
            if (!string.IsNullOrEmpty(Type)) // BUG: Assume existing items are KitNodeBuildItems.
            {
                if (Type != TypeName)
                {
                    throw new ArgumentException("TypeName attribute mismatch.", nameof(json));
                }
            }

            m_pattern = new Pattern(json[JsonNames.Pattern]);
        }

        protected KitBuildItemPattern(KitBuildItemPattern prototype) : base(prototype)
        {
            m_pattern = prototype.m_pattern.Clone();
        }

        public override KitBuildItem Clone()
        {
            return new KitBuildItemPattern(this);
        }

        public override Image CreateImage(DimensionScale scale)
        {
            var renderer = new PatternRenderer();
            return renderer.CreateBitmap(m_pattern, scale);
        }

        public override JToken JsonSave()
        {
            var result = base.JsonSave();

            result[JsonNames.TypeName] = TypeName;
            result[JsonNames.Pattern] = m_pattern.JsonSave();

            return result;
        }
    }
}