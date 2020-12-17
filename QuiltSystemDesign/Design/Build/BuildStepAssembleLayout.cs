//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Design.Build
{
    internal class BuildStepAssembleLayout : BuildStep
    {
        private readonly string m_producesStyleKey;

        public BuildStepAssembleLayout(string producesStyleKey)
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

            foreach (BuildComponentLayout output in Produces)
            {
                if (CanProduceQuantity(output.StyleKey) == 0)
                {
                    throw new InvalidOperationException(string.Format("Output StyleKey {0} does not match ProducesStyleKey {1}.", output.StyleKey, m_producesStyleKey));
                }

                for (int idx = 0; idx < output.Quantity; ++idx)
                {
                    AddLayoutNodeInputs(factory, output.LayoutNode, output.TrimTriangles, true);
                }
            }
        }
    }
}