//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class Project
    {
        public Project()
        {
            ProjectSnapshots = new HashSet<ProjectSnapshot>();
        }

        public Guid ProjectId { get; set; }
        public string ProjectTypeCode { get; set; }
        public long OwnerId { get; set; }
        public int CurrentProjectSnapshotSequence { get; set; }
        public string Name { get; set; }
        public string ProjectNumber { get; set; }
        public DateTime CreateDateTimeUtc { get; set; }
        public DateTime UpdateDateTimeUtc { get; set; }
        public DateTime? DeleteDateTimeUtc { get; set; }

        public virtual Owner Owner { get; set; }
        public virtual ProjectType ProjectTypeCodeNavigation { get; set; }
        public virtual ICollection<ProjectSnapshot> ProjectSnapshots { get; set; }
    }
}
