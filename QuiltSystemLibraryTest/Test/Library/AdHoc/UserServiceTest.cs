//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//

//using System;
//using System.Threading.Tasks;

//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using RichTodd.QuiltSystem.Service.Core;
//using RichTodd.QuiltSystem.Service.User.Data;

//namespace RichTodd.QuiltSystem.Test.UnitTest
//{
//    [TestClass]
//    public class UserServiceTest
//    {
//        

//        private IServiceEnvironment m_environment;
//        private ServicePool m_servicePool;

//        private Words m_words;

//        

//        

//        [TestMethod]
//        public async Task CreateUser()
//        {
//            var email = m_words.GetRandomEmail().ToLower();

//            Console.WriteLine("Email = {0}", email);

//            var request = new User_CreateNewUserRequest()
//            {
//                UserName = email,
//                Email = email,
//                Password = "testtest",
//                SuppressEmailConfirmation = true
//            };

//            var userId = await m_servicePool.UserService.CreateNewUserAsync(request);

//            Console.WriteLine("UserId = {0}", userId);
//        }

//        [TestMethod]
//        public async Task CreateUser1000()
//        {
//            for (int idx = 0; idx < 1000; ++idx)
//            {
//                await CreateUser();
//            }
//        }

//        [TestCleanup]
//        public void TestCleanup()
//        {
//            m_servicePool.Dispose();
//            m_environment.Dispose();
//        }

//        [TestInitialize]
//        public void TestInitialize()
//        {
//            m_environment = DirectServiceEnvironment.Create(DateTimeAccessors.UtcNow, DateTimeAccessors.LocalNow, TimeZoneInfoAccessors.CentralStandardTime, BuiltInUsers.ServiceUser);
//            m_servicePool = new ServicePool(m_environment);

//            m_words = new Words();
//        }

//        
//    }
//}