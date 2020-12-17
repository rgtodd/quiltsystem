//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace RichTodd.QuiltSystem.Design.Component
{
    public class ComponentList : List<Component>
    {
        public ComponentList()
        { }

        public ComponentList(JToken json)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));

            foreach (var jsonComponent in json)
            {
                Add(ComponentFactory.Singleton.Create(jsonComponent));
            }
        }

        protected ComponentList(IList<Component> prototype)
        {
            if (prototype == null) throw new ArgumentNullException(nameof(prototype));

            foreach (var component in prototype)
            {
                Add(component.Clone());
            }
        }

        public static ComponentList Create(JToken json)
        {
            return json != null ? new ComponentList(json) : null;
        }

        public ComponentList Clone()
        {
            return new ComponentList(this);
        }

        public JToken JsonSave()
        {
            var jsonComponents = new JArray();
            foreach (var component in this)
            {
                jsonComponents.Add(component.JsonSave());
            }

            return jsonComponents;
        }
    }
}