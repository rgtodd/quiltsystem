//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RichTodd.QuiltSystem.Test.Service.Regression
{
    [TestClass]
    public class UserTest : BaseTest
    {
        [TestInitialize]
        public void TestInitialize()
        {
            OnTestInitialize(mockEvents: true);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            OnTestCleanup();
        }

        [TestMethod]
        public async Task CreateUser()
        {
            _ = await CreateRandomUserAsync();
        }

        [TestMethod]
        public async Task GetRandomUser()
        {
            _ = await GetRandomUserAsync();
        }
    }
}
