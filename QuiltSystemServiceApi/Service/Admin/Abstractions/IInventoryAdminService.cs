//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;
using System.Threading.Tasks;

using RichTodd.QuiltSystem.Service.Admin.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Admin.Abstractions
{
    public interface IInventoryAdminService
    {
        Task<IReadOnlyList<AInventory_InventoryItem>> GetItemsAsync();
        Task<AInventory_InventoryItem> GetItemAsync(string sku);
        Task<AInventory_AddInventoryItemResponse> AddItem(AInventory_AddInventoryItem request);
    }
}