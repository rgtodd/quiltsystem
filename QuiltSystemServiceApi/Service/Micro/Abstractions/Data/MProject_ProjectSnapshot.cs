//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MProject_ProjectSnapshot
    {
        public Guid ProjectId { get; set; }
        public long ProjectSnapshotId { get; set; }
        public string ProjectNumber { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Sku { get; set; }
        public IList<MProject_ProjectSnapshotComponent> Components { get; set; }
    }
}
