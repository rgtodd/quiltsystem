//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using RichTodd.QuiltSystem.Service.User.Abstractions.Data;

namespace RichTodd.QuiltSystem.Web.Mvc.Models
{
    public class NavBarVcModelFactory : ApplicationModelFactory
    {
        public NavBarVcModel CreateNavBarVcModel(Session_Data mSessionData)
        {
            return new NavBarVcModel()
            {
                CartItemCount = mSessionData.CartItemCount,
                HasMessages = mSessionData.HasMessages,
                HasNotifications = mSessionData.HasNotifications
            };
        }
    }
}