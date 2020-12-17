//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions
{
    public interface IDesignMicroService
    {
        Task<long> AllocateOwnerAsync(string ownerReference);

        Task<MDesign_Dashboard> GetDashboardAsync();
        Task GenerateStandardBlocks();

        Task<IReadOnlyList<MDesign_Design>> GetDesignsAsync(long? ownerId, int? skip, int? take);
        Task<MDesign_Design> GetDesignAsync(Guid designId);
        Task<MDesign_Design> GetDesignAsync(int designSnapshotId);
        Task<Guid> CreateDesignAsync(long ownerId, string name, MDesign_DesignSpecification designSpecification, DateTime utcNow);
        Task<Guid> UpdateDesignAsync(Guid designId, MDesign_DesignSpecification designSpecification, DateTime utcNow);
        Task<bool> RenameDesignAsync(Guid designId, string name);
        Task<bool> DeleteDesignAsync(Guid designId, DateTime utcNow);
        Task<bool> HasDeletedDesignsAsync(long ownerId);
        Task<Guid?> UndeleteDesignAsync(long ownerId);

        Task<MDesign_FabricStyleCatalog> GetFabricStyles();
        Task<MDesign_BlockCollection> GetBlockCollectionAsync(int previewSize);

        Task<byte[]> GetBlockThumbnailAsync(string blockId, int thumbnailSize);
        Task<byte[]> GetDesignThumbnailAsync(Guid designId, int thumbnailSize);
        Task<byte[]> GetDesignSnapshotThumbnailAsync(int designSnapshotId, int thumbnailSize);
    }
}
