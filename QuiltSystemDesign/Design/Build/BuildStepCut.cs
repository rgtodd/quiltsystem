//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Build
{
    internal class BuildStepCut : BuildStep
    {
        private readonly string m_producesStyleKey;

        public BuildStepCut(string producesStyleKey)
        {
            if (producesStyleKey == null) throw new ArgumentNullException(nameof(producesStyleKey));

            m_producesStyleKey = CutStyleFromRectangleStyle(producesStyleKey);
        }

        public override int CanProduceQuantity(string styleKey)
        {
            if (styleKey == null) throw new ArgumentNullException(styleKey);

            if (m_producesStyleKey == CutStyleFromRectangleStyle(styleKey))
            {
                return int.MaxValue;
            }

            return 0;
        }

        public override void ComputeInputs(BuildComponentFactory factory)
        {
            //Consumes.Clear();

            if (Produces.Count == 0) return;

            var cutShapes = new List<ICutShape>();
            foreach (BuildComponentRectangle output in Produces)
            {
                for (int idx = 0; idx < output.Quantity; ++idx)
                {
                    cutShapes.Add(new CutShape(output));
                }
            }

            var cutPlan = CutPlanner.Plan(cutShapes);

            foreach (var cutStock in cutPlan.CutStocks)
            {
                var input = factory.CreateBuildComponentYardage(((BuildComponentRectangle)Produces[0]).FabricStyle, cutStock.AreaSize);

                foreach (var cutRegion in cutPlan.CutRegions)
                {
                    if (cutRegion.CutStock == cutStock && cutRegion.CutShape != null)
                    {
                        input.Regions.Add(
                            new BuildComponentYardageRegion(
                                ((CutShape)cutRegion.CutShape).BuildComponentRectangle,
                                cutRegion.Left,
                                cutRegion.Top,
                                cutRegion.Width,
                                cutRegion.Height));
                    }
                }

                AddInput(input);
            }
        }

        private static string CutStyleFromRectangleStyle(string rectangleStyleKey)
        {
            var idx = rectangleStyleKey.IndexOf(BuildComponent.StyleKeyDelimiter);
            idx = rectangleStyleKey.IndexOf(BuildComponent.StyleKeyDelimiter, idx + 1);
            return rectangleStyleKey.Substring(0, idx);
        }

        #region Private Classes

        private class CutShape : ICutShape
        {

            private readonly BuildComponentRectangle m_buildComponentRectangle;

            public CutShape(BuildComponentRectangle buildComponentRectangle)
            {
                m_buildComponentRectangle = buildComponentRectangle ?? throw new ArgumentNullException(nameof(buildComponentRectangle));
            }

            public Area Area
            {
                get
                {
                    return m_buildComponentRectangle.Area;
                }
            }

            public BuildComponentRectangle BuildComponentRectangle
            {
                get
                {
                    return m_buildComponentRectangle;
                }
            }

        }

        #endregion Private Classes
    }
}