//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

using Newtonsoft.Json.Linq;

using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Core
{
    public class Pattern
    {
        private readonly Area m_fabricSize;
        private readonly PatternElementList m_patternElements;

        public Pattern(Area fabricSize)
        {
            m_fabricSize = fabricSize ?? throw new ArgumentNullException(nameof(fabricSize));
            m_patternElements = new PatternElementList();
        }

        public Pattern(JToken json)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));

            var width = Dimension.Parse((string)json[JsonNames.Width]);
            var height = Dimension.Parse((string)json[JsonNames.Height]);
            m_fabricSize = new Area(width, height);
            m_patternElements = new PatternElementList(json[JsonNames.PatternElementList]);
        }

        protected Pattern(Pattern prototype)
        {
            if (prototype == null) throw new ArgumentNullException(nameof(prototype));

            m_fabricSize = prototype.m_fabricSize;
            m_patternElements = prototype.m_patternElements.Clone();
        }

        public Pattern Clone()
        {
            return new Pattern(this);
        }

        public JToken JsonSave()
        {
            var result = new JObject()
            {
                new JProperty(JsonNames.Width, m_fabricSize.Width.ToString()),
                new JProperty(JsonNames.Height, m_fabricSize.Height.ToString()),
                new JProperty(JsonNames.PatternElementList, m_patternElements.JsonSave())
            };

            return result;
        }

        public Area FabricSize
        {
            get
            {
                return m_fabricSize;
            }
        }

        public PatternElementList PatternElements
        {
            get
            {
                return m_patternElements;
            }
        }
    }
}
