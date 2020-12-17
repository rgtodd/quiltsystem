//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

using Newtonsoft.Json.Linq;

using RichTodd.QuiltSystem.Design.Nodes;
using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Component
{
    public abstract class Component
    {
        private readonly string m_category;
        private readonly FabricStyleList m_fabricStyles;
        private readonly string m_name;
        private readonly ComponentParameterCollection m_parameters;
        private readonly string m_type;

        protected Component(string type, string category, string name, FabricStyleList fabricStyles, ComponentParameterCollection parameters)
        {
            if (string.IsNullOrEmpty(type)) throw new ArgumentNullException(nameof(type));
            if (string.IsNullOrEmpty(category)) throw new ArgumentNullException(nameof(category));
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            m_type = type;
            m_category = category;
            m_name = name;
            m_parameters = parameters != null ? parameters.Clone() : new ComponentParameterCollection();
            m_fabricStyles = fabricStyles != null ? fabricStyles.Clone() : new FabricStyleList();
        }

        protected Component(JToken json)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));

            m_type = (string)json[JsonNames.TypeName];
            m_category = (string)json[JsonNames.Category];
            m_name = (string)json[JsonNames.Name];
            m_parameters = new ComponentParameterCollection(json[JsonNames.Parameters]);
            m_fabricStyles = new FabricStyleList(json[JsonNames.FabricStyleList]);
        }

        protected Component(Component prototype)
        {
            if (prototype == null) throw new ArgumentNullException(nameof(prototype));

            m_type = prototype.m_type;
            m_name = prototype.m_name;
            m_category = prototype.Category;
            m_parameters = prototype.m_parameters.Clone();
            m_fabricStyles = prototype.m_fabricStyles.Clone();
        }

        public string Category
        {
            get
            {
                return m_category;
            }
        }

        public FabricStyleList FabricStyles
        {
            get
            {
                return m_fabricStyles;
            }
        }

        public string Name
        {
            get
            {
                return m_name;
            }
        }

        public ComponentParameterCollection Parameters
        {
            get
            {
                return m_parameters;
            }
        }

        public string Type
        {
            get
            {
                return m_type;
            }
        }

        public abstract Component Clone();

        public abstract Node Expand(bool includeChildren);

        public virtual JToken JsonSave()
        {
            var result = new JObject()
            {
                new JProperty(JsonNames.TypeName, m_type),
                new JProperty(JsonNames.Category, m_category),
                new JProperty(JsonNames.Name, m_name),
                new JProperty(JsonNames.Parameters, m_parameters.JsonSave()),
                new JProperty(JsonNames.FabricStyleList, m_fabricStyles.JsonSave())
            };

            return result;
        }
    }
}