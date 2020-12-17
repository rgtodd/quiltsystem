//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//

//using System;
//using System.Linq;
//using System.Threading.Tasks;

//using Microsoft.VisualStudio.TestTools.UnitTesting;

//using RichTodd.QuiltSystem.AdminService;
//using RichTodd.QuiltSystem.AdminService.Data;
//using RichTodd.QuiltSystem.Database;
//using RichTodd.QuiltSystem.Environment;
//using RichTodd.QuiltSystem.Service;
//using RichTodd.QuiltSystem.Test;

//namespace QuiltAGoGoLibraryTest
//{
//    [TestClass]
//    public class StressTest
//    {
//        private Random m_random = new Random();

//        private IServiceEnvironment m_environment;
//        private ServicePool m_servicePool;
//        private IAdmin_UserService m_adminUserService;

//        private Words m_words;

//        [TestInitialize]
//        public void TestInitialize()
//        {
//            m_environment = DirectServiceEnvironment.Create(null);
//            m_servicePool = new ServicePool(m_environment);
//            m_adminUserService = new Admin_UserService(m_environment);

//            Application.Configure();

//            m_words = new Words();
//        }

//        [TestMethod]
//        public async Task CreateOrderAsync()
//        {
//            var userId = await GetRandomUser();
//            var environment = DirectServiceEnvironment.Create(userId);
//            var shoppingCartService = new ShoppingCartService(environment);

//            await shoppingCartService.AddKit("4AF84FDB-F77B-498A-943D-2797B589DE7F");
//            await shoppingCartService.AddKit("DB73A4A5-E520-4050-B6BD-4CD9F37FF72A");

//            var order = await shoppingCartService.GetCartOrder();
//            await shoppingCartService.UpdateCartItemQuantity(order.Items[0].CartItemId, m_random.Next(10) + 1);
//            await shoppingCartService.UpdateCartItemQuantity(order.Items[1].CartItemId, m_random.Next(10) + 1);
//        }

//        [TestMethod]
//        public async Task SubmitOrderAsync()
//        {
//            var userId = await GetRandomUser();
//            var environment = DirectServiceEnvironment.Create(userId);
//            var orderService = new Admin_OrderService(environment);

//            var orders = orderService.GetOrderSummaries(orderService.ViewMode_Incomplete);
//            foreach (var order in orders)
//            {
//                if (order.OrderStatusType == "Pending")
//                {
//                    await orderService.EnsureOrderSubmittedAsync(order.OrderId);
//                }
//            }
//        }

//        private async Task<Guid> GetRandomUser()
//        {
//            var users = await m_adminUserService.GetUsersAsync();
//            var user = users[m_random.Next(users.Length)];
//            return user.UserId;
//        }

//        [TestCleanup]
//        public void TestCleanup()
//        {
//            //m_servicePool.Dispose();
//        }
//    }
//}
