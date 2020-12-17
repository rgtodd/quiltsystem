//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json.Linq;

namespace RichTodd.QuiltSystem.Business.Libraries
{
    internal class MemoryResourceLibrary : AbstractResourceLibrary
    {
        private readonly string m_name;
        private readonly List<ResourceLibraryEntry> m_entries;

        public MemoryResourceLibrary(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            m_name = name;
            m_entries = new List<ResourceLibraryEntry>();
        }

        public MemoryResourceLibrary(JToken json)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));

            m_name = json.Value<string>(Json_Name);
            m_entries = new List<ResourceLibraryEntry>();

            foreach (var jsonEntry in json[Json_Entries])
            {
                m_entries.Add(new ResourceLibraryEntry(jsonEntry));
            }
        }

        public override string Name
        {
            get
            {
                return m_name;
            }
        }

        public override IReadOnlyList<ResourceLibraryEntry> GetEntries()
        {
            return m_entries;
        }

        public override ResourceLibraryEntry GetEntry(string name)
        {
            return m_entries.Where(r => r.Name == name).SingleOrDefault();
        }

        public override void CreateEntry(string name, string type, string data, string[] tags)
        {
            if (m_entries.Exists(r => r.Name == name))
            {
                throw new InvalidOperationException(string.Format("Entry {0} already exists.", name));
            }

            m_entries.Add(new ResourceLibraryEntry(name, type, data, tags));
        }

        public override void UpdateEntry(string name, string data, string[] tags)
        {
            var entry = m_entries.Where(r => r.Name == name).Single();
            _ = m_entries.Remove(entry);
            m_entries.Add(new ResourceLibraryEntry(entry.Name, entry.Type, data, tags));
        }
    }
}