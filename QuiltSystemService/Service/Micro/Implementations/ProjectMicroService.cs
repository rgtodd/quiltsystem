//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using RichTodd.QuiltSystem.Database.Model;
using RichTodd.QuiltSystem.Database.Model.Extensions;
using RichTodd.QuiltSystem.Service.Base;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Database.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;
using RichTodd.QuiltSystem.Utility;

namespace RichTodd.QuiltSystem.Service.Micro.Implementations
{
    internal class ProjectMicroService : MicroService, IProjectMicroService
    {
        private IProjectEventMicroService ProjectEventService { get; }
        private IInventoryMicroService InventoryMicroService { get; }

        public ProjectMicroService(
            IApplicationLocale locale,
            ILogger<ProjectMicroService> logger,
            IQuiltContextFactory quiltContextFactory,
            IProjectEventMicroService projectEventService,
            IInventoryMicroService inventoryMicroService)
            : base(
                  locale,
                  logger,
                  quiltContextFactory)
        {
            ProjectEventService = projectEventService ?? throw new ArgumentNullException(nameof(projectEventService));
            InventoryMicroService = inventoryMicroService ?? throw new ArgumentNullException(nameof(inventoryMicroService));
        }

        public async Task<long> AllocateOwnerAsync(string ownerReference)
        {
            using var log = BeginFunction(nameof(ProjectMicroService), nameof(AllocateOwnerAsync), ownerReference);
            try
            {
                using var ctx = QuiltContextFactory.Create();

                var dbOwner = await ctx.Owners.Where(r => r.OwnerReference == ownerReference).SingleOrDefaultAsync().ConfigureAwait(false);
                if (dbOwner == null)
                {
                    dbOwner = new Owner()
                    {
                        OwnerReference = ownerReference,
                        OwnerTypeCode = "A"
                    };
                    _ = ctx.Owners.Add(dbOwner);

                    _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
                }

                var result = dbOwner.OwnerId;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MProject_Dashboard> GetDashboardAsync()
        {
            using var log = BeginFunction(nameof(ProjectMicroService), nameof(GetDashboardAsync));
            try
            {
                using var ctx = CreateQuiltContext();

                var dashboard = new MProject_Dashboard()
                {
                    TotalProjects = await ctx.Projects.CountAsync()
                };

                var result = dashboard;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<IReadOnlyList<MProject_Project>> GetProjectsAsync(long? ownerId, int? skip, int? take)
        {
            var projects = new List<MProject_Project>();

            using var ctx = QuiltContextFactory.Create();

            var dbProjects = ctx.Projects.Where(r => r.DeleteDateTimeUtc == null);
            if (ownerId.HasValue)
            {
                dbProjects = dbProjects.Where(r => r.OwnerId == ownerId.Value);
            }
            dbProjects = dbProjects.OrderByDescending(r => r.UpdateDateTimeUtc);
            if (skip.HasValue)
            {
                dbProjects = dbProjects.Skip(skip.Value);
            }
            if (take.HasValue)
            {
                dbProjects = dbProjects.Take(take.Value);
            }

            var dbProjectList = await dbProjects.ToListAsync().ConfigureAwait(false);

            foreach (var dbProject in dbProjectList)
            {
                var dbProjectSnapshot = dbProject.ProjectSnapshots.Where(r => r.ProjectSnapshotSequence == dbProject.CurrentProjectSnapshotSequence).Single();

                var project = Create.MProject_Project(dbProjectSnapshot);

                projects.Add(project);
            }

            return projects;
        }

        public async Task<MProject_Project> GetProjectAsync(Guid projectId)
        {
            using var ctx = QuiltContextFactory.Create();

            var projectSnapshotId = await ctx.Projects
                .Where(r => r.ProjectId == projectId)
                .Join(
                    ctx.ProjectSnapshots,
                    project => new { project.ProjectId, Sequence = project.CurrentProjectSnapshotSequence },
                    projectSnapshot => new { projectSnapshot.ProjectId, Sequence = projectSnapshot.ProjectSnapshotSequence },
                    (project, projectSnapshot) => projectSnapshot.ProjectSnapshotId)
                .SingleAsync().ConfigureAwait(false);

            var mProject = await ctx.ProjectSnapshots
                 .Include(r => r.Artifact)
                 .Include(r => r.DesignSnapshot)
                     .ThenInclude(r => r.Artifact)
                 .Include(r => r.ProjectSnapshotComponents)
                 .Where(r => r.ProjectSnapshotId == projectSnapshotId)
                 .Select(r => Create.MProject_Project(r))
                 .SingleOrDefaultAsync().ConfigureAwait(false);

            return mProject;
        }

        public async Task<MProject_Project> GetProjectAsync(long projectSnapshotId)
        {
            using var ctx = QuiltContextFactory.Create();

            var mProject = await ctx.ProjectSnapshots
                .Include(r => r.Artifact)
                .Include(r => r.DesignSnapshot)
                    .ThenInclude(r => r.Artifact)
                .Include(r => r.ProjectSnapshotComponents)
                .Where(r => r.ProjectSnapshotId == projectSnapshotId)
                .Select(r => Create.MProject_Project(r))
                .SingleOrDefaultAsync().ConfigureAwait(false);

            return mProject;
        }

        public async Task<MProject_ProjectSnapshot> GetProjectSnapshotAsync(long projectShapshotId)
        {
            using var log = BeginFunction(nameof(ProjectMicroService), nameof(GetProjectSnapshotAsync), projectShapshotId);
            try
            {
                using var ctx = CreateQuiltContext();

                var dbProjectSnapshot = await ctx.ProjectSnapshots
                    .Where(r => r.ProjectSnapshotId == projectShapshotId)
                    .Include(r => r.ProjectSnapshotComponents)
                    .FirstAsync()
                    .ConfigureAwait(false);

                var dbProject = await ctx.Projects
                    .Where(r => r.ProjectId == dbProjectSnapshot.ProjectId)
                    .FirstAsync()
                    .ConfigureAwait(false);

                var totalPrice = 0m;

                var projectSnapshotComponents = new List<MProject_ProjectSnapshotComponent>();
                foreach (var dbProjectSnapshotComponent in dbProjectSnapshot.ProjectSnapshotComponents)
                {
                    var inventoryItemId = ParseInventoryItemId.FromConsumableReference(dbProjectSnapshotComponent.ConsumableReference);
                    var mInventoryItem = InventoryMicroService.GetEntry(inventoryItemId);

                    var dbInventoryItemType = CachedInventoryItemTypes.Where(r => r.InventoryItemTypeCode == mInventoryItem.InventoryItemTypeCode).First();

                    var dbPricingSchedule = CachedPricingSchedules.Where(r => r.PricingScheduleId == mInventoryItem.PricingScheduleId).First();
                    var dbPricingScheduleEntry = dbPricingSchedule.PricingScheduleEntries.Where(r => r.UnitOfMeasureCode == dbProjectSnapshotComponent.UnitOfMeasureCode).First();

                    var dbUnitOfMeasure = CachedUnitOfMeasures.Where(r => r.UnitOfMeasureCode == dbProjectSnapshotComponent.UnitOfMeasureCode).First();

                    var unitPrice = dbPricingScheduleEntry.Price;
                    var componentPrice = unitPrice * dbProjectSnapshotComponent.Quantity;
                    totalPrice += componentPrice;

                    projectSnapshotComponents.Add(Create.MProject_ProjectSnapshotComponent(dbProjectSnapshotComponent, mInventoryItem, dbInventoryItemType, dbUnitOfMeasure, unitPrice, componentPrice));
                }

                var projectSnapshot = new MProject_ProjectSnapshot()
                {
                    ProjectId = dbProject.ProjectId,
                    ProjectNumber = dbProject.ProjectNumber,
                    ProjectSnapshotId = dbProjectSnapshot.ProjectSnapshotId,
                    Name = dbProjectSnapshot.Name,
                    Price = totalPrice,
                    Sku = dbProjectSnapshot.Project.ProjectNumber + "." + dbProjectSnapshot.ProjectSnapshotSequence.ToString(),
                    Components = projectSnapshotComponents
                };

                var result = projectSnapshot;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<long> GetCurrentSnapshotIdAsync(Guid projectId)
        {
            using var log = BeginFunction(nameof(ProjectMicroService), nameof(GetCurrentSnapshotIdAsync), projectId);
            try
            {
                using var ctx = CreateQuiltContext();

                // Find the current project snapshot associated with the specified project ID.
                //
                var dbProjectSnapshots =
                    from p in ctx.Projects
                    join ps in ctx.ProjectSnapshots
                        on new { p.ProjectId, ProjectSnapshotSequence = p.CurrentProjectSnapshotSequence } equals new { ps.ProjectId, ps.ProjectSnapshotSequence }
                    where p.ProjectId == projectId
                    select ps;
                var dbProjectSnapshot = await dbProjectSnapshots.SingleAsync().ConfigureAwait(false);

                var result = dbProjectSnapshot.ProjectSnapshotId;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<Guid> CreateProjectAsync(long ownerId, string name, string projectTypeCode, long designSnapshotId, MProject_ProjectSpecification projectSpecification, DateTime utcNow)
        {
            //if (SecurityPolicy.IsBuiltInUser(m_userId))
            //{
            //    throw new InvalidOperationException("Built-in user ID not supported.");
            //}

            try
            {
                using var ctx = QuiltContextFactory.Create();

                var projectNumber = ctx.GetProjectNumber(utcNow);

                var dbProject = new Project()
                {
                    ProjectId = Guid.NewGuid(),
                    OwnerId = ownerId,
                    Name = name,
                    CurrentProjectSnapshotSequence = 0,
                    ProjectNumber = projectNumber,
                    CreateDateTimeUtc = utcNow,
                    UpdateDateTimeUtc = utcNow,
                    ProjectTypeCode = projectTypeCode,
                };
                _ = ctx.Projects.Add(dbProject);

                CreateProjectSnapshot(ctx, dbProject, designSnapshotId, projectSpecification, utcNow);

                _ = await ctx.SaveChangesAsync().ConfigureAwait(false);

                return dbProject.ProjectId;
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log(ex);
                throw;
            }
        }

        public async Task<bool> RenameProjectAsync(Guid projectId, string name)
        {
            using var ctx = QuiltContextFactory.Create();

            var dbProject = await ctx.Projects.Where(r => r.ProjectId == projectId).SingleOrDefaultAsync().ConfigureAwait(false);
            if (dbProject == null)
            {
                return false;
            }

            //var ownerUserId = ParseUserId.FromOwnerReference(dbProject.Owner.OwnerReference);
            //if (!SecurityPolicy.IsAuthorized(userId, ownerUserId))
            //{
            //    return false;
            //}

            var dbProjectSnapshot = dbProject.ProjectSnapshots.Where(r => r.ProjectSnapshotSequence == dbProject.CurrentProjectSnapshotSequence).Single();

            dbProject.Name = name;
            dbProjectSnapshot.Name = name;

            _ = await ctx.SaveChangesAsync().ConfigureAwait(false);

            return true;
        }

        public async Task<bool> UpdateProjectAsync(Guid projectId, MProject_ProjectSpecification projectSpecification, DateTime utcNow)
        {
            //if (SecurityPolicy.IsBuiltInUser(userId))
            //{
            //    throw new InvalidOperationException("Built-in user ID not supported.");
            //}

            try
            {
                using var ctx = QuiltContextFactory.Create();

                var dbProject = await ctx.Projects.Where(r => r.ProjectId == projectId).SingleOrDefaultAsync().ConfigureAwait(false);
                if (dbProject == null)
                {
                    return false;
                }

                //var ownerUserId = ParseUserId.FromOwnerReference(dbProject.Owner.OwnerReference);
                //if (!SecurityPolicy.IsAuthorized(userId, ownerUserId))
                //{
                //    return false;
                //}

                var dbCurrentProjectSnapshot = dbProject.ProjectSnapshots.Where(r => r.ProjectSnapshotSequence == dbProject.CurrentProjectSnapshotSequence).Single();

                dbProject.CurrentProjectSnapshotSequence += 1;
                dbProject.UpdateDateTimeUtc = utcNow;

                CreateProjectSnapshot(ctx, dbProject, dbCurrentProjectSnapshot.DesignSnapshotId, projectSpecification, utcNow);

                _ = await ctx.SaveChangesAsync().ConfigureAwait(false);

                return true;
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log(ex);
                throw;
            }
        }

        public async Task<bool> DeleteProjectAsync(Guid projectId, DateTime utcNow)
        {
            using var ctx = QuiltContextFactory.Create();

            var dbProject = await ctx.Projects.Where(r => r.ProjectId == projectId).SingleOrDefaultAsync().ConfigureAwait(false);
            if (dbProject == null)
            {
                return false;
            }
            //var ownerUserId = ParseUserId.FromOwnerReference(dbProject.Owner.OwnerReference);
            //if (!SecurityPolicy.IsAuthorized(userId, ownerUserId))
            //{
            //    return false;
            //}

            dbProject.DeleteDateTimeUtc = utcNow;

            _ = await ctx.SaveChangesAsync().ConfigureAwait(false);

            return true;
        }

        public async Task<bool> HasDeletedProjectsAsync(long ownerId)
        {
            //if (SecurityPolicy.IsBuiltInUser(userId))
            //{
            //    throw new InvalidOperationException("Built-in user ID not supported.");
            //}

            using var ctx = QuiltContextFactory.Create();

            var result = await ctx.Projects.AnyAsync(r => r.OwnerId == ownerId && r.DeleteDateTimeUtc != null).ConfigureAwait(false);
            return result;
        }

        public async Task<Guid?> UndeleteProjectAsync(long ownerId, DateTime utcNow)
        {
            //if (SecurityPolicy.IsBuiltInUser(userId))
            //{
            //    throw new InvalidOperationException("Built-in user ID not supported.");
            //}

            using var ctx = QuiltContextFactory.Create();

            var dbProject = await (from p in ctx.Projects
                                   where p.DeleteDateTimeUtc != null && p.OwnerId == ownerId
                                   orderby p.DeleteDateTimeUtc descending
                                   select p).FirstOrDefaultAsync().ConfigureAwait(false);

            if (dbProject != null)
            {
                dbProject.DeleteDateTimeUtc = null;

                _ = await ctx.SaveChangesAsync().ConfigureAwait(false);

                return dbProject.ProjectId;
            }

            return null;
        }

        public async Task<int> ProcessEventsAsync()
        {
            using var log = BeginFunction(nameof(ProjectMicroService), nameof(ProcessEventsAsync));
            try
            {
                await Task.CompletedTask.ConfigureAwait(false);

                var result = 0;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<int> CancelEventsAsync()
        {
            using var log = BeginFunction(nameof(ProjectMicroService), nameof(CancelEventsAsync));
            try
            {
                await Task.CompletedTask.ConfigureAwait(false);

                var result = 0;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        private void CreateProjectSnapshot(QuiltContext ctx, Project dbProject, long designSnapshotId, MProject_ProjectSpecification data, DateTime utcNow)
        {
            var dbArtifact = new Artifact()
            {
                ArtifactTypeCode = data.ProjectArtifactTypeCode, // ArtifactTypes.Kit
                ArtifactValueTypeCode = data.ProjectArtifactValueTypeCode, // ArtifactValueTypes.Json
                Value = data.ProjectArtifactValue
            };
            _ = ctx.Artifacts.Add(dbArtifact);

            var dbProjectSnapshot = new ProjectSnapshot()
            {
                Project = dbProject,
                ProjectSnapshotSequence = dbProject.CurrentProjectSnapshotSequence,
                Name = dbProject.Name,
                DesignSnapshotId = designSnapshotId,
                Artifact = dbArtifact,
                CreateDateTimeUtc = utcNow,
                UpdateDateTimeUtc = utcNow,
            };
            _ = ctx.ProjectSnapshots.Add(dbProjectSnapshot);

            foreach (var component in data.Components)
            {
                var mInventoryItem = InventoryMicroService.GetEntry(component.Sku);
                var consumableReference = CreateConsumableReference.FromInventoryItemId(mInventoryItem.InventoryItemId);

                var dbProjectSnapshotComponent = dbProjectSnapshot.ProjectSnapshotComponents.Where(r =>
                    r.ConsumableReference == consumableReference &&
                    r.UnitOfMeasureCodeNavigation == ctx.UnitOfMeasure(component.UnitOfMeasureCode)).SingleOrDefault();

                if (dbProjectSnapshotComponent != null)
                {
                    dbProjectSnapshotComponent.Quantity += component.Quantity;
                }
                else
                {
                    dbProjectSnapshotComponent = new ProjectSnapshotComponent()
                    {
                        ProjectSnapshot = dbProjectSnapshot,
                        ProjectSnapshotComponentSequence = dbProjectSnapshot.ProjectSnapshotComponents.Count + 1,
                        ConsumableReference = consumableReference,
                        UnitOfMeasureCode = component.UnitOfMeasureCode,
                        Quantity = component.Quantity,
                    };
                    _ = ctx.ProjectSnapshotComponents.Add(dbProjectSnapshotComponent);
                }
            }
        }

        private IList<InventoryItemType> m_cachedInventoryItemTypes;
        private IList<InventoryItemType> CachedInventoryItemTypes
        {
            get
            {
                if (m_cachedInventoryItemTypes == null)
                {
                    using var ctx = CreateQuiltContext();

                    m_cachedInventoryItemTypes = ctx.InventoryItemTypes.ToList();
                }

                return m_cachedInventoryItemTypes;
            }
        }

        private IList<PricingSchedule> m_cachedPricingSchedules;
        private IList<PricingSchedule> CachedPricingSchedules
        {
            get
            {
                if (m_cachedPricingSchedules == null)
                {
                    using var ctx = CreateQuiltContext();

                    m_cachedPricingSchedules = ctx.PricingSchedules
                        .Include(r => r.PricingScheduleEntries)
                        .ToList();
                }

                return m_cachedPricingSchedules;
            }
        }

        private IList<UnitOfMeasure> m_cachedUnitOfMeasures;
        private IList<UnitOfMeasure> CachedUnitOfMeasures
        {
            get
            {
                if (m_cachedUnitOfMeasures == null)
                {
                    using var ctx = CreateQuiltContext();

                    m_cachedUnitOfMeasures = ctx.UnitOfMeasures.ToList();
                }

                return m_cachedUnitOfMeasures;
            }
        }

        private static class Create
        {
            public static MProject_Project Create_MProject_Project(Guid projectId, long projectSnapshotId, string name, DateTime updateDateTimeUtc, string designArtifactValue, string projectArtifcatTypeCode, string projectArtifactValueTypeCode, string projectArtifactValue)
            {
                var projectSpecification = new MProject_ProjectSpecification(
                    designArtifactValue,
                    projectArtifcatTypeCode,
                    projectArtifactValueTypeCode,
                    projectArtifactValue,
                    new List<MProject_ProjectSpecificationComponent>());

                var project = new MProject_Project(
                    projectId,
                    projectSnapshotId,
                    name,
                    updateDateTimeUtc,
                    projectSpecification);

                return project;
            }

            public static MProject_Project MProject_Project(ProjectSnapshot dbProjectSnapshot)
            {
                var projectSpecificationComponents = new List<MProject_ProjectSpecificationComponent>();
                foreach (var dbProjectSnapshotComponent in dbProjectSnapshot.ProjectSnapshotComponents.ToList())
                {
                    projectSpecificationComponents.Add(
                        new MProject_ProjectSpecificationComponent(
                            dbProjectSnapshotComponent.ConsumableReference,
                            dbProjectSnapshotComponent.UnitOfMeasureCode,
                            dbProjectSnapshotComponent.Quantity));
                }

                var projectSpecification = new MProject_ProjectSpecification(
                    dbProjectSnapshot.DesignSnapshot.Artifact.Value,
                    dbProjectSnapshot.Artifact.ArtifactTypeCode,
                    dbProjectSnapshot.Artifact.ArtifactValueTypeCode,
                    dbProjectSnapshot.Artifact.Value,
                    projectSpecificationComponents);

                var project = new MProject_Project(
                    dbProjectSnapshot.ProjectId,
                    dbProjectSnapshot.ProjectSnapshotId,
                    dbProjectSnapshot.Name,
                    dbProjectSnapshot.UpdateDateTimeUtc,
                    projectSpecification);

                return project;
            }

            public static MProject_ProjectSnapshotComponent MProject_ProjectSnapshotComponent(ProjectSnapshotComponent dbProjectSnapshotComponent, MInventory_LibraryEntry mInventoryItem, InventoryItemType dbInventoryItemType, UnitOfMeasure dbUnitOfMeasure, decimal unitPrice, decimal totalPrice)
            {
                return new MProject_ProjectSnapshotComponent()
                {
                    ProjectSnapshotComponentId = dbProjectSnapshotComponent.ProjectSnapshotComponentId,
                    Description = mInventoryItem.Name,
                    Quantity = dbProjectSnapshotComponent.Quantity,
                    UnitPrice = unitPrice,
                    TotalPrice = totalPrice,
                    ConsumableReference = CreateConsumableReference.FromInventoryItemId(mInventoryItem.InventoryItemId),
                    Sku = mInventoryItem.Sku,
                    UnitOfMeasure = GetValue.MCommon_UnitOfMeasure(dbUnitOfMeasure.UnitOfMeasureCode),
                    UnitOfMeasureName = dbUnitOfMeasure.Name,
                    Category = dbInventoryItemType.Name,
                    Collection = mInventoryItem.Collection,
                    Manufacturer = mInventoryItem.Manufacturer
                };
            }
        }
    }
}
