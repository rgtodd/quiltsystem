//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MProject_ProjectSpecification
    {
        private readonly IList<MProject_ProjectSpecificationComponent> m_components;

        public MProject_ProjectSpecification(string designArtifactValue, string projectArtifactTypeCode, string projectArtifactValueTypeCode, string projectArtifactValue, ICollection<MProject_ProjectSpecificationComponent> components)
        {
            DesignArtifactValue = designArtifactValue;
            ProjectArtifactTypeCode = projectArtifactTypeCode;
            ProjectArtifactValueTypeCode = projectArtifactValueTypeCode;
            ProjectArtifactValue = projectArtifactValue ?? throw new ArgumentNullException(nameof(projectArtifactValue));
            m_components = new List<MProject_ProjectSpecificationComponent>(components);
        }

        public IList<MProject_ProjectSpecificationComponent> Components { get { return m_components; } }
        public string DesignArtifactValue { get; }
        public string ProjectArtifactTypeCode { get; }
        public string ProjectArtifactValue { get; }
        public string ProjectArtifactValueTypeCode { get; }
    }
}