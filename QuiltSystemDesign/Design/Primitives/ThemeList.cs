//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace RichTodd.QuiltSystem.Design.Primitives
{
    class ThemeList : List<Theme>
    {
        public ThemeList()
        { }

        public ThemeList(JToken json)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));

            foreach (var jsonTheme in json)
            {
                Add(new Theme(jsonTheme));
            }
        }

        protected ThemeList(IList<Theme> prototype)
        {
            if (prototype == null) throw new ArgumentNullException(nameof(prototype));

            foreach (var theme in prototype)
            {
                Add(theme.Clone());
            }
        }

        public ThemeList Clone()
        {
            return new ThemeList(this);
        }

        public JToken JsonSave()
        {
            var jsonThemes = new JArray();
            foreach (var theme in this)
            {
                jsonThemes.Add(theme.JsonSave());
            }

            return jsonThemes;
        }
    }
}