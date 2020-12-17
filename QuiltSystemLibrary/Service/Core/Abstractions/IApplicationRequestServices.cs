//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Service.Core.Abstractions
{
    public interface IApplicationRequestServices
    {
        public IApplicationLocale Locale { get; }

        public IApplicationSecurityPolicy SecurityPolicy { get; }
    }
}
