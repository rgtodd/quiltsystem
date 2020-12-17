//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace RichTodd.QuiltSystem.Design.Primitives
{
    public class Palette : IPalette
    {
        private readonly PaletteEntryList m_entries;
        private string m_name;

        public Palette(string name)
        {
            m_name = name ?? throw new ArgumentNullException(nameof(name));
            m_entries = new PaletteEntryList();
        }

        public Palette(JToken json)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));

            m_name = (string)json[JsonNames.Name];
            m_entries = new PaletteEntryList(json[JsonNames.PaletteEntries]);
        }

        protected Palette(Palette prototype)
        {
            if (prototype == null) throw new ArgumentNullException(nameof(prototype));

            m_name = prototype.m_name;
            m_entries = prototype.m_entries.Clone();
        }

        public List<PaletteEntry> Entries
        {
            get
            {
                return m_entries;
            }
        }

        public string Name
        {
            get
            {
                return m_name;
            }

            set
            {
                m_name = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        public static Palette Create(string name, double hue, int count)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (hue < 0 || hue > 360) throw new ArgumentOutOfRangeException(nameof(hue));
            if (count < 2) throw new ArgumentOutOfRangeException(nameof(count));

            var palette = new Palette(name);

            var brightnessDelta = 0.5 / (count - 1);
            for (var idx = 0; idx < count; ++idx)
            {
                var brightness = 1.0 - (brightnessDelta * idx);
                var color = Color.FromAhsb(255, hue, 1.0, brightness);

                var paletteEntry = new PaletteEntry(new FabricStyle(color));

                palette.Entries.Add(paletteEntry);
            }

            return palette;
        }

        public Palette Clone()
        {
            return new Palette(this);
        }

        public FabricStyle GetFabricStyle(int index)
        {
            return Entries[index % Entries.Count].FabricStyle;
        }

        public virtual JToken JsonSave()
        {
            var result = new JObject()
            {
                new JProperty(JsonNames.Name, m_name),
                new JProperty(JsonNames.PaletteEntries, m_entries.JsonSave())
            };

            return result;
        }
    }
}