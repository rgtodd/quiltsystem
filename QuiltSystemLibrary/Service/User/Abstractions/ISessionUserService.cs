//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Threading.Tasks;

using RichTodd.QuiltSystem.Service.User.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.User.Abstractions
{
    public interface ISessionUserService
    {
        Task<Session_Data> GetSession(string userId);

        Task<Session_ViewOptionsData> GetViewOptions(string userId);
    }
}