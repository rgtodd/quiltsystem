//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Linq;

namespace RichTodd.QuiltSystem.Database.Model.Extensions
{
    public static class AspNetUserExtensions
    {
        public static string EmailName(this AspNetUser aspNetUser)
        {
            var userProfile = aspNetUser.UserProfileAspNetUsers.SingleOrDefault()?.UserProfile;

            return userProfile != null
                ? userProfile.FirstName != null
                    ? userProfile.LastName != null
                        ? userProfile.FirstName + " " + userProfile.LastName
                        : userProfile.FirstName
                    : userProfile.LastName ?? aspNetUser.Email
                : aspNetUser.Email;
        }
    }
}
