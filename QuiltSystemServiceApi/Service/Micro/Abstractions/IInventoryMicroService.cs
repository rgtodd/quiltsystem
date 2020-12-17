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
    public interface IInventoryMicroService : IEventService
    {
        Task ConsumeItemsAsync(IList<long> consumableIds);

        IList<MInventory_LibraryEntry> GetEntries();

        MInventory_LibraryEntry GetEntry(string sku);

        MInventory_LibraryEntry GetEntry(long inventoryItemId);

        Task<long> CreateEntryAsync(string sku, string inventoryItemTypeCode, string name, string collection, string manufacturer, int hue, int saturation, int value, IEnumerable<string> unitOfMeasureCodes, string pricingScheduleName, DateTime utcNow);
    }
}
