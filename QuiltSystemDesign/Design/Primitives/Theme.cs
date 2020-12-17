//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace RichTodd.QuiltSystem.Design.Primitives
{
    public class Theme : ITheme
    {
        private string m_name;
        private readonly ThemeEntryList m_entries;

        public Theme(string name)
        {
            m_name = name ?? throw new ArgumentNullException(nameof(name));
            m_entries = new ThemeEntryList();
        }

        public Theme(JToken json)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));

            m_name = (string)json[JsonNames.Name];
            m_entries = new ThemeEntryList(json[JsonNames.ThemeEntries]);
        }

        protected Theme(Theme prototype)
        {
            if (prototype == null) throw new ArgumentNullException(nameof(prototype));

            m_name = prototype.m_name;
            m_entries = prototype.m_entries.Clone();
        }

        public virtual JToken JsonSave()
        {
            var result = new JObject()
            {
                new JProperty(JsonNames.Name, m_name),
                new JProperty(JsonNames.ThemeEntries, m_entries.JsonSave())
            };

            return result;
        }

        public Theme Clone()
        {
            return new Theme(this);
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

        public IList<ThemeEntry> Entries
        {
            get
            {
                return m_entries;
            }
        }

        public IPalette GetPalette(int index)
        {
            return Entries[index % Entries.Count].Palette;
        }
    }
}
