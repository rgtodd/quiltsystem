//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json.Linq;

using RichTodd.QuiltSystem.Business.Libraries;
using RichTodd.QuiltSystem.Database.Domain;
using RichTodd.QuiltSystem.Design.Core;
using RichTodd.QuiltSystem.Service.Base;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;
using RichTodd.QuiltSystem.Service.User.Abstractions;
using RichTodd.QuiltSystem.Service.User.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.User.Implementations
{
    internal class ProjectUserService : BaseService, IProjectUserService
    {
        private IDesignMicroService DesignMicroService { get; }
        private IProjectMicroService ProjectMicroService { get; }

        public ProjectUserService(
            IApplicationRequestServices requestServices,
            ILogger<ProjectUserService> logger,
            IDesignMicroService designMicroService,
            IProjectMicroService projectMicroService)
            : base(requestServices, logger)
        {
            DesignMicroService = designMicroService ?? throw new ArgumentNullException(nameof(designMicroService));
            ProjectMicroService = projectMicroService ?? throw new ArgumentNullException(nameof(projectMicroService));
        }

        public string ProjectType_Kit
        {
            get { return "KIT"; }
        }

        public async Task<string> CreateProjectAsync(string userId, string projectType, string projectName, Guid designId)
        {
            using var log = BeginFunction(nameof(ProjectUserService), nameof(CreateProjectAsync), projectType, userId, projectName, designId);
            try
            {
                await Assert(SecurityPolicy.IsAuthorized, userId).ConfigureAwait(false);

                string result;
                if (projectType == ProjectType_Kit)
                {
                    var mDesign = await DesignMicroService.GetDesignAsync(designId).ConfigureAwait(false);

                    var design = new Design.Core.Design(JToken.Parse(mDesign.DesignArtifactValue));

                    var projectData = ProjectLibraryKitUtility.CreateProjectSpecification(design);

                    var ownerReference = CreateOwnerReference.FromUserId(userId);
                    var ownerId = await ProjectMicroService.AllocateOwnerAsync(ownerReference).ConfigureAwait(false);

                    var id = await ProjectMicroService.CreateProjectAsync(ownerId, projectName, ProjectTypeCodes.Kit, mDesign.DesignSnapshotId, projectData, GetUtcNow()).ConfigureAwait(false);

                    result = id.ToString();
                }
                else
                {
                    throw new ArgumentException(string.Format("Unknown projectType {0}", projectType), nameof(projectType));
                }

                log.Result(result);
                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<bool> DeleteProjectAsync(string userId, Guid projectId)
        {
            using var log = BeginFunction(nameof(ProjectUserService), nameof(DeleteProjectAsync), userId, projectId);
            try
            {
                await Assert(SecurityPolicy.IsAuthorized, userId).ConfigureAwait(false);

                var result = await ProjectMicroService.DeleteProjectAsync(projectId, GetUtcNow()).ConfigureAwait(false);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<byte[]> GetProjectSnapshotThumbnailAsync(string userId, int projectSnapshotId, int thumbnailSize)
        {
            using var log = BeginFunction(nameof(ProjectUserService), nameof(GetProjectSnapshotThumbnailAsync), userId, projectSnapshotId, thumbnailSize);
            try
            {
                await Assert(SecurityPolicy.IsAuthorized, userId).ConfigureAwait(false);

                var entry = await ProjectMicroService.GetProjectAsync(projectSnapshotId).ConfigureAwait(false);

                var kit = ProjectLibraryKitUtility.CreateKit(entry);

                var renderer = new DesignRenderer();

                using var image = renderer.CreateBitmap(kit, thumbnailSize);
                using var ms = new MemoryStream();

                image.Save(ms, ImageFormat.Png);
                var result = ms.ToArray();

                log.Result(string.Format("byte[{0}]", result.Length));
                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<UProject_ProjectSummaryList> GetProjectSummariesAsync(string userId, int? skip, int? take)
        {
            using var log = BeginFunction(nameof(ProjectUserService), nameof(GetProjectSummariesAsync), userId, skip, take);
            try
            {
                await Assert(SecurityPolicy.IsAuthorized, userId).ConfigureAwait(false);

                var ownerReference = CreateOwnerReference.FromUserId(userId);
                var ownerId = await ProjectMicroService.AllocateOwnerAsync(ownerReference).ConfigureAwait(false);

                var entries = await ProjectMicroService.GetProjectsAsync(ownerId, skip, take).ConfigureAwait(false);
                var hasDeletedProjects = await ProjectMicroService.HasDeletedProjectsAsync(ownerId).ConfigureAwait(false);

                var summaries = Create.UProject_ProjectSummaries(entries);

                var result = new UProject_ProjectSummaryList()
                {
                    ProjectSummaries = summaries,
                    HasDeletedProjects = hasDeletedProjects
                };

                log.Result(result);
                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<byte[]> GetProjectThumbnailAsync(string userId, Guid projectId, int thumbnailSize)
        {
            using var log = BeginFunction(nameof(ProjectUserService), nameof(GetProjectThumbnailAsync), userId, projectId, thumbnailSize);
            try
            {
                await Assert(SecurityPolicy.IsAuthorized, userId).ConfigureAwait(false);

                var entry = await ProjectMicroService.GetProjectAsync(projectId).ConfigureAwait(false);

                var kit = ProjectLibraryKitUtility.CreateKit(entry);

                var renderer = new DesignRenderer();

                using var image = renderer.CreateBitmap(kit, thumbnailSize);
                using var ms = new MemoryStream();

                image.Save(ms, ImageFormat.Png);
                var result = ms.ToArray();

                log.Result(string.Format("byte[{0}]", result.Length));
                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<bool> RenameProjectAsync(string userId, Guid projectId, string projectName)
        {
            using var log = BeginFunction(nameof(ProjectUserService), nameof(RenameProjectAsync), userId, projectId, projectName);
            try
            {
                await Assert(SecurityPolicy.IsAuthorized, userId).ConfigureAwait(false);

                var result = await ProjectMicroService.RenameProjectAsync(projectId, projectName).ConfigureAwait(false);

                log.Result(result);
                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<Guid?> UndeleteProjectAsync(string userId)
        {
            using var log = BeginFunction(nameof(ProjectUserService), nameof(UndeleteProjectAsync), userId);
            try
            {
                await Assert(SecurityPolicy.IsAuthorized, userId).ConfigureAwait(false);

                var ownerReference = CreateOwnerReference.FromUserId(userId);
                var ownerId = await ProjectMicroService.AllocateOwnerAsync(ownerReference).ConfigureAwait(false);

                var result = await ProjectMicroService.UndeleteProjectAsync(ownerId, GetUtcNow()).ConfigureAwait(false);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        private static class Create
        {
            public static IList<UProject_ProjectSummary> UProject_ProjectSummaries(IEnumerable<MProject_Project> mProjects)
            {
                var resultList = new List<UProject_ProjectSummary>();
                foreach (var mProject in mProjects)
                {
                    resultList.Add(UProject_ProjectSummary(mProject));
                }

                return resultList;
            }

            private static UProject_ProjectSummary UProject_ProjectSummary(MProject_Project mProject)
            {
                var result = new UProject_ProjectSummary()
                {
                    ProjectId = mProject.ProjectId,
                    ProjectName = mProject.Name,
                    UpdateDateTimeUtc = mProject.UpdateDateTimeUtc
                };

                return result;
            }
        }
    }
}