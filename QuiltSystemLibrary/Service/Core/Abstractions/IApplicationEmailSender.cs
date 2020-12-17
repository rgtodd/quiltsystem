//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Threading.Tasks;

using RichTodd.QuiltSystem.Service.Core.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Core.Abstractions
{
    public interface IApplicationEmailSender
    {
        Task SendEmailAsync(ApplicationEmailRequest emailRequest);
    }
}
