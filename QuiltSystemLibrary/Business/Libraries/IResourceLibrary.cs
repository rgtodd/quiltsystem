//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace RichTodd.QuiltSystem.Business.Libraries
{
    internal interface IResourceLibrary
    {
        string Name { get; }

        IReadOnlyList<ResourceLibraryEntry> GetEntries();
        ResourceLibraryEntry GetEntry(string name);
        void CreateEntry(string name, string type, string data, string[] tags);
        void UpdateEntry(string name, string data, string[] tags);

        JToken JsonSave();
    }
}