//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RichTodd.QuiltSystem.Design.Nodes.Standard;
using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Nodes.Generator
{
    public class HalfSquareTriangleNodeGenerator : INodeGenerator
    {
        public ICollection<NodeGeneratorItem> Generate()
        {
            var items = new List<NodeGeneratorItem>();

            var patterns = new Dictionary<string, Pattern<Patches>>();

            foreach (var fingerprint in GetFingerprints())
            {
                var pattern = GetCanonical(Pattern<Patches>.CreatePattern(fingerprint, Resources.MirrorFingerprintMapper));
                var signature = pattern.GetSignature();
                _ = patterns.TryAdd(signature, pattern);
            }

            foreach (var fingerprint in GetFingerprints())
            {
                var pattern = GetCanonical(Pattern<Patches>.CreatePattern(fingerprint, Resources.PinwheelFingerprintMapper));
                var signature = pattern.GetSignature();
                _ = patterns.TryAdd(signature, pattern);
            }

            int count = 0;
            foreach (var signature in patterns.Keys.OrderBy(r => r))
            {
                var pattern = patterns[signature];
                var tags = GetTags(pattern);
                var category = GetCategory(tags);

                var name = $"{category}/Block {++count:0000}";

                items.Add(
                    new NodeGeneratorItem()
                    {
                        Name = name,
                        Node = CreateNode(pattern),
                        Tags = tags.ToArray()
                    });
            }

            return items;
        }

        private string GetCategory(IEnumerable<string> tags)
        {
            var sb = new StringBuilder();

            foreach (var tag in tags)
            {
                if (sb.Length > 0)
                {
                    _ = sb.Append(" - ");
                }
                _ = sb.Append(tag);
            }

            return sb.ToString();
        }

        private static Node CreateNode(Pattern<Patches> pattern)
        {
            int rowCount = pattern.RowCount;
            int columnCount = pattern.ColumnCount;

            GridLayoutNode gridLayoutNode = new GridLayoutNode(rowCount * 2, columnCount * 2);

            Styles styles = new Styles();

            for (int row = 0; row < rowCount; ++row)
            {
                for (int column = 0; column < columnCount; ++column)
                {
                    var layoutRow = row * 2;
                    var layoutColumn = column * 2;

                    switch (pattern.Values[row, column])
                    {
                        case Patches.Rectangle1:
                            {
                                var layoutSite = gridLayoutNode.GetLayoutSite(layoutRow, layoutColumn);
                                gridLayoutNode.SetRowSpan(layoutRow, layoutColumn, 2);
                                gridLayoutNode.SetColumnSpan(layoutRow, layoutColumn, 2);

                                layoutSite.Node = CreateRectangleNode(styles.Rectangle1Style);
                            }
                            break;

                        case Patches.Rectangle2:
                            {
                                var layoutSite = gridLayoutNode.GetLayoutSite(layoutRow, layoutColumn);
                                gridLayoutNode.SetRowSpan(layoutRow, layoutColumn, 2);
                                gridLayoutNode.SetColumnSpan(layoutRow, layoutColumn, 2);

                                layoutSite.Node = CreateRectangleNode(styles.Rectangle2Style);
                            }
                            break;

                        case Patches.HalfSquareTriangle:
                            {
                                var layoutSite = gridLayoutNode.GetLayoutSite(layoutRow, layoutColumn);
                                gridLayoutNode.SetRowSpan(layoutRow, layoutColumn, 2);
                                gridLayoutNode.SetColumnSpan(layoutRow, layoutColumn, 2);

                                layoutSite.Node = CreateHalfSquareTriangleNode(styles.HalfSquareTriangleStyle1, styles.HalfSquareTriangleStyle2);
                            }
                            break;

                        case Patches.HalfSquareTriangle90:
                            {
                                var layoutSite = gridLayoutNode.GetLayoutSite(layoutRow, layoutColumn);
                                gridLayoutNode.SetRowSpan(layoutRow, layoutColumn, 2);
                                gridLayoutNode.SetColumnSpan(layoutRow, layoutColumn, 2);

                                layoutSite.Node = CreateHalfSquareTriangleNode(styles.HalfSquareTriangleStyle1, styles.HalfSquareTriangleStyle2);
                                layoutSite.PathOrientation.PointOffset = 3;
                            }
                            break;

                        case Patches.HalfSquareTriangle180:
                            {
                                var layoutSite = gridLayoutNode.GetLayoutSite(layoutRow, layoutColumn);
                                gridLayoutNode.SetRowSpan(layoutRow, layoutColumn, 2);
                                gridLayoutNode.SetColumnSpan(layoutRow, layoutColumn, 2);

                                layoutSite.Node = CreateHalfSquareTriangleNode(styles.HalfSquareTriangleStyle1, styles.HalfSquareTriangleStyle2);
                                layoutSite.PathOrientation.PointOffset = 2;
                            }
                            break;

                        case Patches.HalfSquareTriangle270:
                            {
                                var layoutSite = gridLayoutNode.GetLayoutSite(layoutRow, layoutColumn);
                                gridLayoutNode.SetRowSpan(layoutRow, layoutColumn, 2);
                                gridLayoutNode.SetColumnSpan(layoutRow, layoutColumn, 2);

                                layoutSite.Node = CreateHalfSquareTriangleNode(styles.HalfSquareTriangleStyle1, styles.HalfSquareTriangleStyle2);
                                layoutSite.PathOrientation.PointOffset = 1;
                            }
                            break;


                        case Patches.SplitRectangle:
                            {
                                // Left Patch
                                {
                                    var layoutSite = gridLayoutNode.GetLayoutSite(layoutRow, layoutColumn);
                                    gridLayoutNode.SetRowSpan(layoutRow, layoutColumn, 2);
                                    layoutSite.Node = CreateRectangleNode(styles.SplitRectangleStyle1);
                                }

                                // Right Patch
                                {
                                    var layoutSite = gridLayoutNode.GetLayoutSite(layoutRow, layoutColumn + 1);
                                    gridLayoutNode.SetRowSpan(layoutRow, layoutColumn + 1, 2);
                                    layoutSite.Node = CreateRectangleNode(styles.SplitRectangleStyle2);
                                }
                            }
                            break;

                        case Patches.SplitRectangle90:
                            {
                                // Top Patch
                                {
                                    var layoutSite = gridLayoutNode.GetLayoutSite(layoutRow, layoutColumn);
                                    gridLayoutNode.SetColumnSpan(layoutRow, layoutColumn, 2);
                                    layoutSite.Node = CreateRectangleNode(styles.SplitRectangleStyle1);
                                }

                                // Bottom Patch
                                {
                                    var layoutSite = gridLayoutNode.GetLayoutSite(layoutRow + 1, layoutColumn);
                                    gridLayoutNode.SetColumnSpan(layoutRow + 1, layoutColumn, 2);
                                    layoutSite.Node = CreateRectangleNode(styles.SplitRectangleStyle2);
                                }
                            }
                            break;

                        case Patches.SplitRectangle180:
                            {
                                // Right Patch
                                {
                                    var layoutSite = gridLayoutNode.GetLayoutSite(layoutRow, layoutColumn + 1);
                                    gridLayoutNode.SetRowSpan(layoutRow, layoutColumn + 1, 2);
                                    layoutSite.Node = CreateRectangleNode(styles.SplitRectangleStyle1);
                                }

                                // Left Patch
                                {
                                    var layoutSite = gridLayoutNode.GetLayoutSite(layoutRow, layoutColumn);
                                    gridLayoutNode.SetRowSpan(layoutRow, layoutColumn, 2);
                                    layoutSite.Node = CreateRectangleNode(styles.SplitRectangleStyle2);
                                }
                            }
                            break;

                        case Patches.SplitRectangle270:
                            {
                                // Bottom Patch
                                {
                                    var layoutSite = gridLayoutNode.GetLayoutSite(layoutRow + 1, layoutColumn);
                                    gridLayoutNode.SetColumnSpan(layoutRow + 1, layoutColumn, 2);
                                    layoutSite.Node = CreateRectangleNode(styles.SplitRectangleStyle1);
                                }

                                // Top Patch
                                {
                                    var layoutSite = gridLayoutNode.GetLayoutSite(layoutRow, layoutColumn);
                                    gridLayoutNode.SetColumnSpan(layoutRow, layoutColumn, 2);
                                    layoutSite.Node = CreateRectangleNode(styles.SplitRectangleStyle2);
                                }
                            }
                            break;

                        default:
                            throw new InvalidOperationException(string.Format("Unknown patch {0}", pattern.Values[row, column]));
                    }
                }
            }

            return gridLayoutNode.GetSimplified();
        }

        private static RectangleShapeNode CreateRectangleNode(string style)
        {
            return new RectangleShapeNode(FabricStyle.Default) { Style = style };
        }

        private static HalfSquareTriangleLayoutNode CreateHalfSquareTriangleNode(string style1, string style2)
        {
            var layout = new HalfSquareTriangleLayoutNode();

            layout.LayoutSites[0].Node = new TriangleShapeNode(FabricStyle.Default) { Style = style1 };
            layout.LayoutSites[1].Node = new TriangleShapeNode(FabricStyle.Default) { Style = style2 };

            return layout;
        }

        private static IEnumerable<Fingerprint<Patches>> GetFingerprints()
        {
            foreach (var patch1 in GetPatches())
            {
                foreach (var patch2 in GetPatches())
                {
                    foreach (var patch3 in GetPatches())
                    {
                        var patches = new Patches[] { patch1, patch2, patch3 };
                        yield return new Fingerprint<Patches>(patches);
                    }
                }
            }
        }

        private static IEnumerable<Patches> GetPatches()
        {
            yield return Patches.Rectangle1;
            yield return Patches.Rectangle2;
            yield return Patches.HalfSquareTriangle;
            yield return Patches.HalfSquareTriangle90;
            yield return Patches.HalfSquareTriangle180;
            yield return Patches.HalfSquareTriangle270;
            yield return Patches.SplitRectangle;
            yield return Patches.SplitRectangle90;
            yield return Patches.SplitRectangle180;
            yield return Patches.SplitRectangle270;
        }

        private bool IsPinwheel(Pattern<Patches> pattern)
        {
            return pattern == RotateRight(pattern);
        }

        private bool IsMirror(Pattern<Patches> pattern)
        {
            return pattern == FlipHorizontally(pattern) && pattern == FlipVertically(pattern);
        }

        private Pattern<Patches> GetCanonical(Pattern<Patches> pattern)
        {
            string signature;
            var canonicalPattern = pattern;
            var canonicalSignature = canonicalPattern.GetSignature();

            for (int idx = 0; idx < 3; ++idx)
            {
                pattern = RotateRight(pattern);
                signature = pattern.GetSignature();
                if (signature.CompareTo(canonicalSignature) < 0)
                {
                    canonicalPattern = pattern;
                    canonicalSignature = signature;
                }
            }

            pattern = SwapHalfSquareTriangles(canonicalPattern);
            signature = pattern.GetSignature();
            if (signature.CompareTo(canonicalSignature) < 0)
            {
                canonicalPattern = pattern;
                canonicalSignature = signature;
            }

            pattern = SwapRectangles(canonicalPattern);
            signature = pattern.GetSignature();
            if (signature.CompareTo(canonicalSignature) < 0)
            {
                canonicalPattern = pattern;
                canonicalSignature = signature;
            }

            pattern = SwapSplitRectangles(canonicalPattern);
            signature = pattern.GetSignature();
            if (signature.CompareTo(canonicalSignature) < 0)
            {
                canonicalPattern = pattern;
                //canonicalSignature = signature;
            }

            return canonicalPattern;
        }

        private IEnumerable<string> GetTags(Pattern<Patches> pattern)
        {
            var hasHalfSquareTriangle = false;
            var hasRectangle = false;
            var hasSplitRectangle = false;
            foreach (var patch in pattern.GetAllValues())
            {
                switch (patch)
                {
                    case Patches.HalfSquareTriangle:
                    case Patches.HalfSquareTriangle90:
                    case Patches.HalfSquareTriangle180:
                    case Patches.HalfSquareTriangle270:
                        hasHalfSquareTriangle = true;
                        break;

                    case Patches.Rectangle1:
                    case Patches.Rectangle2:
                        hasRectangle = true;
                        break;

                    case Patches.SplitRectangle:
                    case Patches.SplitRectangle90:
                    case Patches.SplitRectangle180:
                    case Patches.SplitRectangle270:
                        hasSplitRectangle = true;
                        break;

                    default: // Ignore
                        break;
                }
            }

            var tags = new List<string>();
            if (hasHalfSquareTriangle)
            {
                tags.Add("Half Square Triangle");
            }
            if (hasRectangle)
            {
                tags.Add("Solid");
            }
            if (hasSplitRectangle)
            {
                tags.Add("Split Rectangle");
            }
            if (IsPinwheel(pattern))
            {
                tags.Add("Pinwheel");
            }
            if (IsMirror(pattern))
            {
                tags.Add("Mirror");
            }

            return tags;
        }

        // [0,0] [0,1] [0,2] [0,3]
        //
        // [1,0] [1,1] [1,2] [1,3]
        //
        // [2,0] [2,1] [2,2] [2,3]
        //
        // [3,0] [3,1] [3,2] [3,3]
        //
        private static Pattern<Patches> RotateRight(Pattern<Patches> pattern)
        {
            var values = new Patches[4, 4];

            for (int row = 0; row < 4; ++row)
            {
                for (int column = 0; column < 4; ++column)
                {
                    int fromRow = 3 - column;
                    int fromColumn = row;
                    values[row, column] = Resources.RotateRightPatchMapper.Map(pattern.Values[fromRow, fromColumn]);
                }
            }

            return new Pattern<Patches>(values);
        }

        private static Pattern<Patches> FlipHorizontally(Pattern<Patches> pattern)
        {
            var values = new Patches[4, 4];

            for (int row = 0; row < 4; ++row)
            {
                for (int column = 0; column < 4; ++column)
                {
                    int fromRow = row;
                    int fromColumn = 3 - column;
                    values[row, column] = Resources.HorizontalMirrorPatchMapper.Map(pattern.Values[fromRow, fromColumn]);
                }
            }

            return new Pattern<Patches>(values);
        }

        private static Pattern<Patches> FlipVertically(Pattern<Patches> pattern)
        {
            var values = new Patches[4, 4];

            for (int row = 0; row < 4; ++row)
            {
                for (int column = 0; column < 4; ++column)
                {
                    int fromRow = 3 - row;
                    int fromColumn = column;
                    values[row, column] = Resources.VerticalMirrorPatchMapper.Map(pattern.Values[fromRow, fromColumn]);
                }
            }

            return new Pattern<Patches>(values);
        }

        private static Pattern<Patches> SwapRectangles(Pattern<Patches> pattern)
        {
            var rowCount = pattern.RowCount;
            var columnCount = pattern.ColumnCount;

            var values = new Patches[rowCount, columnCount];
            for (int row = 0; row < rowCount; ++row)
            {
                for (int column = 0; column < columnCount; ++column)
                {
                    values[row, column] = Resources.SwapRectanglesPatchMapper.Map(pattern.Values[row, column]);
                }
            }

            var swappedPattern = new Pattern<Patches>(values);
            return swappedPattern;
        }

        private static Pattern<Patches> SwapHalfSquareTriangles(Pattern<Patches> pattern)
        {
            var rowCount = pattern.RowCount;
            var columnCount = pattern.ColumnCount;

            var values = new Patches[rowCount, columnCount];
            for (int row = 0; row < rowCount; ++row)
            {
                for (int column = 0; column < columnCount; ++column)
                {
                    values[row, column] = Resources.SwapHalfSquareTrianglesPatchMapper.Map(pattern.Values[row, column]);
                }
            }

            var swappedPattern = new Pattern<Patches>(values);
            return swappedPattern;
        }

        private static Pattern<Patches> SwapSplitRectangles(Pattern<Patches> pattern)
        {
            var rowCount = pattern.RowCount;
            var columnCount = pattern.ColumnCount;

            var values = new Patches[rowCount, columnCount];
            for (int row = 0; row < rowCount; ++row)
            {
                for (int column = 0; column < columnCount; ++column)
                {
                    values[row, column] = Resources.SwapSplitRectanglesPatchMapper.Map(pattern.Values[row, column]);
                }
            }

            var swappedPattern = new Pattern<Patches>(values);
            return swappedPattern;
        }

        private enum Patches
        {
            Rectangle1,
            Rectangle2,
            HalfSquareTriangle, // 0/1
            HalfSquareTriangle90, // 1\0
            HalfSquareTriangle180, // 1/0
            HalfSquareTriangle270, // 0\1
            SplitRectangle,
            SplitRectangle90,
            SplitRectangle180,
            SplitRectangle270
        }

        private class Styles
        {
            private int m_idxStyle = 0;
            private string m_halfSquareTriangleStyle1 = null;
            private string m_halfSquareTriangleStyle2 = null;
            private string m_rectangle1Style = null;
            private string m_rectangle2Style = null;
            private string m_splitRectangleStyle1 = null;
            private string m_splitRectangleStyle2 = null;

            public string HalfSquareTriangleStyle1
            {
                get
                {
                    if (m_halfSquareTriangleStyle1 == null)
                    {
                        m_halfSquareTriangleStyle1 = (++m_idxStyle).ToString();
                    }
                    return m_halfSquareTriangleStyle1;
                }
            }

            public string HalfSquareTriangleStyle2
            {
                get
                {
                    if (m_halfSquareTriangleStyle2 == null)
                    {
                        m_halfSquareTriangleStyle2 = (++m_idxStyle).ToString();
                    }
                    return m_halfSquareTriangleStyle2;
                }
            }

            public string Rectangle1Style
            {
                get
                {
                    if (m_rectangle1Style == null)
                    {
                        m_rectangle1Style = (++m_idxStyle).ToString();
                    }
                    return m_rectangle1Style;
                }
            }

            public string Rectangle2Style
            {
                get
                {
                    if (m_rectangle2Style == null)
                    {
                        m_rectangle2Style = (++m_idxStyle).ToString();
                    }
                    return m_rectangle2Style;
                }
            }

            public string SplitRectangleStyle1
            {
                get
                {
                    if (m_splitRectangleStyle1 == null)
                    {
                        m_splitRectangleStyle1 = (++m_idxStyle).ToString();
                    }
                    return m_splitRectangleStyle1;
                }
            }

            public string SplitRectangleStyle2
            {
                get
                {
                    if (m_splitRectangleStyle2 == null)
                    {
                        m_splitRectangleStyle2 = (++m_idxStyle).ToString();
                    }
                    return m_splitRectangleStyle2;
                }
            }
        }

        private static class Resources
        {
            private static PatchMapper<Patches> s_diagonalPatchMapper;
            public static PatchMapper<Patches> DiagonalPatchMapper
            {
                get
                {
                    if (s_diagonalPatchMapper == null)
                    {
                        var mappings = new Dictionary<Patches, Patches>
                        {
                            { Patches.HalfSquareTriangle, Patches.HalfSquareTriangle },
                            { Patches.HalfSquareTriangle90, Patches.HalfSquareTriangle270 },
                            { Patches.HalfSquareTriangle180, Patches.HalfSquareTriangle180 },
                            { Patches.HalfSquareTriangle270, Patches.HalfSquareTriangle90 },
                            { Patches.SplitRectangle, Patches.SplitRectangle270 },
                            { Patches.SplitRectangle90, Patches.SplitRectangle180 },
                            { Patches.SplitRectangle180, Patches.SplitRectangle90 },
                            { Patches.SplitRectangle270, Patches.SplitRectangle }
                        };

                        s_diagonalPatchMapper = new PatchMapper<Patches>(mappings);
                    }

                    return s_diagonalPatchMapper;
                }
            }

            private static PatchMapper<Patches> s_swapRectanglesPatchMapper;
            public static PatchMapper<Patches> SwapRectanglesPatchMapper
            {
                get
                {
                    if (s_swapRectanglesPatchMapper == null)
                    {
                        var mappings = new Dictionary<Patches, Patches>
                        {
                            { Patches.Rectangle1, Patches.Rectangle2 },
                            { Patches.Rectangle2, Patches.Rectangle1 }
                        };

                        s_swapRectanglesPatchMapper = new PatchMapper<Patches>(mappings);
                    }

                    return s_swapRectanglesPatchMapper;
                }
            }

            private static PatchMapper<Patches> s_swapSplitRectanglePatchMapper;
            public static PatchMapper<Patches> SwapSplitRectanglesPatchMapper
            {
                get
                {
                    if (s_swapSplitRectanglePatchMapper == null)
                    {
                        var mappings = new Dictionary<Patches, Patches>
                        {
                            { Patches.SplitRectangle, Patches.SplitRectangle180 },
                            { Patches.SplitRectangle90, Patches.SplitRectangle270 },
                            { Patches.SplitRectangle180, Patches.SplitRectangle },
                            { Patches.SplitRectangle270, Patches.SplitRectangle90 },
                        };

                        s_swapSplitRectanglePatchMapper = new PatchMapper<Patches>(mappings);
                    }

                    return s_swapSplitRectanglePatchMapper;
                }
            }

            private static PatchMapper<Patches> s_swapHalfSquareTrianglesPatchMapper;
            public static PatchMapper<Patches> SwapHalfSquareTrianglesPatchMapper
            {
                get
                {
                    if (s_swapHalfSquareTrianglesPatchMapper == null)
                    {
                        var mappings = new Dictionary<Patches, Patches>
                        {
                            { Patches.HalfSquareTriangle, Patches.HalfSquareTriangle180 },
                            { Patches.HalfSquareTriangle90, Patches.HalfSquareTriangle270 },
                            { Patches.HalfSquareTriangle180, Patches.HalfSquareTriangle },
                            { Patches.HalfSquareTriangle270, Patches.HalfSquareTriangle90 },
                        };

                        s_swapHalfSquareTrianglesPatchMapper = new PatchMapper<Patches>(mappings);
                    }

                    return s_swapHalfSquareTrianglesPatchMapper;
                }
            }

            private static PatchMapper<Patches> s_horizontalMirrorPatchMapper;
            public static PatchMapper<Patches> HorizontalMirrorPatchMapper
            {
                get
                {
                    if (s_horizontalMirrorPatchMapper == null)
                    {
                        var mappings = new Dictionary<Patches, Patches>
                        {
                            { Patches.HalfSquareTriangle, Patches.HalfSquareTriangle90 },
                            { Patches.HalfSquareTriangle90, Patches.HalfSquareTriangle },
                            { Patches.HalfSquareTriangle180, Patches.HalfSquareTriangle270 },
                            { Patches.HalfSquareTriangle270, Patches.HalfSquareTriangle180 },
                            { Patches.SplitRectangle, Patches.SplitRectangle180 },
                            { Patches.SplitRectangle90, Patches.SplitRectangle90 },
                            { Patches.SplitRectangle180, Patches.SplitRectangle },
                            { Patches.SplitRectangle270, Patches.SplitRectangle270 }
                        };

                        s_horizontalMirrorPatchMapper = new PatchMapper<Patches>(mappings);
                    }

                    return s_horizontalMirrorPatchMapper;
                }
            }

            private static PatchMapper<Patches> s_rotateRightPatchMapper;
            public static PatchMapper<Patches> RotateRightPatchMapper
            {
                get
                {
                    if (s_rotateRightPatchMapper == null)
                    {
                        var mappings = new Dictionary<Patches, Patches>
                        {
                            { Patches.HalfSquareTriangle, Patches.HalfSquareTriangle90 },
                            { Patches.HalfSquareTriangle90, Patches.HalfSquareTriangle180 },
                            { Patches.HalfSquareTriangle180, Patches.HalfSquareTriangle270 },
                            { Patches.HalfSquareTriangle270, Patches.HalfSquareTriangle },
                            { Patches.SplitRectangle, Patches.SplitRectangle90 },
                            { Patches.SplitRectangle90, Patches.SplitRectangle180 },
                            { Patches.SplitRectangle180, Patches.SplitRectangle270 },
                            { Patches.SplitRectangle270, Patches.SplitRectangle }
                        };

                        s_rotateRightPatchMapper = new PatchMapper<Patches>(mappings);
                    }

                    return s_rotateRightPatchMapper;
                }
            }

            private static PatchMapper<Patches> s_verticalMirrorPatchMapper;
            public static PatchMapper<Patches> VerticalMirrorPatchMapper
            {
                get
                {
                    if (s_verticalMirrorPatchMapper == null)
                    {
                        var mappings = new Dictionary<Patches, Patches>
                        {
                            { Patches.HalfSquareTriangle, Patches.HalfSquareTriangle270 },
                            { Patches.HalfSquareTriangle90, Patches.HalfSquareTriangle180 },
                            { Patches.HalfSquareTriangle180, Patches.HalfSquareTriangle90 },
                            { Patches.HalfSquareTriangle270, Patches.HalfSquareTriangle },
                            { Patches.SplitRectangle, Patches.SplitRectangle },
                            { Patches.SplitRectangle90, Patches.SplitRectangle270 },
                            { Patches.SplitRectangle180, Patches.SplitRectangle180 },
                            { Patches.SplitRectangle270, Patches.SplitRectangle90 }
                        };

                        s_verticalMirrorPatchMapper = new PatchMapper<Patches>(mappings);
                    }

                    return s_verticalMirrorPatchMapper;
                }
            }

            private static FingerprintMapper<Patches> s_mirrorFingerprintMapper;
            public static FingerprintMapper<Patches> MirrorFingerprintMapper
            {
                get
                {
                    if (s_mirrorFingerprintMapper == null)
                    {
                        var fingerprintIndexes = new int[4, 4];

                        fingerprintIndexes[0, 0] = 0;
                        fingerprintIndexes[0, 1] = 1;
                        fingerprintIndexes[0, 2] = 1;
                        fingerprintIndexes[0, 3] = 0;

                        fingerprintIndexes[1, 0] = 1;
                        fingerprintIndexes[1, 1] = 2;
                        fingerprintIndexes[1, 2] = 2;
                        fingerprintIndexes[1, 3] = 1;

                        fingerprintIndexes[2, 0] = 1;
                        fingerprintIndexes[2, 1] = 2;
                        fingerprintIndexes[2, 2] = 2;
                        fingerprintIndexes[2, 3] = 1;

                        fingerprintIndexes[3, 0] = 0;
                        fingerprintIndexes[3, 1] = 1;
                        fingerprintIndexes[3, 2] = 1;
                        fingerprintIndexes[3, 3] = 0;

                        var patchMappers = new PatchMapper<Patches>[4, 4];

                        patchMappers[0, 0] = PatchMapper<Patches>.IdentityMapper;
                        patchMappers[0, 1] = PatchMapper<Patches>.IdentityMapper;
                        patchMappers[0, 2] = HorizontalMirrorPatchMapper;
                        patchMappers[0, 3] = HorizontalMirrorPatchMapper;

                        patchMappers[1, 0] = DiagonalPatchMapper;
                        patchMappers[1, 1] = PatchMapper<Patches>.IdentityMapper;
                        patchMappers[1, 2] = HorizontalMirrorPatchMapper;
                        patchMappers[1, 3] = DiagonalPatchMapper.Then(HorizontalMirrorPatchMapper);

                        patchMappers[2, 0] = DiagonalPatchMapper.Then(VerticalMirrorPatchMapper);
                        patchMappers[2, 1] = VerticalMirrorPatchMapper;
                        patchMappers[2, 2] = HorizontalMirrorPatchMapper.Then(VerticalMirrorPatchMapper);
                        patchMappers[2, 3] = DiagonalPatchMapper.Then(HorizontalMirrorPatchMapper).Then(VerticalMirrorPatchMapper);

                        patchMappers[3, 0] = VerticalMirrorPatchMapper;
                        patchMappers[3, 1] = VerticalMirrorPatchMapper;
                        patchMappers[3, 2] = HorizontalMirrorPatchMapper.Then(VerticalMirrorPatchMapper);
                        patchMappers[3, 3] = HorizontalMirrorPatchMapper.Then(VerticalMirrorPatchMapper);

                        s_mirrorFingerprintMapper = new FingerprintMapper<Patches>(fingerprintIndexes, patchMappers);
                    }

                    return s_mirrorFingerprintMapper;
                }
            }

            private static FingerprintMapper<Patches> s_pinwheelFingerprintMapper;
            public static FingerprintMapper<Patches> PinwheelFingerprintMapper
            {
                get
                {
                    if (s_pinwheelFingerprintMapper == null)
                    {
                        var fingerprintIndexes = new int[4, 4];

                        fingerprintIndexes[0, 0] = 0;
                        fingerprintIndexes[0, 1] = 1;
                        fingerprintIndexes[0, 2] = 1;
                        fingerprintIndexes[0, 3] = 0;

                        fingerprintIndexes[1, 0] = 1;
                        fingerprintIndexes[1, 1] = 2;
                        fingerprintIndexes[1, 2] = 2;
                        fingerprintIndexes[1, 3] = 1;

                        fingerprintIndexes[2, 0] = 1;
                        fingerprintIndexes[2, 1] = 2;
                        fingerprintIndexes[2, 2] = 2;
                        fingerprintIndexes[2, 3] = 1;

                        fingerprintIndexes[3, 0] = 0;
                        fingerprintIndexes[3, 1] = 1;
                        fingerprintIndexes[3, 2] = 1;
                        fingerprintIndexes[3, 3] = 0;

                        var patchMappers = new PatchMapper<Patches>[4, 4];

                        patchMappers[0, 0] = PatchMapper<Patches>.IdentityMapper;
                        patchMappers[0, 1] = PatchMapper<Patches>.IdentityMapper;
                        patchMappers[0, 2] = RotateRightPatchMapper;
                        patchMappers[0, 3] = RotateRightPatchMapper;

                        patchMappers[1, 0] = PatchMapper<Patches>.IdentityMapper;
                        patchMappers[1, 1] = PatchMapper<Patches>.IdentityMapper;
                        patchMappers[1, 2] = RotateRightPatchMapper;
                        patchMappers[1, 3] = RotateRightPatchMapper;

                        patchMappers[2, 0] = RotateRightPatchMapper.Then(RotateRightPatchMapper).Then(RotateRightPatchMapper);
                        patchMappers[2, 1] = RotateRightPatchMapper.Then(RotateRightPatchMapper).Then(RotateRightPatchMapper);
                        patchMappers[2, 2] = RotateRightPatchMapper.Then(RotateRightPatchMapper);
                        patchMappers[2, 3] = RotateRightPatchMapper.Then(RotateRightPatchMapper);

                        patchMappers[3, 0] = RotateRightPatchMapper.Then(RotateRightPatchMapper).Then(RotateRightPatchMapper);
                        patchMappers[3, 1] = RotateRightPatchMapper.Then(RotateRightPatchMapper).Then(RotateRightPatchMapper);
                        patchMappers[3, 2] = RotateRightPatchMapper.Then(RotateRightPatchMapper);
                        patchMappers[3, 3] = RotateRightPatchMapper.Then(RotateRightPatchMapper);

                        s_pinwheelFingerprintMapper = new FingerprintMapper<Patches>(fingerprintIndexes, patchMappers);
                    }

                    return s_pinwheelFingerprintMapper;
                }
            }
        }
    }
}