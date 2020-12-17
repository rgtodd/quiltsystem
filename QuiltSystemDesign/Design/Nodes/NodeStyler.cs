//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Nodes
{
    public static class NodeStyler
    {
        public static void Style(Node node, ITheme theme)
        {
            Style(node, theme, string.Empty);
        }

        public static void Style(Node node, IEnumerable<FabricStyle> fabricStyles)
        {
            var palette = new Palette("temp");
            foreach (var fabricStyle in fabricStyles)
            {
                palette.Entries.Add(new PaletteEntry(fabricStyle));
            }

            var theme = new Theme("temp");
            theme.Entries.Add(new ThemeEntry(palette));

            Style(node, theme);
        }

        private static string AppendStyle(string stylePath, string style)
        {
            if (style == null)
            {
                return stylePath;
            }

            stylePath = !string.IsNullOrEmpty(stylePath)
                ? stylePath + "/" + style
                : style;

            return stylePath;
        }

        private static FabricStyle GetFabricStyle(ITheme theme, string stylePath)
        {
            string[] styles = stylePath.Split(new char[] { '/' });

            // Lookup Palette
            //
            IPalette palette;
            {
                int entryIndex;
                if (styles.Length >= 2)
                {
                    if (!int.TryParse(styles[0], out entryIndex))
                    {
                        entryIndex = 0;
                    }
                }
                else
                {
                    entryIndex = 0;
                }

                palette = theme.GetPalette(entryIndex);
            }

            // Lookup FabricStyle
            //
            FabricStyle fabricStyle;
            {
                int entryIndex;
                if (styles.Length >= 2)
                {
                    if (!int.TryParse(styles[1], out entryIndex))
                    {
                        entryIndex = 0;
                    }
                }
                else if (styles.Length >= 1)
                {
                    if (!int.TryParse(styles[0], out entryIndex))
                    {
                        entryIndex = 0;
                    }
                }
                else
                {
                    entryIndex = 0;
                }

                fabricStyle = palette.GetFabricStyle(entryIndex);
            }

            return fabricStyle;
        }

        private static void Style(Node node, ITheme theme, string stylePath)
        {
            stylePath = AppendStyle(stylePath, node.Style);

            if (node is ShapeNode shapeNode)
            {
                Style(shapeNode, theme, stylePath);
            }
            else if (node is LayoutNode layoutNode)
            {
                Style(layoutNode, theme, stylePath);
            }
            else
            {
                throw new InvalidOperationException(string.Format("Unknown node type {0}", node.GetType().Name));
            }
        }

        private static void Style(ShapeNode shapeNode, ITheme theme, string stylePath)
        {
            shapeNode.FabricStyle = GetFabricStyle(theme, stylePath);
        }

        private static void Style(LayoutNode layoutNode, ITheme theme, string stylePath)
        {
            foreach (var layoutSite in layoutNode.LayoutSites)
            {
                if (layoutSite.Node != null)
                {
                    var layoutSylePath = AppendStyle(stylePath, layoutSite.Style);
                    Style(layoutSite.Node, theme, layoutSylePath);
                }
            }
        }
    }
}