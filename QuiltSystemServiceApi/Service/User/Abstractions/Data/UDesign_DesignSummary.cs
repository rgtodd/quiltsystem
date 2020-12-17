//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Service.User.Abstractions.Data
{
    public class UDesign_DesignSummary
    {
        public Guid DesignId { get; set; }
        public string DesignName { get; set; }
        public DateTime UpdateDateTimeUtc { get; set; }
    }
}
