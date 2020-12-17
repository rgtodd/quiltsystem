//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MDesign_Design
    {
        public Guid DesignId { get; set; }
        public long DesignSnapshotId { get; set; }
        public string Name { get; set; }
        public string DesignArtifactValue { get; set; }
        public DateTime UpdateDateTimeUtc { get; set; }
    }
}
