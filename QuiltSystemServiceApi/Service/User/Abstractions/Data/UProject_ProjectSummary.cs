//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Service.User.Abstractions.Data
{
    public class UProject_ProjectSummary
    {
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; }
        public DateTime UpdateDateTimeUtc { get; set; }
    }
}
