//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Threading.Tasks;

using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions
{
    public interface IKitMicroService
    {
        Task<MKit_Kit> GetKitDetailAsync(string userId, Guid projectId, int thumbnailSize);

        Task<MKit_Kit> GetKitDetailAsync(string userId, long projectSnapshotId, int thumbnailSize);

        Task<MKit_Kit> GetKitDetailFromDesignAsync(string userId, Guid designId, int thumbnailSize);

        Task<MKit_Kit> GetKitDetailPreviewAsync(string userId, Guid projectId, int thumbnailSize, MKit_KitSpecificationUpdate specificationUpdate);

        Task<MKit_Kit> GetKitDetailPreviewFromDesignAsync(string userId, Guid designId, int thumbnailSize, MKit_KitSpecificationUpdate specificationUpdate);

        Task<bool> SaveKitAsync(string userId, MKit_Kit kit);

        Task<MKit_UpdateSpecificationResponse> UpdateKitSpecificationAsync(string userId, Guid projectId, MKit_KitSpecificationUpdate specificationUpdate);
    }
}