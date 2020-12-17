//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

using RichTodd.QuiltSystem.Design.Nodes;
using RichTodd.QuiltSystem.Design.Nodes.Standard;
using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Core
{
    public static class TextPatternParser
    {
        public static Node Parse(IList<string> lines)
        {
            int size = int.Parse(lines[0]);
            int styleCount = int.Parse(lines[1]);

            var patternLineOffset = 2;
            var styleLineOffset = patternLineOffset + size;

            var gridLayout = new GridLayoutNode(size, size);
            for (int column = 0; column < size; ++column)
            {
                for (int row = 0; row < size; ++row)
                {
                    var cellDefinition = lines[row + patternLineOffset].Substring(column, 1);
                    var styleDefinition = lines[row + styleLineOffset].Substring(column * styleCount, styleCount);

                    var layoutSite = gridLayout.GetLayoutSite(row, column);
                    PopulateLayoutSite(layoutSite, cellDefinition, styleDefinition);
                }
            }

            return gridLayout;
        }

        private static void PopulateLayoutSite(LayoutSite layoutSite, string cellDefinition, string styleDefinition)
        {
            switch (cellDefinition)
            {
                case "*":
                    {
                        var style = styleDefinition.Substring(0, 1);

                        layoutSite.Node = new RectangleShapeNode(FabricStyle.Default) { Style = style };
                    }
                    return;

                case "/":
                    {
                        var node = CreateHalfSquareTriangleLayoutNode(
                            styleDefinition.Substring(0, 1),
                            styleDefinition.Substring(1, 1));

                        layoutSite.Node = node;
                    }
                    return;

                case "\\":
                    {
                        var node = CreateHalfSquareTriangleLayoutNode(
                            styleDefinition.Substring(0, 1),
                            styleDefinition.Substring(1, 1));

                        layoutSite.Node = node;
                        layoutSite.PathOrientation.PointOffset = 1;
                    }
                    return;

                default:
                    throw new InvalidOperationException(string.Format("Unknown cell format {0}", cellDefinition));
            }
        }

        private static HalfSquareTriangleLayoutNode CreateHalfSquareTriangleLayoutNode(string style1, string style2)
        {
            var layout = new HalfSquareTriangleLayoutNode();

            layout.LayoutSites[0].Node = new TriangleShapeNode(FabricStyle.Default) { Style = style1 };
            layout.LayoutSites[1].Node = new TriangleShapeNode(FabricStyle.Default) { Style = style2 };

            return layout;
        }
    }
}
