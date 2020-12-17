//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MInventory_LibraryEntry
    {

        private readonly long m_inventoryItemId;
        private readonly string m_sku;
        private readonly string m_name;
        private readonly string m_inventoryItemTypeCode;
        private readonly long m_pricingScheduleId;
        private readonly string m_manufacturer;
        private readonly string m_collection;
        private readonly int m_quantity;
        private readonly int m_reservedQuantity;
        private readonly int m_hue;
        private readonly int m_saturation;
        private readonly int m_value;
        private readonly IList<MInventory_LibraryItemStockEntry> m_stocks;

        public MInventory_LibraryEntry(long inventoryItemId, string sku, string name, string inventoryItemTypeCode, long pricingScheduleId, string manufacturer, string collection, int quantity, int reservedQuantity, int hue, int saturation, int value, IList<MInventory_LibraryItemStockEntry> stocks)
        {
            if (string.IsNullOrEmpty(sku)) throw new ArgumentNullException(nameof(sku));
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrEmpty(inventoryItemTypeCode)) throw new ArgumentNullException(nameof(inventoryItemTypeCode));

            m_inventoryItemId = inventoryItemId;
            m_sku = sku;
            m_name = name;
            m_inventoryItemTypeCode = inventoryItemTypeCode;
            m_pricingScheduleId = pricingScheduleId;
            m_manufacturer = manufacturer;
            m_collection = collection;
            m_quantity = quantity;
            m_reservedQuantity = reservedQuantity;
            m_hue = hue;
            m_saturation = saturation;
            m_value = value;
            m_stocks = stocks ?? throw new ArgumentNullException(nameof(stocks));
        }

        public string Collection
        {
            get { return m_collection; }
        }

        public int Hue
        {
            get { return m_hue; }
        }

        public long InventoryItemId
        {
            get { return m_inventoryItemId; }
        }

        public string InventoryItemTypeCode
        {
            get { return m_inventoryItemTypeCode; }
        }

        public long PricingScheduleId
        {
            get { return m_pricingScheduleId; }
        }

        public string Manufacturer
        {
            get { return m_manufacturer; }
        }

        public string Name
        {
            get { return m_name; }
        }

        public int Quantity
        {
            get { return m_quantity; }
        }

        public int ReservedQuantity
        {
            get { return m_reservedQuantity; }
        }

        public int Saturation
        {
            get { return m_saturation; }
        }

        public string Sku
        {
            get { return m_sku; }
        }

        public IList<MInventory_LibraryItemStockEntry> Stocks
        {
            get { return m_stocks; }
        }

        public int Value
        {
            get { return m_value; }
        }

    }
}