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
    public interface IProjectMicroService : IEventService
    {
        // Allocation Methods
        //
        Task<long> AllocateOwnerAsync(string ownerReference);

        Task<MProject_Dashboard> GetDashboardAsync();

        // Lookup Methods
        //
        Task<IReadOnlyList<MProject_Project>> GetProjectsAsync(long? ownerId, int? skip, int? take);
        Task<MProject_Project> GetProjectAsync(Guid projectId);
        Task<MProject_Project> GetProjectAsync(long projectSnapshotId);
        Task<MProject_ProjectSnapshot> GetProjectSnapshotAsync(long projectShapshotId);
        Task<long> GetCurrentSnapshotIdAsync(Guid projectId);

        // Update Methods
        //
        Task<Guid> CreateProjectAsync(long ownerId, string name, string projectTypeCode, long designSnapshotId, MProject_ProjectSpecification projectSpecification, DateTime utcNow);
        Task<bool> RenameProjectAsync(Guid projectId, string name);
        Task<bool> UpdateProjectAsync(Guid projectId, MProject_ProjectSpecification projectSpecification, DateTime utcNow);
        Task<bool> DeleteProjectAsync(Guid projectId, DateTime utcNow);
        Task<bool> HasDeletedProjectsAsync(long ownerId);
        Task<Guid?> UndeleteProjectAsync(long ownerId, DateTime utcNow);
    }
}
