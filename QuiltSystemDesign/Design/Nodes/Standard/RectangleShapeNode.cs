//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using Newtonsoft.Json.Linq;

using RichTodd.QuiltSystem.Design.Path;
using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Nodes.Standard
{
    [Node(PathGeometryNames.RECTANGLE)]
    public class RectangleShapeNode : ShapeNode
    {
        public RectangleShapeNode(FabricStyle fabricStyle) : base(fabricStyle, PathGeometries.Rectangle)
        { }

        public RectangleShapeNode(JToken json) : base(json)
        { }

        protected RectangleShapeNode(RectangleShapeNode prototype) : base(prototype)
        { }

        public override Node Clone()
        {
            return new RectangleShapeNode(this);
        }
    }
}