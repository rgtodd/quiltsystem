//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace RichTodd.QuiltSystem.Design.Primitives
{
    class ThemeEntryList : List<ThemeEntry>
    {
        public ThemeEntryList()
        { }

        public ThemeEntryList(JToken json)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));

            foreach (var jsonThemeEntry in json)
            {
                Add(new ThemeEntry(jsonThemeEntry));
            }
        }

        protected ThemeEntryList(IList<ThemeEntry> prototype)
        {
            if (prototype == null) throw new ArgumentNullException(nameof(prototype));

            foreach (var themeEntry in prototype)
            {
                Add(themeEntry.Clone());
            }
        }

        public ThemeEntryList Clone()
        {
            return new ThemeEntryList(this);
        }

        public JToken JsonSave()
        {
            var jsonThemeEntries = new JArray();
            foreach (var themeEntry in this)
            {
                jsonThemeEntries.Add(themeEntry.JsonSave());
            }

            return jsonThemeEntries;
        }
    }
}