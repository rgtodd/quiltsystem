//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Drawing;

using Newtonsoft.Json.Linq;

using RichTodd.QuiltSystem.Design.Nodes;
using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Core
{
    public class KitBuildItemNode : KitBuildItem
    {
        public const string TypeName = "Node";

        private readonly Node m_node;

        public KitBuildItemNode(string id, int quantity, string name, string style, FabricStyleList fabricStyles, Area area, Node node)
            : base(TypeName, id, quantity, name, style, fabricStyles, area)
        {
            m_node = node;
        }

        public KitBuildItemNode(JToken json) : base(json)
        {
            if (!string.IsNullOrEmpty(Type)) // BUG: Assume existing items are KitNodeBuildItems.
            {
                if (Type != TypeName)
                {
                    //throw new ArgumentException("TypeName attribute mismatch.", nameof(json));
                }
            }

            var jsonNode = json[JsonNames.Node];
            if (jsonNode != null && jsonNode.HasValues)
            {
                m_node = NodeFactory.Singleton.Create(jsonNode);
            }
        }

        protected KitBuildItemNode(KitBuildItemNode prototype) : base(prototype)
        {
            m_node = prototype.m_node?.Clone();
        }

        public override KitBuildItem Clone()
        {
            return new KitBuildItemNode(this);
        }

        public override Image CreateImage(DimensionScale scale)
        {
            if (m_node == null)
            {
                return null;
            }

            var renderer = new DesignRenderer();
            return renderer.CreateBitmap(m_node, scale, false);
        }

        public override JToken JsonSave()
        {
            var result = base.JsonSave();

            result[JsonNames.TypeName] = TypeName;
            result[JsonNames.Node] = m_node?.JsonSave();

            return result;
        }
    }
}