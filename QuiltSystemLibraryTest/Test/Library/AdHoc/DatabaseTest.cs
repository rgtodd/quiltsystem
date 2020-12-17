//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RichTodd.QuiltSystem.Test.Library.AdHoc
{
    [TestClass]
    public class DatabaseTest : BaseTest
    {
        [TestInitialize]
        public void TestInitialize()
        {
            OnTestInitialize();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            OnTestCleanup();
        }

        [TestMethod]
        public void HasResources()
        {
            //var serviceProvider = (ServiceProvider)TestContext.Properties["SERVICES"];
            //var quiltContextFactory = serviceProvider.GetService<IQuiltContextFactory>();

            using var ctx = QuiltContextFactory.Create();

            var count = ctx.Resources.Count();

            TestContext.WriteLine("{0} records found.", count);

            Assert.AreNotEqual(count, 0);
        }

        [TestMethod]
        public void HasInventoryItems()
        {
            //var serviceProvider = (ServiceProvider)TestContext.Properties["SERVICES"];
            //var quiltContextFactory = serviceProvider.GetService<IQuiltContextFactory>();

            using var ctx = QuiltContextFactory.Create();

            var count = ctx.InventoryItems.Count();

            TestContext.WriteLine("{0} records found.", count);

            Assert.AreNotEqual(count, 0);
        }

        [TestMethod]
        public void HasUsers()
        {
            //var serviceProvider = (ServiceProvider)TestContext.Properties["SERVICES"];
            //var quiltContextFactory = serviceProvider.GetService<IQuiltContextFactory>();

            using var ctx = QuiltContextFactory.Create();

            var count = ctx.AspNetUsers.Count();

            TestContext.WriteLine("{0} records found.", count);

            Assert.AreNotEqual(count, 0);
        }

        //

        //private IServiceEnvironment m_environment;
        //private ServicePool m_servicePool;

        //

        //

        //[TestMethod]
        //[Ignore]
        //public void CreateWebFabrics()
        //{
        //    ModelSetup.DefineWebFabrics();
        //}

        //[TestMethod]
        //public void DefineDomains()
        //{
        //    ModelSetup.DefineDomains();
        //}

        //[TestInitialize]
        //public void Initialize()
        //{
        //    m_environment = DirectServiceEnvironment.Create(DateTimeAccessors.UtcNow, DateTimeAccessors.LocalNow, TimeZoneInfoAccessors.CentralStandardTime, BuiltInUsers.ServiceUser);
        //    m_servicePool = new ServicePool(m_environment);

        //    QuiltContext.DatabaseName = "Quilt";

        //    using (var ctx = QuiltContext.Create())
        //    {
        //        ctx.Database.Initialize(force: true);
        //    }
        //}

        //[TestMethod]
        //public void PersistDatabaseTest()
        //{
        //    QuiltContext.DatabaseName = "Quilt";
        //    var resourceLibrary = DatabaseResourceLibrary.Create("Standard");
        //    AbstractResourceLibrary.AzureSave(resourceLibrary.JsonSave());

        //    QuiltContext.DatabaseName = "QuiltTest";
        //    using (var ctx = QuiltContext.Create())
        //    {
        //        ctx.Database.Initialize(force: true);
        //    }
        //    var json = AbstractResourceLibrary.AzureLoad("Standard");
        //    DatabaseResourceLibrary.Create(json);
        //}

        //[TestCleanup]
        //public void TestCleanup()
        //{
        //    m_servicePool.Dispose();
        //    m_environment.Dispose();
        //}

        //[TestMethod]
        //[Ignore]
        //public void UpdateKitNumbers()
        //{
        //    using (var ctx = QuiltContext.Create())
        //    {
        //        foreach (var dbKit in ctx.Kits.Where(r => r.KitNumber == null).ToList())
        //        {
        //            dbKit.KitNumber = ctx.GetOrderNumber(m_environment.GetUtcNow());
        //        }

        //        ctx.SaveChanges();
        //    }
        //}

        //[TestMethod]
        //[Ignore]
        //public void UpdateOrderStatusTypes()
        //{
        //    using (var ctx = QuiltContext.Create())
        //    {
        //        foreach (var dbOrder in ctx.Orders)
        //        {
        //            dbOrder.UpdateOrderStatusType(ctx, m_environment.GetUtcNow());
        //        }

        //        ctx.SaveChanges();
        //    }
        //}

        //
    }
}