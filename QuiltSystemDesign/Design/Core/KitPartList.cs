//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace RichTodd.QuiltSystem.Design.Core
{
    public class KitPartList : List<KitPart>
    {
        public KitPartList()
        { }

        public KitPartList(JToken json)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));

            foreach (var jsonKitPart in json)
            {
                Add(new KitPart(jsonKitPart));
            }
        }

        protected KitPartList(IList<KitPart> prototype)
        {
            if (prototype == null) throw new ArgumentNullException(nameof(prototype));

            foreach (var kitPart in prototype)
            {
                Add(kitPart.Clone());
            }
        }

        public KitPartList Clone()
        {
            return new KitPartList(this);
        }

        public JToken JsonSave()
        {
            var jsonKitParts = new JArray();
            foreach (var kitPart in this)
            {
                jsonKitParts.Add(kitPart.JsonSave());
            }

            return jsonKitParts;
        }
    }
}