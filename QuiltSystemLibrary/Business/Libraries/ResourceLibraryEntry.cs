//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

using Newtonsoft.Json.Linq;

namespace RichTodd.QuiltSystem.Business.Libraries
{
    public class ResourceLibraryEntry
    {
        private readonly string m_name;
        private readonly string m_type;
        private readonly string m_data;
        private readonly string[] m_tags;

        public ResourceLibraryEntry(string name, string type, string data, string[] tags)
        {
            if (string.IsNullOrEmpty(type)) throw new ArgumentNullException(nameof(type));
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrEmpty(data)) throw new ArgumentNullException(nameof(data));

            m_name = name;
            m_data = data;
            m_type = type;
            m_tags = tags;
        }

        public ResourceLibraryEntry(JToken json)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));

            m_name = json.Value<string>("Name");
            m_type = json.Value<string>("Type");
            m_data = json.Value<string>("Data");

            var tags = json["Tags"];
            m_tags = tags?.ToObject<string[]>();
        }

        public string Name
        {
            get { return m_name; }
        }

        public string Type
        {
            get { return m_type; }
        }

        public string Data
        {
            get { return m_data; }
        }

        public string[] Tags
        {
            get { return m_tags; }
        }

        public JToken JsonSave()
        {
            var result = new JObject()
            {
                new JProperty("Name", Name),
                new JProperty("Type", Type),
                new JProperty("Data", Data),
                new JProperty("Tags", Tags)
            };

            return result;
        }
    }
}