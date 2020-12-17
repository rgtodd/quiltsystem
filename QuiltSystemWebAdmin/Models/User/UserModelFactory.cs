//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc.Rendering;

using PagedList;

using RichTodd.QuiltSystem.Resources.Web;
using RichTodd.QuiltSystem.Security;
using RichTodd.QuiltSystem.Service.Admin.Abstractions.Data;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;
using RichTodd.QuiltSystem.Web;
using RichTodd.QuiltSystem.Web.Paging;

namespace RichTodd.QuiltSystem.WebAdmin.Models.User
{
    public class UserModelFactory : ApplicationModelFactory
    {
        public const string AnyRole = "*ANY";

        public User CreateUser(AUser_User aUser)
        {
            return new User(aUser, Locale);
        }

        public EditUserRoles CreateEditUserRoles(AUser_User aUser)
        {
            var currentRoles = new List<string>(aUser.Roles);
            currentRoles.Sort();

            var newRoles = new List<string>()
            {
                ApplicationRoles.Administrator,
                ApplicationRoles.Service,
                ApplicationRoles.User,
                ApplicationRoles.FinancialViewer,
                ApplicationRoles.FinancialEditor,
                ApplicationRoles.FulfillmentViewer,
                ApplicationRoles.FulfillmentEditor,
                ApplicationRoles.UserViewer,
                ApplicationRoles.UserEditor,
            };
            foreach (var role in currentRoles)
            {
                _ = newRoles.Remove(role);
            }
            newRoles.Sort();


            return new EditUserRoles()
            {
                UserId = aUser.UserId,
                Email = aUser.Email,
                CurrentRoles = currentRoles,
                NewRoles = newRoles
            };
        }

        public UserList CreateUserList(IList<MUser_UserSummary> aUserSummaries, PagingState pagingState)
        {
            var userSummaries = aUserSummaries.Select(r => CreateUserListItem(r)).ToList();

            var sortFunction = GetSortFunction(pagingState.Sort);
            var sortedUserSummaries = sortFunction != null
                ? pagingState.Descending
                    ? userSummaries.OrderByDescending(sortFunction).ToList()
                    : userSummaries.OrderBy(sortFunction).ToList()
                : userSummaries;

            var pageSize = 10;
            var pageNumber = WebMath.GetPageNumber(pagingState.Page, sortedUserSummaries.Count, pageSize);
            var pagedUserSummaries = sortedUserSummaries.ToPagedList(pageNumber, pageSize);

            (var role, var userName) = ParseFilter(pagingState.Filter);

            var model = new UserList()
            {
                UserSummaries = pagedUserSummaries,
                Role = role,
                Roles = new List<SelectListItem>
                {
                    new SelectListItem() { Text = "Any", Value= AnyRole },
                    new SelectListItem() { Text = "Administrator", Value= ApplicationRoles.Administrator },
                    new SelectListItem() { Text = "Service", Value= ApplicationRoles.Service },
                    new SelectListItem() { Text = "User", Value= ApplicationRoles.User },
                    new SelectListItem() { Text = "Financial Viewer", Value= ApplicationRoles.FinancialViewer },
                    new SelectListItem() { Text = "Financial Editor", Value= ApplicationRoles.FinancialEditor },
                    new SelectListItem() { Text = "Fulfillment Viewer", Value= ApplicationRoles.FulfillmentViewer },
                    new SelectListItem() { Text = "Fulfillment Editor", Value= ApplicationRoles.FulfillmentEditor },
                    new SelectListItem() { Text = "User Viewer", Value= ApplicationRoles.UserViewer },
                    new SelectListItem() { Text = "User Editor", Value= ApplicationRoles.UserEditor }
                },
                UserName = userName
            };

            return model;
        }

        public string GetDefaultSort()
        {
            return ListItemMetadata.GetDisplayName(m => m.UserName);
        }

        public string CreateFilter(string role, string userName)
        {
            return $"{role}|{userName}";
        }

        public (string role, string userName) ParseFilter(string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                return (null, null);
            }

            var idxDelimiter = filter.IndexOf('|');
            if (idxDelimiter == -1)
            {
                return (null, null);
            }

            var role = filter.Substring(0, idxDelimiter);
            var userName = filter.Substring(idxDelimiter + 1);
            return (role, userName);
        }

        private UserListItem CreateUserListItem(MUser_UserSummary aUserSummary)
        {
            var summary = new UserListItem()
            {
                UserId = aUserSummary.UserId,
                UserName = aUserSummary.UserName
            };

            return summary;
        }

        private ModelMetadata<UserListItem> m_listItemMetadata;
        private ModelMetadata<UserListItem> ListItemMetadata
        {
            get
            {
                if (m_listItemMetadata == null)
                {
                    m_listItemMetadata = ModelMetadata<UserListItem>.Create(HttpContext);
                }

                return m_listItemMetadata;
            }
        }

        private static IDictionary<string, Func<UserListItem, object>> s_sortFunctions;
        private IDictionary<string, Func<UserListItem, object>> SortFunctions
        {
            get
            {
                if (s_sortFunctions == null)
                {
                    var heading = new UserListItem();

                    var sortFunctions = new Dictionary<string, Func<UserListItem, object>>
                    {
                        { ListItemMetadata.GetDisplayName(m => m.UserName), r => r.UserName }
                    };

                    s_sortFunctions = sortFunctions;
                }

                return s_sortFunctions;
            }
        }

        private Func<UserListItem, object> GetSortFunction(string sort)
        {
            return !string.IsNullOrEmpty(sort)
                ? SortFunctions[sort]
                : null;
        }
    }
}