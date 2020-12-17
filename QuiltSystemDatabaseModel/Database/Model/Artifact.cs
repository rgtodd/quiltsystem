//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class Artifact
    {
        public Artifact()
        {
            DesignSnapshots = new HashSet<DesignSnapshot>();
            ProjectSnapshots = new HashSet<ProjectSnapshot>();
        }

        public long ArtifactId { get; set; }
        public string Value { get; set; }
        public string ArtifactTypeCode { get; set; }
        public string ArtifactValueTypeCode { get; set; }
        public byte Locked { get; set; }

        public virtual ArtifactType ArtifactTypeCodeNavigation { get; set; }
        public virtual ArtifactValueType ArtifactValueTypeCodeNavigation { get; set; }
        public virtual ICollection<DesignSnapshot> DesignSnapshots { get; set; }
        public virtual ICollection<ProjectSnapshot> ProjectSnapshots { get; set; }
    }
}
