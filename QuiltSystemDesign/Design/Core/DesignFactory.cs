//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using RichTodd.QuiltSystem.Design.Component.Standard;
using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Core
{
    public static class DesignFactory
    {
        public static Design CreateEmptyDesign()
        {
            var fabricStyles = new FabricStyleList();

            var design = new Design()
            {
                Width = new Dimension(48, DimensionUnits.Inch),
                Height = new Dimension(48, DimensionUnits.Inch)
            };

            var component = LayoutComponent.Create("Standard", "Quilt Layout", fabricStyles, 3, 3, 2);
            design.LayoutComponent = component;

            return design;
        }
    }
}