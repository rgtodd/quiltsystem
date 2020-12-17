//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

using Newtonsoft.Json.Linq;

using RichTodd.QuiltSystem.Design.Component.Standard;
using RichTodd.QuiltSystem.Design.Core;
using RichTodd.QuiltSystem.Design.Nodes;

namespace RichTodd.QuiltSystem.Business.Libraries
{
    internal static class ResourceNodeFactory
    {
        public static Node Create(ResourceLibraryEntry entry)
        {
            Node node;
            if (entry.Data.StartsWith("{"))
            {
                node = NodeFactory.Singleton.Create(JToken.Parse(entry.Data));
            }
            else
            {
                var lines = entry.Data.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                node = TextPatternParser.Parse(lines);
            }

            node.ComponentType = BlockComponent.TypeName;
            node.ComponentName = entry.Name;

            return node;
        }
    }
}