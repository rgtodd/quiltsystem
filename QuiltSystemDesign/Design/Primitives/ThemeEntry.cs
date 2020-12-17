//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

using Newtonsoft.Json.Linq;

namespace RichTodd.QuiltSystem.Design.Primitives
{
    public class ThemeEntry
    {
        private Palette m_palette;

        public ThemeEntry(Palette palette)
        {
            m_palette = palette ?? throw new ArgumentNullException(nameof(palette));
        }

        public ThemeEntry(JToken json)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));

            m_palette = new Palette(json[JsonNames.Palette]);
        }

        protected ThemeEntry(ThemeEntry prototype)
        {
            if (prototype == null) throw new ArgumentNullException(nameof(prototype));

            m_palette = prototype.m_palette.Clone();
        }

        public ThemeEntry Clone()
        {
            return new ThemeEntry(this);
        }

        public JToken JsonSave()
        {
            var result = new JObject()
            {
                new JProperty(JsonNames.Palette, m_palette.JsonSave())
            };

            return result;
        }

        public Palette Palette
        {
            get
            {
                return m_palette;
            }

            set
            {
                m_palette = value ?? throw new ArgumentNullException(nameof(value));
            }
        }
    }
}
