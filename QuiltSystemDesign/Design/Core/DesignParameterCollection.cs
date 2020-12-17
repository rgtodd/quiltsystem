//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace RichTodd.QuiltSystem.Design.Core
{
    public class DesignParameterCollection
    {
        private readonly Dictionary<string, string> m_parameters;

        public DesignParameterCollection()
        {
            m_parameters = new Dictionary<string, string>();
        }

        public DesignParameterCollection(IDictionary<string, string> parameters)
        {
            if (parameters == null)
            {
                m_parameters = new Dictionary<string, string>();
            }
            else
            {
                m_parameters = new Dictionary<string, string>(parameters);
            }
        }

        public DesignParameterCollection(JToken json)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));

            m_parameters = new Dictionary<string, string>();
            foreach (JProperty jsonProperty in ((JObject)json).Properties())
            {
                m_parameters.Add(jsonProperty.Name, (string)jsonProperty.Value);
            }
        }

        protected DesignParameterCollection(DesignParameterCollection prototype)
        {
            if (prototype == null) throw new ArgumentNullException(nameof(prototype));

            m_parameters = new Dictionary<string, string>(prototype.m_parameters);
        }

        public JToken JsonSave()
        {
            var result = new JObject();

            foreach (var key in m_parameters.Keys)
            {
                result[key] = m_parameters[key];
            }

            return result;
        }

        public DesignParameterCollection Clone()
        {
            return new DesignParameterCollection(this);
        }

        public string this[string index]
        {
            get
            {
                if (m_parameters.ContainsKey(index))
                {
                    return m_parameters[index];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                m_parameters[index] = value;
            }
        }
    }
}
