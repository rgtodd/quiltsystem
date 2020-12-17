//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using RichTodd.QuiltSystem.Business.ComponentProviders;
using RichTodd.QuiltSystem.Design.Component.Standard;
using RichTodd.QuiltSystem.Design.Primitives;
using RichTodd.QuiltSystem.Service.Ajax.Implementations;
using RichTodd.QuiltSystem.Service.Base;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;
using RichTodd.QuiltSystem.Service.User.Abstractions;
using RichTodd.QuiltSystem.Service.User.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.User.Implementations
{
    internal class DesignUserService : BaseService, IDesignUserService
    {
        private IDesignMicroService DesignMicroService { get; }

        public DesignUserService(
            IApplicationRequestServices requestServices,
            ILogger<DesignUserService> logger,
            IDesignMicroService designMicroService)
            : base(requestServices, logger)
        {
            DesignMicroService = designMicroService ?? throw new ArgumentNullException(nameof(designMicroService));
        }

        public async Task<string> CreateDesignAsync(string userId, string designName)
        {
            using var log = BeginFunction(nameof(DesignUserService), nameof(CreateDesignAsync), userId, designName);
            try
            {
                await Assert(SecurityPolicy.IsAuthorized, userId).ConfigureAwait(false);

                var fabricStyles = new FabricStyleList
                {
                    new FabricStyle(FabricStyle.UNKNOWN_SKU, Color.Red),
                    new FabricStyle(FabricStyle.UNKNOWN_SKU, Color.Green),
                    new FabricStyle(FabricStyle.UNKNOWN_SKU, Color.Blue)
                };

                var provider = new BuiltInQuiltLayoutComponenProvider();
                var entry = provider.GetComponent(LayoutComponent.TypeName, Constants.DefaultComponentCategory, BuiltInQuiltLayoutComponenProvider.ComponentName_Checkerboard);

                var design = new Design.Core.Design()
                {
                    Width = new Dimension(48, DimensionUnits.Inch),
                    Height = new Dimension(48, DimensionUnits.Inch)
                };

                var component = LayoutComponent.Create(entry.Category, entry.Name, fabricStyles, 3, 3, entry.BlockCount);
                design.LayoutComponent = component;

                var ownerReference = CreateOwnerReference.FromUserId(userId);
                var ownerId = await DesignMicroService.AllocateOwnerAsync(ownerReference).ConfigureAwait(false);

                var mDesignSpecification = BusinessDataFactory.Create_MDesign_DesignSpecification(design);

                var id = await DesignMicroService.CreateDesignAsync(ownerId, designName, mDesignSpecification, GetUtcNow()).ConfigureAwait(false);

                var result = id.ToString();

                log.Result(result);
                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<bool> DeleteDesignAsync(string userId, Guid designId)
        {
            using var log = BeginFunction(nameof(DesignUserService), nameof(DeleteDesignAsync), userId, designId);
            try
            {
                await Assert(SecurityPolicy.IsAuthorized, userId).ConfigureAwait(false);

                var result = await DesignMicroService.DeleteDesignAsync(designId, GetUtcNow()).ConfigureAwait(false);

                log.Result(result);
                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<UDesign_Design> GetDesignAsync(string userId, Guid designId)
        {
            using var log = BeginFunction(nameof(DesignUserService), nameof(GetDesignAsync), userId, designId);
            try
            {
                await Assert(SecurityPolicy.IsAuthorized, userId).ConfigureAwait(false);

                var mDesign = await DesignMicroService.GetDesignAsync(designId).ConfigureAwait(false);
                if (mDesign == null)
                {
                    log.Result(null);
                    return null;
                }

                var result = Create.UDesign_Design(mDesign);

                log.Result(result);
                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<byte[]> GetDesignSnapshotThumbnailAsync(string userId, int designSnapshotId, int thumbnailSize)
        {
            using var log = BeginFunction(nameof(DesignUserService), nameof(GetDesignSnapshotThumbnailAsync), userId, designSnapshotId, thumbnailSize);
            try
            {
                await Assert(SecurityPolicy.IsAuthorized, userId).ConfigureAwait(false);

                var result = await DesignMicroService.GetDesignSnapshotThumbnailAsync(designSnapshotId, thumbnailSize).ConfigureAwait(false);

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<UDesign_DesignSummaryList> GetDesignSummariesAsync(string userId, int? skip, int? take)
        {
            using var log = BeginFunction(nameof(DesignUserService), nameof(GetDesignSummariesAsync), userId, skip, take);
            try
            {
                await Assert(SecurityPolicy.IsAuthorized, userId).ConfigureAwait(false);

                var ownerReference = CreateOwnerReference.FromUserId(userId);
                var ownerId = await DesignMicroService.AllocateOwnerAsync(ownerReference).ConfigureAwait(false);

                var mDesigns = await DesignMicroService.GetDesignsAsync(ownerId, skip, take).ConfigureAwait(false);
                var hasDeletedDesigns = await DesignMicroService.HasDeletedDesignsAsync(ownerId).ConfigureAwait(false);

                var summaries = Create.UDesign_DesignSummarys(mDesigns);

                var result = new UDesign_DesignSummaryList()
                {
                    Summaries = summaries,
                    HasDeletedDesigns = hasDeletedDesigns
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

        public async Task<byte[]> GetDesignThumbnailAsync(string userId, Guid designId, int thumbnailSize)
        {
            using var log = BeginFunction(nameof(DesignUserService), nameof(GetDesignThumbnailAsync), userId, designId, thumbnailSize);
            try
            {
                await Assert(SecurityPolicy.IsAuthorized, userId).ConfigureAwait(false);

                var result = await DesignMicroService.GetDesignThumbnailAsync(designId, thumbnailSize).ConfigureAwait(false);

                log.Result(result);
                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<bool> RenameDesignAsync(string userId, Guid designId, string designName)
        {
            using var log = BeginFunction(nameof(DesignUserService), nameof(RenameDesignAsync), userId, designId, designName);
            try
            {
                await Assert(SecurityPolicy.IsAuthorized, userId).ConfigureAwait(false);

                var result = await DesignMicroService.RenameDesignAsync(designId, designName).ConfigureAwait(false);

                log.Result(result);
                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<string> UndeleteDesignAsync(string userId)
        {
            using var log = BeginFunction(nameof(DesignUserService), nameof(UndeleteDesignAsync), userId, userId);
            try
            {
                await Assert(SecurityPolicy.IsAuthorized, userId).ConfigureAwait(false);

                var ownerReference = CreateOwnerReference.FromUserId(userId);
                var ownerId = await DesignMicroService.AllocateOwnerAsync(ownerReference).ConfigureAwait(false);

                var id = await DesignMicroService.UndeleteDesignAsync(ownerId).ConfigureAwait(false);
                if (id.HasValue)
                {
                    var result = id.Value.ToString();

                    log.Result(result);
                    return result;
                }

                log.Result(null);
                return null;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        private static class Create
        {
            public static IList<UDesign_DesignSummary> UDesign_DesignSummarys(IEnumerable<MDesign_Design> mDesigns)
            {
                var summaries = new List<UDesign_DesignSummary>();

                foreach (var mDesign in mDesigns)
                {
                    summaries.Add(UDesign_DesignSummary(mDesign));
                }

                return summaries;
            }

            private static UDesign_DesignSummary UDesign_DesignSummary(MDesign_Design mDesign)
            {
                var result = new UDesign_DesignSummary()
                {
                    DesignId = mDesign.DesignId,
                    DesignName = mDesign.Name,
                    UpdateDateTimeUtc = mDesign.UpdateDateTimeUtc
                };

                return result;
            }

            public static UDesign_Design UDesign_Design(MDesign_Design mDesign)
            {
                var result = new UDesign_Design()
                {
                    DesignId = mDesign.DesignId,
                    DesignName = mDesign.Name
                };

                return result;
            }
        }
    }
}