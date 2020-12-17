//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace RichTodd.QuiltSystem.Design.Core
{
    public class PatternList : List<Pattern>
    {
        public PatternList()
        { }

        public PatternList(JToken json)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));

            foreach (var jsonPattern in json)
            {
                Add(new Pattern(jsonPattern));
            }
        }

        protected PatternList(IList<Pattern> prototype)
        {
            if (prototype == null) throw new ArgumentNullException(nameof(prototype));

            foreach (var node in prototype)
            {
                Add(node.Clone());
            }
        }

        public PatternList Clone()
        {
            return new PatternList(this);
        }

        public JToken JsonSave()
        {
            var jsonPatterns = new JArray();
            foreach (var pattern in this)
            {
                jsonPatterns.Add(pattern.JsonSave());
            }

            return jsonPatterns;
        }
    }
}