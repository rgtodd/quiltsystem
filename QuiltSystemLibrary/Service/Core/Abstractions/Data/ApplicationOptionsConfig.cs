//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using Microsoft.Extensions.Options;

namespace RichTodd.QuiltSystem.Service.Core.Abstractions.Data
{
    public class ApplicationOptionsConfig : IConfigureOptions<ApplicationOptions>
    {
        public void Configure(ApplicationOptions options)
        { }
    }
}
