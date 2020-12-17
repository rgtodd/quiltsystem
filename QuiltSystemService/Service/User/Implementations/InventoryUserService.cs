//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using RichTodd.QuiltSystem.Design.Primitives;
using RichTodd.QuiltSystem.Service.Base;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;
using RichTodd.QuiltSystem.Service.User.Abstractions;
using RichTodd.QuiltSystem.Service.User.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.User.Implementations
{
    internal class InventoryUserService : BaseService, IInventoryUserService
    {
        private IInventoryMicroService InventoryMicroService { get; }

        public InventoryUserService(
            IApplicationRequestServices requestServices,
            ILogger<InventoryUserService> logger,
            IInventoryMicroService inventoryMicroService)
            : base(requestServices, logger)
        {
            InventoryMicroService = inventoryMicroService ?? throw new ArgumentNullException(nameof(inventoryMicroService));
        }

        #region IInventoryItemService

        public UInventory_InventoryItem GetInventoryItem(string sku)
        {
            using var log = BeginFunction(nameof(InventoryUserService), nameof(GetInventoryItem), sku);
            try
            {
                var entry = InventoryMicroService.GetEntry(sku);

                var result = Create.UInventory_InventoryItem(entry);

                log.Result(result);
                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<IReadOnlyList<UInventory_InventoryItem>> GetInventoryItemsAsync()
        {
            using var log = BeginFunction(nameof(InventoryUserService), nameof(GetInventoryItemsAsync));
            try
            {
                await Task.CompletedTask.ConfigureAwait(false);

                var entries = InventoryMicroService.GetEntries();

                var result = Create.UInventory_InventoryItems(entries);

                log.Result(result);
                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        #endregion IInventoryItemService

        private static class Create
        {
            public static IReadOnlyList<UInventory_InventoryItem> UInventory_InventoryItems(IEnumerable<MInventory_LibraryEntry> entries)
            {
                var inventoryItems = new List<UInventory_InventoryItem>();

                foreach (var entry in entries)
                {
                    inventoryItems.Add(UInventory_InventoryItem(entry));
                }

                return inventoryItems;
            }

            public static UInventory_InventoryItem UInventory_InventoryItem(MInventory_LibraryEntry entry)
            {
                //var color = Color.FromAhsb(255, entry.Hue, entry.Saturation / 100.0, entry.Value / 100.0);

                var result = new UInventory_InventoryItem()
                {
                    Id = entry.InventoryItemId,
                    Type = entry.InventoryItemTypeCode,
                    Sku = entry.Sku,
                    Name = entry.Name,
                    Manufacturer = entry.Manufacturer,
                    Collection = entry.Collection,
                    Quantity = entry.Quantity,
                    ReservedQuantity = entry.ReservedQuantity,
                    Color = MCommon_Color(entry.Hue, entry.Saturation, entry.Value)
                };

                return result;
            }

            private static MCommon_Color MCommon_Color(int hue, int saturation, int value)
            {
                var color = Color.FromAhsb(255, hue, saturation / 100.0, value / 100.0);

                var result = new MCommon_Color()
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