//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Build
{
    internal class BuildStepAssembleQuilt : BuildStep
    {
        private readonly string m_producesStyleKey;

        public BuildStepAssembleQuilt(string producesStyleKey)
        {
            m_producesStyleKey = producesStyleKey ?? throw new ArgumentNullException(nameof(producesStyleKey));
        }

        public override int CanProduceQuantity(string styleKey)
        {
            if (styleKey == null) throw new ArgumentNullException(styleKey);

            if (styleKey == m_producesStyleKey)
            {
                return int.MaxValue;
            }

            return 0;
        }

        public override void ComputeInputs(BuildComponentFactory factory)
        {
            if (Consumes.Count != 0)
            {
                throw new InvalidOperationException("Inputs already computed.");
            }

            if (Produces.Count != 1
                || !(Produces[0] is BuildComponentQuilt))
            {
                throw new InvalidOperationException("Invalid outputs.");
            }

            var output = Produces[0] as BuildComponentQuilt;

            // Add build components for quilt top.
            //
            AddLayoutNodeInputs(factory, output.PageLayoutNode, output.KitSpecification.TrimTriangles, true);

            // Add build components for binding.
            //
            {
                var bindingWidth = output.KitSpecification.BindingWidth;
                if (bindingWidth.Value > 0)
                {
                    var maxBindingHeight = new Dimension(40, DimensionUnits.Inch);
                    var bindingAllowance = new Dimension(12, DimensionUnits.Inch);

                    var style = output.KitSpecification.BindingFabricStyle;
                    var bindingHeight = (output.KitSpecification.Width * 2) + (output.KitSpecification.Height * 2) + bindingAllowance;

                    while (bindingHeight > maxBindingHeight)
                    {
                        AddOrUpdateInput(factory, style, Area.CreateHorizontalArea(bindingWidth, maxBindingHeight));
                        bindingHeight -= maxBindingHeight;
                    }
                    {
                        AddOrUpdateInput(factory, style, Area.CreateHorizontalArea(bindingWidth, bindingHeight));
                    }
                }
            }

            // Add build components for backing.
            {
                if (output.KitSpecification.HasBacking)
                {
                    var maxBackingHeight = new Dimension(3 * 36, DimensionUnits.Inch);
                    var maxBackingWidth = new Dimension(40, DimensionUnits.Inch);
                    var style = output.KitSpecification.BackingFabricStyle;

                    var backingHeight = output.KitSpecification.Height;
                    while (backingHeight > maxBackingHeight)
                    {
                        var backingWidth = output.KitSpecification.Width;
                        while (backingWidth > maxBackingWidth)
                        {
                            AddOrUpdateInput(factory, style, Area.CreateHorizontalArea(maxBackingWidth, maxBackingHeight));
                            backingWidth -= maxBackingWidth;
                        }
                        {
                            AddOrUpdateInput(factory, style, Area.CreateHorizontalArea(backingWidth, maxBackingHeight));
                        }

                        backingHeight -= maxBackingHeight;
                    }

                    {
                        var backingWidth = output.KitSpecification.Width;
                        while (backingWidth > maxBackingWidth)
                        {
                            AddOrUpdateInput(factory, style, Area.CreateHorizontalArea(maxBackingWidth, backingHeight));
                            backingWidth -= maxBackingWidth;
                        }
                        {
                            AddOrUpdateInput(factory, style, Area.CreateHorizontalArea(backingWidth, backingHeight));
                        }
                    }
                }
            }
        }

        private void AddOrUpdateInput(BuildComponentFactory factory, FabricStyle style, Area area)
        {
            foreach (var input in Consumes)
            {
                if (input is BuildComponentRectangle inputRectangle &&
                    inputRectangle.FabricStyle.Sku == style.Sku &&
                    inputRectangle.Area.Matches(area))
                {
                    inputRectangle.Quantity += 1;
                    return;
                }
            }

            var component = factory.CreateBuildComponentRectangle(style, area);
            AddInput(component);
        }
    }
}