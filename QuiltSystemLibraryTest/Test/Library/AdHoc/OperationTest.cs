//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//

//using System;
//using System.Threading.Tasks;

//using Microsoft.VisualStudio.TestTools.UnitTesting;

//using RichTodd.QuiltSystem.Business.Operation;
//using RichTodd.QuiltSystem.Service.Core;

//namespace RichTodd.QuiltSystem.Test.UnitTest
//{
//    [TestClass]
//    public class OperationTest
//    {
//        private IServiceEnvironment m_environment;
//        private ServicePool m_servicePool;

//        [TestInitialize]
//        public void Initialize()
//        {
//            m_environment = DirectServiceEnvironment.Create(DateTimeAccessors.UtcNow, DateTimeAccessors.LocalNow, TimeZoneInfoAccessors.CentralStandardTime, BuiltInUsers.ServiceUser);
//            m_servicePool = new ServicePool(m_environment);
//        }

//        [TestMethod]
//        public async Task UspsAddressValidateOperationTest()
//        {
//            var op = new UspsAddressValidateOperation(m_environment);
//            var response = await op.ExecuteAsync("17340 west 156th Terrace", "apt 2", "olathe", "kansas", "66062");
//            Console.WriteLine("UspsAddressValidateOperationTest {0}", response.ResponseData);
//            Console.WriteLine("Address1 = {0}", response.Address1);
//            Console.WriteLine("Address2 = {0}", response.Address2);
//            Console.WriteLine("City = {0}", response.City);
//            Console.WriteLine("StateCode = {0}", response.StateCode);
//            Console.WriteLine("PostalCode = {0}", response.PostalCode);
//        }

//        [TestCleanup]
//        public void TestCleanup()
//        {
//            m_servicePool.Dispose();
//            m_environment.Dispose();
//        }
//    }
//}