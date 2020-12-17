//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

namespace RichTodd.QuiltSystem.Design.Build
{
    internal interface IBuildPlan
    {
        IReadOnlyList<IBuildStep> BuildSteps { get; }
    }
}