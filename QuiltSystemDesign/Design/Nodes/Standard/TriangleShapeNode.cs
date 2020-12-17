//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using Newtonsoft.Json.Linq;

using RichTodd.QuiltSystem.Design.Path;
using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Nodes.Standard
{
    [Node(PathGeometryNames.TRIANGLE)]
    internal class TriangleShapeNode : ShapeNode
    {
        public TriangleShapeNode(FabricStyle fabricStyle) : base(fabricStyle, PathGeometries.Triangle)
        { }

        public TriangleShapeNode(JToken json) : base(json)
        { }

        protected TriangleShapeNode(TriangleShapeNode prototype) : base(prototype)
        { }

        public override Node Clone()
        {
            return new TriangleShapeNode(this);
        }
    }
}