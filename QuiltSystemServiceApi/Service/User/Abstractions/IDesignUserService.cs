//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Threading.Tasks;

using RichTodd.QuiltSystem.Service.User.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.User.Abstractions
{
    public interface IDesignUserService
    {
        Task<string> CreateDesignAsync(string userId, string designName);

        Task<bool> DeleteDesignAsync(string userId, Guid designId);

        Task<UDesign_Design> GetDesignAsync(string userId, Guid designId);

        Task<byte[]> GetDesignSnapshotThumbnailAsync(string userId, int designSnapshotId, int thumbnailSize);

        Task<UDesign_DesignSummaryList> GetDesignSummariesAsync(string userId, int? skip, int? take);

        Task<byte[]> GetDesignThumbnailAsync(string userId, Guid designId, int thumbnailSize);

        Task<bool> RenameDesignAsync(string userId, Guid designId, string designName);

        Task<string> UndeleteDesignAsync(string userId);
    }
}