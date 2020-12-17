//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using RichTodd.QuiltSystem.Database.Domain;
using RichTodd.QuiltSystem.Database.Model;
using RichTodd.QuiltSystem.Database.Model.Extensions;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Database.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Micro.Implementations
{
    internal class InventoryMicroService : MicroService, IInventoryMicroService
    {
        private static readonly IList<MInventory_LibraryItemStockEntry> EMPTY_STOCK_LIST = new List<MInventory_LibraryItemStockEntry>();

        private readonly ICacheMicroService m_cache;

        public InventoryMicroService(
            IApplicationLocale locale,
            ILogger<InventoryMicroService> logger,
            IQuiltContextFactory quiltContextFactory,
            ICacheMicroService cache)
            : base(
                  locale,
                  logger,
                  quiltContextFactory)
        {
            m_cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task<long> CreateEntryAsync(string sku, string inventoryItemTypeCode, string name, string collection, string manufacturer, int hue, int saturation, int value, IEnumerable<string> unitOfMeasureCodes, string pricingScheduleName, DateTime utcNow)
        {
            using var ctx = QuiltContextFactory.Create();

            var dbPricingSchedule = ctx.PricingSchedules.Where(r => r.Name == pricingScheduleName).Single();

            var dbCollectionTag = ctx.Tags.Where(r => r.TagTypeCode == TagTypeCodes.Collection && r.Value == collection).SingleOrDefault();
            if (dbCollectionTag == null)
            {
                dbCollectionTag = new Tag()
                {
                    TagTypeCodeNavigation = ctx.TagType(TagTypeCodes.Collection),
                    Value = collection,
                    CreateDateTimeUtc = utcNow
                };
                _ = ctx.Tags.Add(dbCollectionTag);
            }

            var dbManufacturerTag = ctx.Tags.Where(r => r.TagTypeCode == TagTypeCodes.Manufacturer && r.Value == manufacturer).SingleOrDefault();
            if (dbManufacturerTag == null)
            {
                dbManufacturerTag = new Tag()
                {
                    TagTypeCodeNavigation = ctx.TagType(TagTypeCodes.Manufacturer),
                    Value = manufacturer,
                    CreateDateTimeUtc = utcNow
                };
                _ = ctx.Tags.Add(dbManufacturerTag);
            }

            var dbInventoryItem = ctx.InventoryItems.Where(r => r.Sku == sku).SingleOrDefault();
            if (dbInventoryItem == null)
            {
                dbInventoryItem = new InventoryItem()
                {
                    Sku = sku,
                    InventoryItemTypeCode = inventoryItemTypeCode,
                    Name = name,
                    Quantity = 0,
                    ReservedQuantity = 0,
                    Hue = hue,
                    Saturation = saturation,
                    Value = value,
                    PricingSchedule = dbPricingSchedule
                };
                _ = ctx.InventoryItems.Add(dbInventoryItem);
            }
            else
            {
                dbInventoryItem.InventoryItemTypeCode = inventoryItemTypeCode;
                dbInventoryItem.Name = name;
                //dbInventoryItem.Quantity
                //dbInventoryItem.ReservedQuantity
                dbInventoryItem.Hue = hue;
                dbInventoryItem.Saturation = saturation;
                dbInventoryItem.Value = value;
                dbInventoryItem.PricingSchedule = dbPricingSchedule;
            }

            var dbInventoryItemCollectionTag = dbInventoryItem.InventoryItemTags.Where(r => r.Tag.TagTypeCode == TagTypeCodes.Collection).SingleOrDefault();
            if (dbInventoryItemCollectionTag == null)
            {
                dbInventoryItemCollectionTag = new InventoryItemTag()
                {
                    InventoryItem = dbInventoryItem,
                    Tag = dbCollectionTag,
                    CreateDateTimeUtc = utcNow,
                };
                _ = ctx.InventoryItemTags.Add(dbInventoryItemCollectionTag);
            }
            else
            {
                dbInventoryItemCollectionTag.Tag = dbCollectionTag;
            }

            var dbInventoryItemManufacturerTag = dbInventoryItem.InventoryItemTags.Where(r => r.Tag.TagTypeCode == TagTypeCodes.Manufacturer).SingleOrDefault();
            if (dbInventoryItemManufacturerTag == null)
            {
                dbInventoryItemManufacturerTag = new InventoryItemTag()
                {
                    InventoryItem = dbInventoryItem,
                    Tag = dbManufacturerTag,
                    CreateDateTimeUtc = utcNow
                };
                _ = ctx.InventoryItemTags.Add(dbInventoryItemManufacturerTag);
            }
            else
            {
                dbInventoryItemManufacturerTag.Tag = dbManufacturerTag;
            }

            foreach (var unitOfMeasureCode in unitOfMeasureCodes)
            {
                var dbUnitOfMeasure = ctx.UnitOfMeasure(unitOfMeasureCode);
                if (!dbInventoryItem.InventoryItemUnits.Any(r => r.UnitOfMeasureCode == unitOfMeasureCode))
                {
                    var dbInventoryItemUnit = new InventoryItemUnit()
                    {
                        InventoryItem = dbInventoryItem,
                        UnitOfMeasureCodeNavigation = dbUnitOfMeasure
                    };
                    _ = ctx.InventoryItemUnits.Add(dbInventoryItemUnit);
                }
            }

            _ = await ctx.SaveChangesAsync().ConfigureAwait(false);

            InvalidateCachedEntries();

            return dbInventoryItem.InventoryItemId;
        }

        public IList<MInventory_LibraryEntry> GetEntries()
        {
            return GetCachedEntries();
        }

        public MInventory_LibraryEntry GetEntry(string sku)
        {
            return GetCachedEntries().Where(r => r.Sku == sku).SingleOrDefault();
        }

        public MInventory_LibraryEntry GetEntry(long inventoryItemId)
        {
            return GetCachedEntries().Where(r => r.InventoryItemId == inventoryItemId).SingleOrDefault();
        }

        public Task ConsumeItemsAsync(IList<long> consumableIds)
        {
            throw new NotImplementedException();
        }

        public async Task<int> ProcessEventsAsync()
        {
            using var log = BeginFunction(nameof(InventoryMicroService), nameof(ProcessEventsAsync));
            try
            {
                await Task.CompletedTask.ConfigureAwait(false);

                var result = 0;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<int> CancelEventsAsync()
        {
            using var log = BeginFunction(nameof(InventoryMicroService), nameof(CancelEventsAsync));
            try
            {
                await Task.CompletedTask.ConfigureAwait(false);

                var result = 0;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        private IList<MInventory_LibraryEntry> GetCachedEntries()
        {
            return m_cache.GetCachedEntries() ?? GetCachedEntriesSync();
        }

        private void InvalidateCachedEntries()
        {
            m_cache.SetCachedEntries(null);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private IList<MInventory_LibraryEntry> GetCachedEntriesSync()
        {
            if (m_cache.GetCachedEntries() == null)
            {
                m_cache.SetCachedEntries(LoadEntries());
            }

            return m_cache.GetCachedEntries();
        }

        private List<MInventory_LibraryEntry> LoadEntries()
        {
            //Logger.LogMessage("InventoryItemLibrary::LoadEntries");

            var entries = new List<MInventory_LibraryEntry>();

            using (var ctx = QuiltContextFactory.Create())
            {

                var dbInventoryItems = ctx.InventoryItems
                    .Include(r => r.InventoryItemTypeCodeNavigation)
                    .Include(r => r.InventoryItemStocks)
                    .Include(r => r.InventoryItemTags)
                        .ThenInclude(r => r.Tag)
                    .Where(r => r.Quantity > 0)
                    .OrderBy(r => r.Hue).ThenBy(r => r.Value).ThenBy(r => r.Saturation)
                    .ToList();

                foreach (var dbInventoryItem in dbInventoryItems)
                {
                    var dbCollectionTag = dbInventoryItem.InventoryItemTags.Where(r => r.Tag.TagTypeCode == TagTypeCodes.Collection).SingleOrDefault();
                    var dbManufacturerTag = dbInventoryItem.InventoryItemTags.Where(r => r.Tag.TagTypeCode == TagTypeCodes.Manufacturer).SingleOrDefault();

                    IList<MInventory_LibraryItemStockEntry> stocks = null;
                    foreach (var dbInventoryItemStock in dbInventoryItem.InventoryItemStocks)
                    {
                        if (stocks == null)
                        {
                            stocks = new List<MInventory_LibraryItemStockEntry>();
                        }

                        var stock = new MInventory_LibraryItemStockEntry(
                            dbInventoryItemStock.InventoryItemStockId,
                            dbInventoryItemStock.UnitOfMeasureCodeNavigation.Name,
                            dbInventoryItemStock.UnitCost,
                            dbInventoryItemStock.StockDateTimeUtc,
                            dbInventoryItemStock.OriginalQuantity,
                            dbInventoryItemStock.CurrentQuantity);

                        stocks.Add(stock);
                    }

                    var entry = new MInventory_LibraryEntry(
                        dbInventoryItem.InventoryItemId,
                        dbInventoryItem.Sku,
                        dbInventoryItem.Name,
                        dbInventoryItem.InventoryItemTypeCode,
                        dbInventoryItem.PricingScheduleId,
                        dbManufacturerTag?.Tag.Value,
                        dbCollectionTag?.Tag.Value,
                        dbInventoryItem.Quantity,
                        dbInventoryItem.ReservedQuantity,
                        dbInventoryItem.Hue,
                        dbInventoryItem.Saturation,
                        dbInventoryItem.Value,
                        stocks ?? EMPTY_STOCK_LIST);

                    entries.Add(entry);
                }
            }

            return entries;
        }
    }
}