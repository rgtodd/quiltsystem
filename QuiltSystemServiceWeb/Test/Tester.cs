//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Threading.Tasks;

using RichTodd.QuiltSystem.Service.Admin.Abstractions;

namespace RichTodd.QuiltSystem.Test
{
    class Tester
    {
        private readonly Random m_random = new Random();

        public async Task<string> GetRandomUser(IUserAdminService userService)
        {
            var users = await userService.GetUsersAsync(null).ConfigureAwait(false);
            var user = users.MSummaries.Summaries[m_random.Next(users.MSummaries.Summaries.Count)];
            return user.UserId;
        }
    }
}
