//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace RichTodd.QuiltSystem.Design.Primitives
{
    class PaletteEntryList : List<PaletteEntry>
    {
        public PaletteEntryList()
        { }

        public PaletteEntryList(JToken json)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));

            foreach (var jsonPaletteEntry in json)
            {
                Add(new PaletteEntry(jsonPaletteEntry));
            }
        }

        protected PaletteEntryList(IList<PaletteEntry> prototype)
        {
            if (prototype == null) throw new ArgumentNullException(nameof(prototype));

            foreach (var paletteEntry in prototype)
            {
                Add(paletteEntry.Clone());
            }
        }

        public PaletteEntryList Clone()
        {
            return new PaletteEntryList(this);
        }

        public JToken JsonSave()
        {
            var jsonPaletteEntries = new JArray();
            foreach (var paletteEntry in this)
            {
                jsonPaletteEntries.Add(paletteEntry.JsonSave());
            }

            return jsonPaletteEntries;
        }
    }
}