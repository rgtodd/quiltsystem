//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

using RichTodd.QuiltSystem.Service.Base.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MKit_Kit
    {
        public IList<MKit_KitBuildStep> BuildSteps { get; set; }
        public Guid? DesignId { get; set; }
        public byte[] Image { get; set; }
        public Guid? ProjectId { get; set; }
        public string ProjectName { get; set; }
        public IList<MKit_KitPart> Parts { get; set; }
        public ServiceErrorData ServiceError { get; set; }
        public MKit_KitSpecification Specification { get; set; }
        public MKit_KitSpecificationOptions SpecificationOptions { get; set; }
    }
}