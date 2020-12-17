//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Linq;
using System.Text;

using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Build
{
    internal class BuildStepFlyingGoose : BuildStep
    {
        private readonly string m_producesStyleKey;

        public BuildStepFlyingGoose(string producesStyleKey)
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

            if (styleKey == HalfSquareTriangleStyleFromFlyingGooseStyle(m_producesStyleKey))
            {
                var flyingGooseQuantity = Produces.Where(r => r is BuildComponentFlyingGoose).Sum(r => r.Quantity);
                var triangleQuantity = Produces.Where(r => r is BuildComponentHalfSquareTriangle).Sum(r => r.Quantity);

                return (flyingGooseQuantity * 2) - triangleQuantity;
            }

            return 0;
        }

        public override void ComputeInputs(BuildComponentFactory factory)
        {
            if (Produces.Count == 0)
            {
                return;
            }

            var output = (BuildComponentFlyingGoose)Produces.Where(r => r is BuildComponentFlyingGoose).First();

            Dimension seamAllowance;
            if (output.Trim)
            {
                seamAllowance = new Dimension(0.875, DimensionUnits.Inch);
            }
            else
            {
                seamAllowance = new Dimension(1, DimensionUnits.Inch);
            }

            var producesCount = Produces.Where(r => r is BuildComponentFlyingGoose).Sum(r => r.Quantity);

            var bodyCount = producesCount;
            var bodyWidth = output.Area.Width + seamAllowance;
            var bodyHeight = output.Area.Height + seamAllowance;
            var bodyArea = new Area(bodyWidth, bodyHeight).Round();

            var cornerCount = producesCount * 2;
            var cornerWidth = (output.Area.Width / 2) + seamAllowance;
            var cornerHeight = output.Area.Height + seamAllowance;
            var cornerArea = new Area(cornerWidth, cornerHeight).Round();

            var input1 = factory.CreateBuildComponentRectangle(output.FabricStyles[0], bodyArea);
            input1.Quantity = bodyCount;
            AddInput(input1);

            var input2 = factory.CreateBuildComponentRectangle(output.FabricStyles[1], cornerArea);
            input2.Quantity = cornerCount;
            AddInput(input2);
        }

        private static string HalfSquareTriangleStyleFromFlyingGooseStyle(string flyingGooseStyleKey)
        {
            var values = flyingGooseStyleKey.Split(BuildComponent.StyleKeyDelimiter);

            var sb = new StringBuilder();

            sb.Append(typeof(BuildComponentHalfSquareTriangle).Name);
            sb.Append(BuildComponent.StyleKeyDelimiter);
            sb.Append(values[1].CompareTo(values[2]) <= 0 ? values[1] : values[2]);
            sb.Append(BuildComponent.StyleKeyDelimiter);
            sb.Append(values[1].CompareTo(values[2]) <= 0 ? values[2] : values[1]);
            sb.Append(BuildComponent.StyleKeyDelimiter);
            sb.Append(values[4]);
            sb.Append(BuildComponent.StyleKeyDelimiter);
            sb.Append(values[4]);

            return sb.ToString();
        }
    }
}