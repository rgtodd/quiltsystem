//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

using Microsoft.Extensions.Configuration;

using Newtonsoft.Json.Linq;

using RichTodd.QuiltSystem.Utility;

namespace RichTodd.QuiltSystem.Business.Libraries
{
    public abstract class AbstractResourceLibrary : IResourceLibrary
    {
        protected const string Json_Entries = "Entries";
        protected const string Json_Name = "Name";

        public abstract string Name { get; }

        public static JToken AzureLoad(IConfiguration configuration, string name)
        {
            var text = AzureUtility.LoadAzureStringBlob(configuration, "libraries", string.Format("{0}.json", name));

            var json = JToken.Parse(text);

            return json;
        }

        public static void AzureSave(IConfiguration configuration, JToken json)
        {
            var name = json.Value<string>(Json_Name);

            AzureUtility.SaveAzureStringBlob(configuration, "libraries", string.Format("{0}.json", name), json.ToString());
        }

        public abstract IReadOnlyList<ResourceLibraryEntry> GetEntries();

        public abstract ResourceLibraryEntry GetEntry(string name);

        public abstract void CreateEntry(string name, string type, string data, string[] tags);

        public abstract void UpdateEntry(string name, string data, string[] tags);

        public JToken JsonSave()
        {
            var jsonEntries = new JArray();
            foreach (var entry in GetEntries())
            {
                jsonEntries.Add(entry.JsonSave());
            }

            var jsonResult = new JObject()
            {
                new JProperty(Json_Name, Name),
                new JProperty(Json_Entries, jsonEntries)
            };

            return jsonResult;
        }
    }
}