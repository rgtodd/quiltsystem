//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Threading.Tasks;

using RichTodd.QuiltSystem.Service.Ajax.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Ajax.Abstractions
{
    public interface IDesignAjaxService
    {
        Task<XDesign_Block[]> GetBlocksAsync(int size);
        Task<XDesign_Layout[]> GetLayoutsAsync(int rowCount, int columnCount, int size);
        Task<XDesign_FabricStyleCatalog> GetFabricStyleCatalogAsync();
        Task<XDesign_Design> GetDesignAsync(string userId, string designId);
        Task<XDesign_DesignInfo> GetDesignInfo(XDesign_Design design, int designSize, int layoutSize, int blockSize);
        Task<Guid> SaveDesignAsync(string userId, XDesign_Design design);
    }
}
