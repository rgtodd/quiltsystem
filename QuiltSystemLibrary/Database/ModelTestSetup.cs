//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

using RichTodd.QuiltSystem.Database.Builders;
using RichTodd.QuiltSystem.Database.Domain;
using RichTodd.QuiltSystem.Database.Model;
using RichTodd.QuiltSystem.Database.Model.Extensions;
using RichTodd.QuiltSystem.Service.Database.Abstractions;
using RichTodd.QuiltSystem.Utility;

namespace RichTodd.QuiltSystem.Database
{
    public static class ModelTestSetup
    {
        private const decimal TestInventoryUnitCost = 2.00m;
        private const string TestLedgerAccountTransactionDescription = "Test Cash Deposit";
        private const string TestPricingScheduleName = "Default";

        public static async Task CreateTestEntitiesAsync(IConfiguration configuration, IQuiltContextFactory quiltContextFactory, DateTime utcNow, DateTime localNow)
        {
            CreateTestAccountingYears(quiltContextFactory);
            CreateTestPricingSchedules(quiltContextFactory);
            CreateTestLedgerAccountTransactions(quiltContextFactory, utcNow, localNow);
            await CreateTestInventoryItemsAsync(configuration, quiltContextFactory, utcNow).ConfigureAwait(false);
            await CreateBuiltInInventoryItemsAsync(quiltContextFactory, utcNow).ConfigureAwait(false);
            CreateTestStockTransactions(quiltContextFactory, utcNow, localNow);
        }

        private static async Task CreateBuiltInInventoryItemsAsync(IQuiltContextFactory quiltContextFactory, DateTime utcNow)
        {
            var unitOfMeasureCodes = new List<string>
            {
                UnitOfMeasureCodes.FatQuarter,
                UnitOfMeasureCodes.HalfYardage,
                UnitOfMeasureCodes.Yardage,
                UnitOfMeasureCodes.TwoYards,
                UnitOfMeasureCodes.ThreeYards
            };

            for (var hue = 0; hue < 360; hue += 30)
            {
                for (var saturation = 20; saturation <= 100; saturation += 20)
                {
                    for (var value = 10; value <= 100; value += 20)
                    {
                        var sku = string.Format("WEB-{0:000}-{1:000}-{2:000}", hue, saturation, value);

                        _ = await CreateEntryAsync(
                            quiltContextFactory,
                            sku,
                            InventoryItemTypeCodes.Fabric,
                            sku,
                            "Web Colors", // Collection
                            "Built In", // Manufacturer
                            hue,
                            saturation,
                            value,
                            unitOfMeasureCodes,
                            TestPricingScheduleName,
                            utcNow).ConfigureAwait(false);
                    }
                }
            }
        }

        private static void CreateTestAccountingYears(IQuiltContextFactory quiltContextFactory)
        {
            using var ctx = quiltContextFactory.Create();

            var year = DateTime.Now.Year;

            var dbAccountingYear = ctx.AccountingYears.Where(r => r.Year == year).SingleOrDefault();
            if (dbAccountingYear == null)
            {
                dbAccountingYear = new AccountingYear()
                {
                    Year = year,
                    AccountingYearStatusCode = AccountingYearStatusTypeCodes.Open
                };
                _ = ctx.AccountingYears.Add(dbAccountingYear);

                _ = ctx.SaveChanges();
            }
        }

        private static async Task CreateTestInventoryItemsAsync(IConfiguration configuration, IQuiltContextFactory quiltContextFactory, DateTime utcNow)
        {
            var unitOfMeasureCodes = new List<string>
            {
                UnitOfMeasureCodes.FatQuarter,
                UnitOfMeasureCodes.HalfYardage,
                UnitOfMeasureCodes.Yardage,
                UnitOfMeasureCodes.TwoYards,
                UnitOfMeasureCodes.ThreeYards
            };

            var text = AzureUtility.LoadAzureStringBlob(configuration, "fabrics-default", "index.csv");
            var lines = text.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var fields = line.Split(new char[] { ',' });

                var sku = fields[0];
                var name = fields[1];

                //var entry = library.GetEntry(sku);
                //if (entry == null)
                //{
                var image = (Bitmap)AzureUtility.LoadAzureImageBlob(configuration, "fabrics-default", sku + ".jpg");

                var averageColor = GetAverageColor(image);

                _ = await CreateEntryAsync(
                    quiltContextFactory,
                    sku,
                    InventoryItemTypeCodes.Fabric,
                    name,
                    "Kona Cotton Solid", // Collection
                    "Robert Kaufman", // Manufacturer
                    (int)averageColor.GetHue(),
                    (int)(averageColor.GetSaturation() * 100),
                    (int)(averageColor.GetBrightness() * 100),
                    unitOfMeasureCodes,
                    TestPricingScheduleName,
                    utcNow).ConfigureAwait(false);
                //}
            }
        }

