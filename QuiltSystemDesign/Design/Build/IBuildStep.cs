//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

namespace RichTodd.QuiltSystem.Design.Build
{
    internal interface IBuildStep
    {
        string Description { get; }
        int StepNumber { get; }

        IReadOnlyList<IBuildComponent> Consumes { get; }
        IReadOnlyList<IBuildComponent> Produces { get; }

        void AddInput(IBuildComponent component);
        void AddOutput(IBuildComponent component);
        int CanProduceQuantity(string styleKey);
        void ComputeInputs(BuildComponentFactory factory);
    }
}