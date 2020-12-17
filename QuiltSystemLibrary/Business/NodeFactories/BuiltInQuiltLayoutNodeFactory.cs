//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

using RichTodd.QuiltSystem.Business.ComponentProviders;
using RichTodd.QuiltSystem.Design.Component;
using RichTodd.QuiltSystem.Design.Component.Standard;
using RichTodd.QuiltSystem.Design.Nodes;
using RichTodd.QuiltSystem.Design.Nodes.Standard;

namespace RichTodd.QuiltSystem.Business.NodeFactories
{
    public class BuiltInQuiltLayoutNodeFactory : INodeFactory
    {
        private readonly Dictionary<string, LayoutStyleConstructor> m_layoutStyleConstructors;

        public BuiltInQuiltLayoutNodeFactory()
        {
            m_layoutStyleConstructors = new Dictionary<string, LayoutStyleConstructor>
            {
                {
                    BuiltInQuiltLayoutComponenProvider.ComponentName_Checkerboard,
                    (int rowCount, int columnCount) => RepeatHorizontally(rowCount, columnCount, new int[] { 0, 1 }, 1)
                },
                {
                    BuiltInQuiltLayoutComponenProvider.ComponentName_HorizontalStripes1,
                    (int rowCount, int columnCount) => RepeatHorizontally(rowCount, columnCount, new int[] { 0, 1 }, 0)
                },
                {
                    BuiltInQuiltLayoutComponenProvider.ComponentName_HorizontalStripes2,
                    (int rowCount, int columnCount) => RepeatHorizontally(rowCount, columnCount, new int[] { 0, 1, 2 }, 0)
                },
                {
                    BuiltInQuiltLayoutComponenProvider.ComponentName_HorizontalStripes3,
                    (int rowCount, int columnCount) => RepeatHorizontally(rowCount, columnCount, new int[] { 0, 1, 2, 1 }, 0)
                },
                {
                    BuiltInQuiltLayoutComponenProvider.ComponentName_HorizontalStripes4,
                    (int rowCount, int columnCount) => RepeatHorizontally(rowCount, columnCount, new int[] { 0, 1, 2 }, 1)
                },
                {
                    BuiltInQuiltLayoutComponenProvider.ComponentName_HorizontalStripes5,
                    (int rowCount, int columnCount) => RepeatHorizontally(rowCount, columnCount, new int[] { 0, 1, 2, 1 }, 1)
                },
                {
                    BuiltInQuiltLayoutComponenProvider.ComponentName_VerticalStripes,
                    (int rowCount, int columnCount) => RepeatVertically(rowCount, columnCount, new int[] { 0, 1 }, 0)
                },
                {
                    BuiltInQuiltLayoutComponenProvider.ComponentName_Radial1,
                    (int rowCount, int columnCount) => CopyRadial(rowCount, columnCount, new int[] { 0, 1 })
                },
                {
                    BuiltInQuiltLayoutComponenProvider.ComponentName_Radial2,
                    (int rowCount, int columnCount) => CopyRadial(rowCount, columnCount, new int[] { 0, 1, 0 })
                }
            };
        }

        private delegate int[,] LayoutStyleConstructor(int rowCount, int columnCount);

        public Node Create(Component component, NodeList childNodes)
        {
            if (component == null) throw new ArgumentNullException(nameof(component));

            if (component.Type != LayoutComponent.TypeName) return null;
            if (component.Category != Constants.DefaultComponentCategory) return null;

            var rowCount = int.Parse(component.Parameters["RowCount"]);
            var columnCount = int.Parse(component.Parameters["ColumnCount"]);

            var layoutStyle = GetLayoutStyle(component.Name, rowCount, columnCount);

            var gridLayout = new GridLayoutNode(rowCount, columnCount)
            {
                ComponentType = component.Type,
                ComponentName = component.Name
            };

            for (var row = 0; row < gridLayout.RowCount; ++row)
            {
                for (var column = 0; column < gridLayout.ColumnCount; ++column)
                {
                    var colorIndex = layoutStyle[row, column];

                    var layoutSite = gridLayout.GetLayoutSite(row, column);
                    layoutSite.Style = colorIndex.ToString();

                    if (childNodes != null && childNodes.Count > 0)
                    {
                        var childNode = childNodes[colorIndex % childNodes.Count].Clone();
                        layoutSite.Node = childNode;
                    }
                }
            }

            return gridLayout;
        }

        private int[,] CopyRadial(int rowCount, int columnCount, int[] pattern)
        {
            var styles = new int[rowCount, columnCount];

            var patternLength = pattern.Length;

            for (var row = 0; row < rowCount; ++row)
            {
                for (var column = 0; column < columnCount; ++column)
                {
                    var distance = GetDistanceFromEdge(rowCount, columnCount, row, column);
                    var idx = Math.Min(distance, patternLength - 1);
                    styles[row, column] = pattern[idx];
                }
            }

            return styles;
        }

        private int GetDistanceFromEdge(int rowCount, int columnCount, int row, int column)
        {
            var top = row;
            var bottom = rowCount - 1 - row;
            var left = column;
            var right = columnCount - 1 - column;

            var result = Math.Min(Math.Min(Math.Min(top, bottom), left), right);

            return result;
        }

        private int[,] GetLayoutStyle(string componentName, int rowCount, int columnCount)
        {
            return m_layoutStyleConstructors.ContainsKey(componentName)
                ? m_layoutStyleConstructors[componentName](rowCount, columnCount)
                : throw new InvalidOperationException(string.Format("Unknown componentName {0}.", componentName));
        }

#pragma warning disable IDE0051 // Remove unused private members
        private int GetRadius(int rowCount, int columnCount)
#pragma warning restore IDE0051 // Remove unused private members
        {
            var minDimension = Math.Min(rowCount, columnCount);

            return (minDimension / 2) + (minDimension % 2);
        }

        private int[,] RepeatHorizontally(int rowCount, int columnCount, int[] pattern, int rowOffset)
        {
            var styles = new int[rowCount, columnCount];

            var patternLength = pattern.Length;

            for (var row = 0; row < rowCount; ++row)
            {
                for (var column = 0; column < columnCount; ++column)
                {
                    var idx = (column + (row * rowOffset)) % patternLength;
                    styles[row, column] = pattern[idx];
                }
            }

            return styles;
        }

        private int[,] RepeatVertically(int rowCount, int columnCount, int[] pattern, int columnOffset)
        {
            var styles = new int[rowCount, columnCount];

            var patternLength = pattern.Length;

            for (var row = 0; row < rowCount; ++row)
            {
                for (var column = 0; column < columnCount; ++column)
                {
                    var idx = (row + (column * columnOffset)) % patternLength;
                    styles[row, column] = pattern[idx];
                }
            }

            return styles;
        }
    }
}