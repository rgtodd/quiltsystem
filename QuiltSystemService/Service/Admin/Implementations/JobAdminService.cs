//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using Microsoft.Extensions.Logging;

using RichTodd.QuiltSystem.Service.Admin.Abstractions;
using RichTodd.QuiltSystem.Service.Base;
using RichTodd.QuiltSystem.Service.Core.Abstractions;

namespace RichTodd.QuiltSystem.Service.Admin.Implementations
{
    internal class JobAdminService : BaseService, IJobAdminService
    {
        public JobAdminService(
            IApplicationRequestServices requestServices,
            ILogger<JobAdminService> logger)
            : base(requestServices, logger)
        { }
    }
}
