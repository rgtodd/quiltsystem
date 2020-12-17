//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;
using System.Globalization;

using RichTodd.QuiltSystem.Design.Component;
using RichTodd.QuiltSystem.Design.Component.Standard;
using RichTodd.QuiltSystem.Design.Primitives;
using RichTodd.QuiltSystem.Service.Ajax.Abstractions.Data;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Ajax.Implementations
{
    internal static class BusinessDataFactory
    {
        public static BlockComponent CreateBlockComponent(XDesign_DesignBlock xDesignBlock)
        {
            var result = BlockComponent.Create(
                xDesignBlock.blockCategory ?? Constants.DefaultComponentCategory,
                xDesignBlock.blockName,
                CreateFabricStyleList(xDesignBlock.fabricStyles));

            return result;
        }

        public static ComponentList CreateBlockComponentList(IEnumerable<XDesign_DesignBlock> xDesignBlocks)
        {
            var result = new ComponentList();

            foreach (var blockData in xDesignBlocks)
            {
                result.Add(CreateBlockComponent(blockData));
            }

            return result;
        }

        public static Design.Core.Design CreateDesign(XDesign_Design xDesign)
        {
            var result = new Design.Core.Design()
            {
                Width = Dimension.ParseNullable(xDesign.width),
                Height = Dimension.ParseNullable(xDesign.height)
            };

            var layoutComponent = xDesign.layout != null
                ? CreateLayoutComponent(xDesign.layout)
                : null;

            result.LayoutComponent = layoutComponent;
            result.LayoutComponent.Children.AddRange(CreateBlockComponentList(xDesign.blocks ?? XDesign_DesignBlock.EmptyArray));

            return result;
        }

        public static MDesign_DesignSpecification Create_MDesign_DesignSpecification(XDesign_Design xDesign)
        {
            var design = CreateDesign(xDesign);

            var designSpecification = new MDesign_DesignSpecification()
            {
                ArtifactValue = design.JsonSave().ToString()
            };

            return designSpecification;
        }

        public static MDesign_DesignSpecification Create_MDesign_DesignSpecification(Design.Core.Design design)
        {
            var designSpecification = new MDesign_DesignSpecification()
            {
                ArtifactValue = design.JsonSave().ToString()
            };

            return designSpecification;
        }

        public static FabricStyleList CreateFabricStyleList(IEnumerable<XDesign_FabricStyle> xFabricStyles)
        {
            var result = new FabricStyleList();

            if (xFabricStyles != null)
            {
                foreach (var fabricStyleData in xFabricStyles)
                {
                    result.Add(CreateFabricStyle(fabricStyleData));
                }
            }

            return result;
        }

        public static LayoutComponent CreateLayoutComponent(XDesign_DesignLayout xDesignLayout)
        {
            var result = LayoutComponent.Create(
                    xDesignLayout.layoutCategory ?? Constants.DefaultComponentCategory,
                    xDesignLayout.layoutName,
                    CreateFabricStyleList(xDesignLayout.fabricStyles),
                    xDesignLayout.rowCount,
                    xDesignLayout.columnCount,
                    xDesignLayout.blockCount);

            return result;
        }

        private static Color CreateColor(XDesign_Color aColor)
        {
            var red = int.Parse(aColor.webColor.Substring(1, 2), NumberStyles.HexNumber);
            var green = int.Parse(aColor.webColor.Substring(3, 2), NumberStyles.HexNumber);
            var blue = int.Parse(aColor.webColor.Substring(5, 2), NumberStyles.HexNumber);

            return Color.FromArgb(red, green, blue);
        }

        private static FabricStyle CreateFabricStyle(XDesign_FabricStyle xFabricStyle)
        {
            var result = new FabricStyle(xFabricStyle.sku, CreateColor(xFabricStyle.color));

            return result;
        }
    }
}