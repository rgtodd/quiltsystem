//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

namespace RichTodd.QuiltSystem.Design.Build
{
    internal class BuildPlan : IBuildPlan
    {
        private readonly List<IBuildStep> m_buildSteps = new List<IBuildStep>();

        public IReadOnlyList<IBuildStep> BuildSteps
        {
            get
            {
                return m_buildSteps;
            }
        }

        public void AddBuildStep(IBuildStep buildStep)
        {
            m_buildSteps.Add(buildStep);
        }
    }
}