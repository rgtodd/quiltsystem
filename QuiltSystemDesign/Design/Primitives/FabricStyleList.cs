//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace RichTodd.QuiltSystem.Design.Primitives
{
    public class FabricStyleList : List<FabricStyle>
    {
        public FabricStyleList()
        { }

        public FabricStyleList(JToken json)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));

            foreach (var jsonFabricStyle in json)
            {
                Add(new FabricStyle(jsonFabricStyle));
            }
        }

        protected FabricStyleList(FabricStyleList prototype)
        {
            if (prototype == null) throw new ArgumentNullException(nameof(prototype));

            foreach (var node in prototype)
            {
                Add(node.Clone());
            }
        }

        public FabricStyleList Clone()
        {
            return new FabricStyleList(this);
        }

        public JToken JsonSave()
        {
            var jsonFabricStyles = new JArray();
            foreach (var fabricStyle in this)
            {
                jsonFabricStyles.Add(fabricStyle.JsonSave());
            }

            return jsonFabricStyles;
        }
    }
}