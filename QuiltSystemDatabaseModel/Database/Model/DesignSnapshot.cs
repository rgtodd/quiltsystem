//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class DesignSnapshot
    {
        public DesignSnapshot()
        {
            ProjectSnapshots = new HashSet<ProjectSnapshot>();
        }

        public long DesignSnapshotId { get; set; }
        public Guid DesignId { get; set; }
        public int DesignSnapshotSequence { get; set; }
        public string Name { get; set; }
        public DateTime CreateDateTimeUtc { get; set; }
        public DateTime UpdateDateTimeUtc { get; set; }
        public long ArtifactId { get; set; }

        public virtual Artifact Artifact { get; set; }
        public virtual Design Design { get; set; }
        public virtual ICollection<ProjectSnapshot> ProjectSnapshots { get; set; }
    }
}
