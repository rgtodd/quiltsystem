//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MInventory_LibraryItemStockEntry
    {

        private readonly long m_inventoryItemStockId;
        private readonly string m_unitOfMeasure;
        private readonly decimal m_unitCost;
        private readonly DateTime m_stockDateTimeUtc;
        private readonly int m_originalQuantity;
        private readonly int m_currentQuantity;

        public MInventory_LibraryItemStockEntry(long inventoryItemStockId, string unitOfMeasure, decimal unitCost, DateTime stockDateTimeUtc, int originalQuantity, int currentQuantity)
        {
            m_inventoryItemStockId = inventoryItemStockId;
            m_unitOfMeasure = unitOfMeasure ?? throw new ArgumentNullException(nameof(unitOfMeasure));
            m_unitCost = unitCost;
            m_stockDateTimeUtc = stockDateTimeUtc;
            m_originalQuantity = originalQuantity;
            m_currentQuantity = currentQuantity;
        }

        public int CurrentQuantity
        {
            get { return m_currentQuantity; }
        }

        public long InventoryItemStockId
        {
            get { return m_inventoryItemStockId; }
        }

        public int OriginalQuantity
        {
            get { return m_originalQuantity; }
        }

        public DateTime StockDateTimeUtc
        {
            get { return m_stockDateTimeUtc; }
        }

        public decimal UnitCost
        {
            get { return m_unitCost; }
        }

        public string UnitOfMeasure
        {
            get { return m_unitOfMeasure; }
        }

    }
}