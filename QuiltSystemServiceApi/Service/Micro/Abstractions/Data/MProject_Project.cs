//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MProject_Project
    {
        public MProject_Project(Guid projectId, long projectSnapshotId, string name, DateTime updateDateTimeUtc, MProject_ProjectSpecification specification)
        {
            ProjectId = projectId;
            ProjectSnapshotId = projectSnapshotId;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            UpdateDateTimeUtc = updateDateTimeUtc;
            Specification = specification ?? throw new ArgumentNullException(nameof(specification));
        }

        public MProject_ProjectSpecification Specification { get; }
        public string Name { get; }
        public Guid ProjectId { get; }
        public long ProjectSnapshotId { get; }
        public DateTime UpdateDateTimeUtc { get; }
    }
}