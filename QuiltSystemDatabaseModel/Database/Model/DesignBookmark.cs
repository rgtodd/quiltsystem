//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class DesignBookmark
    {
        public Guid DesignBookmarkId { get; set; }
        public Guid DesignId { get; set; }
        public int DesignSnapshotSequence { get; set; }
    }
}
