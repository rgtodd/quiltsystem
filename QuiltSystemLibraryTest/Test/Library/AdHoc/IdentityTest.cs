//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//

//using System;
//using System.Collections.Generic;

//using Microsoft.AspNet.Identity;
//using Microsoft.AspNet.Identity.EntityFramework;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

//using RichTodd.QuiltSystem.Business.Identity;

//namespace RichTodd.QuiltSystem.Test.UnitTest
//{
//    [TestClass]
//    public class IdentityTest
//    {
//        [TestMethod]
//        [Ignore]
//        public  void CreateRole()
//        {
//            using (var ctx = ApplicationDbContext.Create())
//            {
//                //var rm = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(ctx));
//                //var ir = rm.Create(new IdentityRole("Administrator"));

//                var userIds = new List<Guid>();
//                var um = new ApplicationUserManager(new UserStore<ApplicationUser, ApplicationRole, Guid, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>(ctx));
//                foreach (var user in um.Users)
//                {
//                    userIds.Add(user.Id);
//                }

//                foreach (var userId in userIds)
//                {
//                    um.AddToRole(userId, "Administrator");
//                }
//            }
//        }
//    }
//}
