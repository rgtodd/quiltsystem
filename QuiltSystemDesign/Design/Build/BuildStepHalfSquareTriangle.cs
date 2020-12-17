//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Build
{
    internal class BuildStepHalfSquareTriangle : BuildStep
    {
        private readonly string m_producesStyleKey;

        public BuildStepHalfSquareTriangle(string producesStyleKey)
        {
            m_producesStyleKey = producesStyleKey ?? throw new ArgumentNullException(nameof(producesStyleKey));
        }

        public override int CanProduceQuantity(string styleKey)
        {
            if (styleKey == null) throw new ArgumentNullException(styleKey);

            return styleKey == m_producesStyleKey ? int.MaxValue : 0;
        }

        public override void ComputeInputs(BuildComponentFactory factory)
        {
            //Consumes.Clear();

            if (Produces.Count == 0) return;

            // BUG: Assumes all outputs are the same.
            //
            var output = (BuildComponentHalfSquareTriangle)Produces[0];

            var seamAllowance = output.Trim
                ? new Dimension(0.875, DimensionUnits.Inch)
                : new Dimension(1, DimensionUnits.Inch);

            var producesCount = 0;
            foreach (var o in Produces)
            {
                producesCount += o.Quantity;
            }

            var inputCount = (producesCount + 1) / 2;
            var width = output.Area.Width + seamAllowance;
            var height = output.Area.Height + seamAllowance;

            var area = new Area(width, height).Round();

            var input1 = factory.CreateBuildComponentRectangle(output.FabricStyles[0], area);
            input1.Quantity = inputCount;
            AddInput(input1);

            var input2 = factory.CreateBuildComponentRectangle(output.FabricStyles[1], area);
            input2.Quantity = inputCount;
            AddInput(input2);
        }
    }
}