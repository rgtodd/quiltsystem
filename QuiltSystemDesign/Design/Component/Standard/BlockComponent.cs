//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

using Newtonsoft.Json.Linq;

using RichTodd.QuiltSystem.Design.Nodes;
using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Component.Standard
{
    [Component(TypeName)]
    public class BlockComponent : Component
    {
        public const string TypeName = "Block";

        private static Configuration s_configuration;

        public static void Configure(INodeFactory nodeFactory)
        {
            s_configuration = new Configuration()
            {
                NodeFactory = nodeFactory
            };
        }

        public static BlockComponent Create(string category, string name, FabricStyleList fabricStyles)
        {
            return new BlockComponent(category, name, fabricStyles, null);
        }

        private BlockComponent(string category, string name, FabricStyleList fabricStyles, ComponentParameterCollection parameters)
            : base(TypeName, category, name, fabricStyles, parameters)
        { }

        public BlockComponent(JToken json) : base(json)
        {
            if (Type != TypeName)
            {
                throw new ArgumentException("TypeName attribute mismatch.", nameof(json));
            }
        }

        protected BlockComponent(BlockComponent prototype) : base(prototype)
        { }

        public override Component Clone()
        {
            return new BlockComponent(this);
        }

        public override JToken JsonSave()
        {
            var result = base.JsonSave();

            result[JsonNames.TypeName] = TypeName;

            return result;
        }

        public override Node Expand(bool includeChildren)
        {
            var node = s_configuration.NodeFactory.Create(this, null);

            return node;
        }

        private class Configuration
        {
            public INodeFactory NodeFactory { get; set; }
        }
    }
}
