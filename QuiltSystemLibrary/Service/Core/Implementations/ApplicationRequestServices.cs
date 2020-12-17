//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

using RichTodd.QuiltSystem.Service.Core.Abstractions;

namespace RichTodd.QuiltSystem.Service.Core.Implementations
{
    public class ApplicationRequestServices : IApplicationRequestServices
    {
        public ApplicationRequestServices(
                IApplicationLocale locale,
                IApplicationSecurityPolicy securityPolicy)
        {
            Locale = locale ?? throw new ArgumentNullException(nameof(locale));
            SecurityPolicy = securityPolicy ?? throw new ArgumentNullException(nameof(securityPolicy));
        }

        public IApplicationLocale Locale { get; }

        public IApplicationSecurityPolicy SecurityPolicy { get; }
    }
}
