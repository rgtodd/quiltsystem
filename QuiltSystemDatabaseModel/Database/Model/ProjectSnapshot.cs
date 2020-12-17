//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class ProjectSnapshot
    {
        public ProjectSnapshot()
        {
            ProjectSnapshotComponents = new HashSet<ProjectSnapshotComponent>();
        }

        public long ProjectSnapshotId { get; set; }
        public Guid ProjectId { get; set; }
        public int ProjectSnapshotSequence { get; set; }
        public string Name { get; set; }
        public long DesignSnapshotId { get; set; }
        public DateTime CreateDateTimeUtc { get; set; }
        public DateTime UpdateDateTimeUtc { get; set; }
        public long ArtifactId { get; set; }

        public virtual Artifact Artifact { get; set; }
        public virtual DesignSnapshot DesignSnapshot { get; set; }
        public virtual Project Project { get; set; }
        public virtual ICollection<ProjectSnapshotComponent> ProjectSnapshotComponents { get; set; }
    }
}
