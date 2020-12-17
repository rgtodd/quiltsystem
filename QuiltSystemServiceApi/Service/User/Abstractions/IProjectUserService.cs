//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Threading.Tasks;

using RichTodd.QuiltSystem.Service.User.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.User.Abstractions
{
    public interface IProjectUserService
    {
        string ProjectType_Kit { get; }

        Task<string> CreateProjectAsync(string userId, string projectType, string projectName, Guid designId);

        Task<bool> DeleteProjectAsync(string userId, Guid projectId);

        Task<byte[]> GetProjectSnapshotThumbnailAsync(string userId, int projectSnapshotId, int thumbnailSize);

        Task<UProject_ProjectSummaryList> GetProjectSummariesAsync(string userId, int? skip, int? take);

        Task<byte[]> GetProjectThumbnailAsync(string userId, Guid projectId, int thumbnailSize);

        Task<bool> RenameProjectAsync(string userId, Guid projectId, string projectName);

        Task<Guid?> UndeleteProjectAsync(string userId);
    }
}