        public static async Task<long> CreateEntryAsync(IQuiltContextFactory quiltContextFactory, string sku, string inventoryItemTypeCode, string name, string collection, string manufacturer, int hue, int saturation, int value, IEnumerable<string> unitOfMeasureCodes, string pricingScheduleName, DateTime utcNow)
        {
            using var ctx = quiltContextFactory.Create();

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

            return dbInventoryItem.InventoryItemId;
        }

        private static void CreateTestLedgerAccountTransactions(IQuiltContextFactory quiltContextFactory, DateTime utcNow, DateTime localNow)
        {
            using var ctx = quiltContextFactory.Create();

            var unitOfWork = new UnitOfWork("TEMP");

            var dbLedgerTransaction = ctx.LedgerTransactions.Where(r => r.Description == TestLedgerAccountTransactionDescription).SingleOrDefault();
            if (dbLedgerTransaction == null)
            {
                _ = ctx.CreateLedgerAccountTransactionBuilder()
                    .Begin(TestLedgerAccountTransactionDescription, localNow, utcNow)
                    .UnitOfWork(unitOfWork)
                    .Debit(LedgerAccountNumbers.Cash, 50000.00m)
                    .Credit(LedgerAccountNumbers.OwnersEquity, 50000.00m)
                    .Create();

                _ = ctx.SaveChanges();
            }
        }

        private static void CreateTestPricingScheduleEntry(QuiltContext ctx, PricingSchedule dbPricingSchedule, string unitOfMeasure, decimal price)
        {
            var dbPricingScheduleEntry = dbPricingSchedule.PricingScheduleEntries.Where(r => r.UnitOfMeasureCode == unitOfMeasure).SingleOrDefault();
            if (dbPricingScheduleEntry == null)
            {
                dbPricingScheduleEntry = new PricingScheduleEntry()
                {
                    PricingSchedule = dbPricingSchedule,
                    UnitOfMeasureCode = unitOfMeasure,
                    Price = price
                };
                _ = ctx.PricingScheduleEntries.Add(dbPricingScheduleEntry);
            }
            else
            {
                dbPricingScheduleEntry.Price = price;
            }
        }

        private static void CreateTestPricingSchedules(IQuiltContextFactory quiltContextFactory)
        {
            using var ctx = quiltContextFactory.Create();

            var dbPricingSchedule = ctx.PricingSchedules.Where(r => r.Name == TestPricingScheduleName).SingleOrDefault();
            if (dbPricingSchedule == null)
            {
                dbPricingSchedule = new PricingSchedule()
                {
                    Name = TestPricingScheduleName
                };
                _ = ctx.PricingSchedules.Add(dbPricingSchedule);
            }

            CreateTestPricingScheduleEntry(ctx, dbPricingSchedule, UnitOfMeasureCodes.FatQuarter, 2.5m);

            CreateTestPricingScheduleEntry(ctx, dbPricingSchedule, UnitOfMeasureCodes.HalfYardage, 5.0m);

            CreateTestPricingScheduleEntry(ctx, dbPricingSchedule, UnitOfMeasureCodes.Yardage, 10.0m);

            CreateTestPricingScheduleEntry(ctx, dbPricingSchedule, UnitOfMeasureCodes.TwoYards, 20.0m);

            CreateTestPricingScheduleEntry(ctx, dbPricingSchedule, UnitOfMeasureCodes.ThreeYards, 30.0m);

            _ = ctx.SaveChanges();
        }

        private static void CreateTestStockTransactions(IQuiltContextFactory quiltContextFactory, DateTime utcNow, DateTime localNow)
        {
            using var ctx = quiltContextFactory.Create();

            var builder = ctx.GetInventoryItemStockTransactionBuilder(utcNow, localNow).Begin();

            foreach (var dbInventoryItem in ctx.InventoryItems.ToList())
            {
                if (dbInventoryItem.Quantity == 0)
                {
                    _ = builder.AddInventoryItemStock(dbInventoryItem.InventoryItemId, UnitOfMeasureCodes.FatQuarter, TestInventoryUnitCost, 4 * 50);
                }
            }

            _ = builder.Create();

            _ = ctx.SaveChanges();
        }

        private static Color GetAverageColor(Bitmap bmp)
        {
            //Used for tally
            var r = 0;
            var g = 0;
            var b = 0;

            var total = 0;

            for (var x = 0; x < bmp.Width; x++)
            {
                for (var y = 0; y < bmp.Height; y++)
                {
                    var clr = bmp.GetPixel(x, y);

                    r += clr.R;
                    g += clr.G;
                    b += clr.B;

                    total++;
                }
            }

            //Calculate average
            r /= total;
            g /= total;
            b /= total;

            return Color.FromArgb(r, g, b);
        }
    }
}