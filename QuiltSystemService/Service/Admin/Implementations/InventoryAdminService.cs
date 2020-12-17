//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using RichTodd.QuiltSystem.Design.Primitives;
using RichTodd.QuiltSystem.Service.Admin.Abstractions;
using RichTodd.QuiltSystem.Service.Admin.Abstractions.Data;
using RichTodd.QuiltSystem.Service.Base;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Admin.Implementations
{
    internal class InventoryAdminService : BaseService, IInventoryAdminService
    {
        private IInventoryMicroService InventoryMicroService { get; }

        public InventoryAdminService(
            IApplicationRequestServices requestServices,
            ILogger<InventoryAdminService> logger,
            IInventoryMicroService inventoryMicroService)
            : base(requestServices, logger)
        {
            InventoryMicroService = inventoryMicroService ?? throw new ArgumentNullException(nameof(inventoryMicroService));
        }

        #region IAdmin_InventoryService

        public async Task<AInventory_AddInventoryItemResponse> AddItem(AInventory_AddInventoryItem request)
        {
            using var log = BeginFunction(nameof(InventoryAdminService), nameof(AddItem), request);
            try
            {
                await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

                var inventoryItemId = await InventoryMicroService.CreateEntryAsync(
                    request.Sku,
                    request.InventoryItemTypeCode,
                    request.Name,
                    request.Collection,
                    request.Manufacturer,
                    request.Hue,
                    request.Saturation,
                    request.Value,
                    request.UnitOfMeasureCodeList,
                    request.PricingScheduleName,
                    GetUtcNow()).ConfigureAwait(false);

                var response = new AInventory_AddInventoryItemResponse()
                {
                    InventoryItemId = inventoryItemId
                };

                log.Result(response);
                return response;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<AInventory_InventoryItem> GetItemAsync(string sku)
        {
            using var log = BeginFunction(nameof(InventoryAdminService), nameof(GetItemAsync), sku);
            try
            {
                await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

                var entry = InventoryMicroService.GetEntry(sku);

                var result = Create.Create_InventoryItem_Data(entry);

                log.Result(result);
                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<IReadOnlyList<AInventory_InventoryItem>> GetItemsAsync()
        {
            using var log = BeginFunction(nameof(InventoryAdminService), nameof(GetItemsAsync));
            try
            {
                await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

                var entries = InventoryMicroService.GetEntries();

                var result = Create.Create_InventoryItem_DataList(entries);

                log.Result(result);
                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        #endregion IAdmin_InventoryService

        private static class Create
        {
            public static AInventory_InventoryItem Create_InventoryItem_Data(MInventory_LibraryEntry entry)
            {
                //var color = Color.FromAhsb(255, entry.Hue, entry.Saturation / 100.0, entry.Value / 100.0);

                var result = new AInventory_InventoryItem()
                {
                    Id = entry.InventoryItemId,
                    Type = entry.InventoryItemTypeCode,
                    Sku = entry.Sku,
                    Name = entry.Name,
                    Manufacturer = entry.Manufacturer,
                    Collection = entry.Collection,
                    Quantity = entry.Quantity,
                    ReservedQuantity = entry.ReservedQuantity,
                    Color = Create_Common_ColorData(entry.Hue, entry.Saturation, entry.Value),
                    InventoryItemStocks = Create_Admin_InventoryItem_StockDataList(entry.Stocks)
                };

                return result;
            }

            public static IReadOnlyList<AInventory_InventoryItem> Create_InventoryItem_DataList(IEnumerable<MInventory_LibraryEntry> entries)
            {
                var inventoryItems = new List<AInventory_InventoryItem>();

                foreach (var entry in entries)
                {
                    inventoryItems.Add(Create_InventoryItem_Data(entry));
                }

                return inventoryItems;
            }

            private static AInventory_InventoryItemStock Create_Admin_InventoryItem_StockData(MInventory_LibraryItemStockEntry entry)
            {
                var result = new AInventory_InventoryItemStock()
                {
                    InventoryItemStockId = entry.InventoryItemStockId,
                    UnitOfMeasure = entry.UnitOfMeasure,
                    UnitCost = entry.UnitCost,
                    StockDateTimeUtc = entry.StockDateTimeUtc,
                    OriginalQuantity = entry.OriginalQuantity,
                    CurrentQuantity = entry.CurrentQuantity
                };

                return result;
            }

            private static IList<AInventory_InventoryItemStock> Create_Admin_InventoryItem_StockDataList(IEnumerable<MInventory_LibraryItemStockEntry> entries)
            {
                var result = new List<AInventory_InventoryItemStock>();

                foreach (var entry in entries)
                {
                    result.Add(Create_Admin_InventoryItem_StockData(entry));
                }

                return result;
            }

            private static ACommon_Color Create_Common_ColorData(int hue, int saturation, int value)
            {
                var color = Color.FromAhsb(255, hue, saturation / 100.0, value / 100.0);

                var result = new ACommon_Color()
                {
                    Hue = hue,
                    Saturation = saturation,
                    Value = value,
                    WebColor = color.WebColor
                };

                return result;
            }

        }
    }
}