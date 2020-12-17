//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//

//using System.Diagnostics;

//using Microsoft.VisualStudio.TestTools.UnitTesting;

//using RichTodd.QuiltSystem.Service.Core;

//namespace RichTodd.QuiltSystem.Test.UnitTest
//{
//    [TestClass]
//    public class ProjectLibraryServiceTest
//    {
//        private IServiceEnvironment m_environment;
//        private ServicePool m_servicePool;

//        [TestInitialize]
//        public void TestInitialize()
//        {
//            m_environment = DirectServiceEnvironment.Create(DateTimeAccessors.UtcNow, DateTimeAccessors.LocalNow, TimeZoneInfoAccessors.CentralStandardTime, BuiltInUsers.ServiceUser);
//            m_servicePool = new ServicePool(m_environment);
//        }

//        [TestMethod]
//        [Ignore]
//        public void TestCreate()
//        {
//            var designId = m_servicePool.DesignService.CreateDesignAsync(m_environment.GetUserId(), "testProject").Result;

//            Trace.WriteLine(string.Format("Design ID = {0}", designId));
//        }

//        [TestMethod]
//        [Ignore]
//        public void TestList()
//        {
//            var summaries = m_servicePool.DesignService.GetDesignSummariesAsync(m_environment.GetUserId(), 250).Result;

//            foreach (var summary in summaries.Summaries)
//            {
//                Trace.WriteLine(string.Format("Project ID = {0}, Name = {1}", summary.DesignId, summary.DesignName));
//            }
//        }

//        [TestCleanup]
//        public void TestCleanup()
//        {
//            m_servicePool.Dispose();
//            m_environment.Dispose();
//        }
//    }
//}