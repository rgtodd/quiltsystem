//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace RichTodd.QuiltSystem.Design.Core
{
    public class PatternElementList : List<PatternElement>
    {
        public PatternElementList()
        { }

        public PatternElementList(JToken json)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));

            foreach (var jsonPatternElement in json)
            {
                Add(new PatternElement(jsonPatternElement));
            }
        }

        protected PatternElementList(IList<PatternElement> prototype)
        {
            if (prototype == null) throw new ArgumentNullException(nameof(prototype));

            foreach (var node in prototype)
            {
                Add(node.Clone());
            }
        }

        public PatternElementList Clone()
        {
            return new PatternElementList(this);
        }

        public JToken JsonSave()
        {
            var jsonPatternElements = new JArray();
            foreach (var patternElement in this)
            {
                jsonPatternElements.Add(patternElement.JsonSave());
            }

            return jsonPatternElements;
        }
    }
}