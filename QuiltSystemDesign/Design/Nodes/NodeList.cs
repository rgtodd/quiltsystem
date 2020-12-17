//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace RichTodd.QuiltSystem.Design.Nodes
{
    public class NodeList : List<Node>
    {
        public NodeList()
        { }

        public NodeList(JToken json)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));

            foreach (var jsonNode in json)
            {
                Add(NodeFactory.Singleton.Create(jsonNode));
            }
        }

        protected NodeList(IList<Node> prototype)
        {
            if (prototype == null) throw new ArgumentNullException(nameof(prototype));

            foreach (var node in prototype)
            {
                Add(node.Clone());
            }
        }

        public NodeList Clone()
        {
            return new NodeList(this);
        }

        public JToken JsonSave()
        {
            var jsonNodes = new JArray();
            foreach (var node in this)
            {
                jsonNodes.Add(node.JsonSave());
            }

            return jsonNodes;
        }
    }
}