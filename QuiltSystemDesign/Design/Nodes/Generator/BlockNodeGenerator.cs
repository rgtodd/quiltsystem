//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

using RichTodd.QuiltSystem.Design.Nodes.Standard;
using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Nodes.Generator
{
    public class BlockNodeGenerator : INodeGenerator
    {
        private const string RectangleStyle1 = "0";
        private const string RectangleStyle2 = "1";

        private enum Patches
        {
            Rectangle1,
            Rectangle2
        }

        public ICollection<NodeGeneratorItem> Generate()
        {
            var items = new List<NodeGeneratorItem>();

            var patterns = new List<Pattern<Patches>>();

            var mirrorMapper = CreateMirrorMapper();
            int mirrorCount = 0;
            foreach (var fingerprint in GetFingerprints())
            {
                var pattern = Pattern<Patches>.CreatePattern(fingerprint, mirrorMapper);
                if (!patterns.Contains(pattern))
                {
                    patterns.Add(pattern);

                    var node = CreateNode(pattern);

                    mirrorCount += 1;

                    var item = new NodeGeneratorItem()
                    {
                        Name = string.Format("Mirror Block {0}", mirrorCount),
                        Node = node,
                        Tags = new string[] { "Symmetric", "4 Patch" }
                    };

                    items.Add(item);
                }
            }

            var pinwheelMapper = CreatePinwheelMapper();
            int pinwheelCount = 0;
            foreach (var fingerprint in GetFingerprints())
            {
                var pattern = Pattern<Patches>.CreatePattern(fingerprint, pinwheelMapper);
                if (!patterns.Contains(pattern))
                {
                    patterns.Add(pattern);

                    var node = CreateNode(pattern);

                    pinwheelCount += 1;

                    var item = new NodeGeneratorItem()
                    {
                        Name = string.Format("Pinwheel Block {0}", pinwheelCount),
                        Node = node,
                        Tags = new string[] { "Symmetric", "4 Patch" }
                    };

                    items.Add(item);
                }
            }

            return items;
        }

        private static FingerprintMapper<Patches> CreateMirrorMapper()
        {
            var fingerprintIndexes = new int[4, 4];

            fingerprintIndexes[0, 0] = 0;
            fingerprintIndexes[0, 1] = 1;
            fingerprintIndexes[0, 2] = 1;
            fingerprintIndexes[0, 3] = 0;

            fingerprintIndexes[1, 0] = 2;
            fingerprintIndexes[1, 1] = 3;
            fingerprintIndexes[1, 2] = 3;
            fingerprintIndexes[1, 3] = 2;

            fingerprintIndexes[2, 0] = 2;
            fingerprintIndexes[2, 1] = 3;
            fingerprintIndexes[2, 2] = 3;
            fingerprintIndexes[2, 3] = 2;

            fingerprintIndexes[3, 0] = 0;
            fingerprintIndexes[3, 1] = 1;
            fingerprintIndexes[3, 2] = 1;
            fingerprintIndexes[3, 3] = 0;

            var patchMappers = new PatchMapper<Patches>[4, 4];

            patchMappers[0, 0] = PatchMapper<Patches>.IdentityMapper;
            patchMappers[0, 1] = PatchMapper<Patches>.IdentityMapper;
            patchMappers[0, 2] = PatchMapper<Patches>.IdentityMapper;
            patchMappers[0, 3] = PatchMapper<Patches>.IdentityMapper;

            patchMappers[1, 0] = PatchMapper<Patches>.IdentityMapper;
            patchMappers[1, 1] = PatchMapper<Patches>.IdentityMapper;
            patchMappers[1, 2] = PatchMapper<Patches>.IdentityMapper;
            patchMappers[1, 3] = PatchMapper<Patches>.IdentityMapper;

            patchMappers[2, 0] = PatchMapper<Patches>.IdentityMapper;
            patchMappers[2, 1] = PatchMapper<Patches>.IdentityMapper;
            patchMappers[2, 2] = PatchMapper<Patches>.IdentityMapper;
            patchMappers[2, 3] = PatchMapper<Patches>.IdentityMapper;

            patchMappers[3, 0] = PatchMapper<Patches>.IdentityMapper;
            patchMappers[3, 1] = PatchMapper<Patches>.IdentityMapper;
            patchMappers[3, 2] = PatchMapper<Patches>.IdentityMapper;
            patchMappers[3, 3] = PatchMapper<Patches>.IdentityMapper;

            return new FingerprintMapper<Patches>(fingerprintIndexes, patchMappers);
        }

        private static Node CreateNode(Pattern<Patches> pattern)
        {
            int rowCount = pattern.RowCount;
            int columnCount = pattern.ColumnCount;

            GridLayoutNode gridLayoutNode = new GridLayoutNode(rowCount, columnCount);

            for (int row = 0; row < rowCount; ++row)
            {
                for (int column = 0; column < columnCount; ++column)
                {
                    var node = (pattern.Values[row, column]) switch
                    {
                        Patches.Rectangle1 => new RectangleShapeNode(FabricStyle.Default)
                        {
                            Style = RectangleStyle1
                        },

                        Patches.Rectangle2 => new RectangleShapeNode(FabricStyle.Default)
                        {
                            Style = RectangleStyle2
                        },

                        _ => throw new InvalidOperationException(string.Format("Unknown patch {0}", pattern.Values[row, column])),
                    };
                    var layoutSite = gridLayoutNode.GetLayoutSite(row, column);
                    layoutSite.Node = node;
                }
            }

            return gridLayoutNode.GetSimplified();
        }

        private static FingerprintMapper<Patches> CreatePinwheelMapper()
        {
            var fingerprintIndexes = new int[4, 4];

            fingerprintIndexes[0, 0] = 0;
            fingerprintIndexes[0, 1] = 1;
            fingerprintIndexes[0, 2] = 2;
            fingerprintIndexes[0, 3] = 0;

            fingerprintIndexes[1, 0] = 2;
            fingerprintIndexes[1, 1] = 3;
            fingerprintIndexes[1, 2] = 3;
            fingerprintIndexes[1, 3] = 1;

            fingerprintIndexes[2, 0] = 1;
            fingerprintIndexes[2, 1] = 3;
            fingerprintIndexes[2, 2] = 3;
            fingerprintIndexes[2, 3] = 2;

            fingerprintIndexes[3, 0] = 0;
            fingerprintIndexes[3, 1] = 2;
            fingerprintIndexes[3, 2] = 1;
            fingerprintIndexes[3, 3] = 0;

            var patchMappers = new PatchMapper<Patches>[4, 4];

            patchMappers[0, 0] = PatchMapper<Patches>.IdentityMapper;
            patchMappers[0, 1] = PatchMapper<Patches>.IdentityMapper;
            patchMappers[0, 2] = PatchMapper<Patches>.IdentityMapper;
            patchMappers[0, 3] = PatchMapper<Patches>.IdentityMapper;

            patchMappers[1, 0] = PatchMapper<Patches>.IdentityMapper;
            patchMappers[1, 1] = PatchMapper<Patches>.IdentityMapper;
            patchMappers[1, 2] = PatchMapper<Patches>.IdentityMapper;
            patchMappers[1, 3] = PatchMapper<Patches>.IdentityMapper;

            patchMappers[2, 0] = PatchMapper<Patches>.IdentityMapper;
            patchMappers[2, 1] = PatchMapper<Patches>.IdentityMapper;
            patchMappers[2, 2] = PatchMapper<Patches>.IdentityMapper;
            patchMappers[2, 3] = PatchMapper<Patches>.IdentityMapper;

            patchMappers[3, 0] = PatchMapper<Patches>.IdentityMapper;
            patchMappers[3, 1] = PatchMapper<Patches>.IdentityMapper;
            patchMappers[3, 2] = PatchMapper<Patches>.IdentityMapper;
            patchMappers[3, 3] = PatchMapper<Patches>.IdentityMapper;

            return new FingerprintMapper<Patches>(fingerprintIndexes, patchMappers);
        }

        private static IEnumerable<Fingerprint<Patches>> GetFingerprints()
        {
            foreach (var patch1 in GetPatches())
            {
                foreach (var patch2 in GetPatches())
                {
                    foreach (var patch3 in GetPatches())
                    {
                        foreach (var patch4 in GetPatches())
                        {
                            var patches = new Patches[] { patch1, patch2, patch3, patch4 };
                            yield return new Fingerprint<Patches>(patches);
                        }
                    }
                }
            }
        }

        private static IEnumerable<Patches> GetPatches()
        {
            yield return Patches.Rectangle1;
            yield return Patches.Rectangle2;
        }
    }
}