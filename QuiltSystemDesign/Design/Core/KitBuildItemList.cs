//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace RichTodd.QuiltSystem.Design.Core
{
    public class KitBuildItemList : List<KitBuildItem>
    {
        public KitBuildItemList()
        { }

        public KitBuildItemList(JToken json)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));

            foreach (var jsonKitBuildItem in json)
            {
                Add(KitBuildItem.Create(jsonKitBuildItem));
            }
        }

        protected KitBuildItemList(IList<KitBuildItem> prototype)
        {
            if (prototype == null) throw new ArgumentNullException(nameof(prototype));

            foreach (var kitBuildItem in prototype)
            {
                Add(kitBuildItem.Clone());
            }
        }

        public KitBuildItemList Clone()
        {
            return new KitBuildItemList(this);
        }

        public JToken JsonSave()
        {
            var jsonKitBuildItems = new JArray();
            foreach (var kitBuildItem in this)
            {
                jsonKitBuildItems.Add(kitBuildItem.JsonSave());
            }

            return jsonKitBuildItems;
        }
    }
}