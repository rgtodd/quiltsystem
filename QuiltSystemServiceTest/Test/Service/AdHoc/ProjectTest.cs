//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Test.Service.AdHoc
{
    [TestClass]
    public class ProjectTest : BaseTest
    {
        [TestInitialize]
        public void TestInitialize()
        {
            OnTestInitialize(mockEvents: false);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            OnTestCleanup();
        }

        [TestMethod]
        public async Task CreateProject()
        {
            var logger = ServiceScope.ServiceProvider.GetService<ILogger<ProjectTest>>();

            // Get the user ID.
            //
            string userId;
            {
                var identityUser = await UserManager.FindByNameAsync("user@richtodd.com");
                userId = identityUser.Id;
                logger.LogInformation("UserId = {0}", userId);
            }

            // Create the design.
            //
            Guid designId;
            {
                var designData = Factory.CreateDesign();
                designId = await DesignAjaxService.SaveDesignAsync(userId, designData);
                logger.LogInformation($"Design ID = {designId}");
            }

            // Create the project.
            //
            string projectId;
            {
                projectId = await ProjectUserService.CreateProjectAsync(userId, ProjectUserService.ProjectType_Kit, "Test Project", designId);
                logger.LogInformation($"Project ID = {projectId}");
            }

            // Create the orderable ID.
            //
            long orderableId;
            {
                var projectSnapshotId = await ProjectMicroService.GetCurrentSnapshotIdAsync(Guid.Parse(projectId));
                var mProjectSnapshotDetail = await ProjectMicroService.GetProjectSnapshotAsync(projectSnapshotId);
                var mAllocateOrderable = MicroDataFactory.MOrder_AllocateOrderable(mProjectSnapshotDetail);
                var mAllocateOrderableResponseData = await OrderMicroService.AllocateOrderableAsync(mAllocateOrderable);
                orderableId = mAllocateOrderableResponseData.OrderableId;
                logger.LogInformation($"Orderable ID = {orderableId}");
            }
        }
    }
}
