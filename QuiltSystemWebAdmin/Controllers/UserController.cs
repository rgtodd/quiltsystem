//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using RichTodd.QuiltSystem.Service.Admin.Abstractions;
using RichTodd.QuiltSystem.Service.Admin.Abstractions.Data;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Web;
using RichTodd.QuiltSystem.Web.Extensions;
using RichTodd.QuiltSystem.Web.Paging;
using RichTodd.QuiltSystem.WebAdmin.Models.User;

namespace RichTodd.QuiltSystem.WebAdmin.Controllers
{
    public class UserController : ApplicationController<UserModelFactory>
    {
        private IUserAdminService UserAdminService { get; }

        public UserController(
            IApplicationLocale applicationLocale,
            IDomainMicroService domainMicroService,
            IUserAdminService userAdminService)
            : base(
                  applicationLocale,
                  domainMicroService)
        {
            UserAdminService = userAdminService ?? throw new ArgumentNullException(nameof(userAdminService));
        }

        public async Task<ActionResult> Index(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var model = await GetUser(id);

                return View("Detail", model);
            }
            else
            {
                var defaultFilter = ModelFactory.CreateFilter(UserModelFactory.AnyRole, null);

                this.SetSortDefault(ModelFactory.GetDefaultSort());
                this.SetFilterDefault(defaultFilter);

                var model = await GetUserList(this.GetPagingState(0));

                return View("List", model);
            }
        }

        public async Task<ActionResult> ListSubmit()
        {
            return await Index(null);
        }

        [HttpPost]
        public async Task<ActionResult> ListSubmit(UserList model)
        {
            var filter = ModelFactory.CreateFilter(model.Role, model.UserName);

            this.SetPagingState(filter);

            model = await GetUserList(this.GetPagingState(0));

            return View("List", model);
        }

        public async Task<ActionResult> EditRoles(string id)
        {
            var model = await GetEditUserRoles(id);

            return View("EditRoles", model);
        }

        public async Task<ActionResult> RemoveRole(string id, string role)
        {
            _ = await UserAdminService.RemoveUserRoleAsync(id, role);

            return RedirectToAction("EditRoles", new { id });
        }

        public async Task<ActionResult> AddRole(string id, string role)
        {
            _ = await UserAdminService.AddUserRoleAsync(id, role);

            return RedirectToAction("EditRoles", new { id });
        }

        private async Task<User> GetUser(string id)
        {
            var aUser = await UserAdminService.GetUserAsync(id);

            var model = ModelFactory.CreateUser(aUser);

            return model;
        }

        private async Task<EditUserRoles> GetEditUserRoles(string id)
        {
            var aUser = await UserAdminService.GetUserAsync(id);

            var model = ModelFactory.CreateEditUserRoles(aUser);

            return model;
        }

        private async Task<UserList> GetUserList(PagingState pagingState)
        {
            (var role, var userName) = ModelFactory.ParseFilter(pagingState.Filter);

            if (role == UserModelFactory.AnyRole)
            {
                role = null;
            }

            var request = new AUser_GetUsers()
            {
                Role = role,
                UserName = userName
            };

            var aUserSummaries = await UserAdminService.GetUsersAsync(request);

            var model = ModelFactory.CreateUserList(aUserSummaries.MSummaries.Summaries, pagingState);

            return model;
        }

    }
}