//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using RichTodd.QuiltSystem.Database.Domain;
using RichTodd.QuiltSystem.Database.Model;
using RichTodd.QuiltSystem.Database.Model.Extensions;

namespace RichTodd.QuiltSystem.Database.Builders
{
    public class InventoryItemStockTransactionBuilder
    {
        private readonly QuiltContext m_ctx;
        private readonly DateTime m_localNow;
        private readonly DateTime m_utcNow;
        private InventoryItemStockTransaction m_inventoryItemStockTransaction;
        private IList<InventoryItemStockTransactionItem> m_inventoryItemStockTransactionItems;
        //private Order m_order;

        public InventoryItemStockTransactionBuilder(QuiltContext ctx, DateTime utcNow, DateTime localNow)
        {
            m_ctx = ctx;
            //m_order = null;
            m_utcNow = utcNow;
            m_localNow = localNow;
        }

        public InventoryItemStockTransactionBuilder AddInventoryItemStock(long inventoryItemId, string unitOfMeasureCode, decimal unitCost, int quantity)
        {
            if (m_inventoryItemStockTransaction == null) throw new InvalidOperationException("Begin has not been called.");

            //if (m_order != null) throw new InvalidOperationException("Cannot add stock when processing order.");

            var dbInventoryItemStock = new InventoryItemStock()
            {
                InventoryItemId = inventoryItemId,
                UnitOfMeasureCode = unitOfMeasureCode,
                UnitCost = unitCost,
                StockDateTimeUtc = m_utcNow,
                OriginalQuantity = quantity,
                CurrentQuantity = quantity,
            };
            _ = m_ctx.InventoryItemStocks.Add(dbInventoryItemStock);

            var dbInventoryItemStockTransactionItem = new InventoryItemStockTransactionItem()
            {
                InventoryItemStock = dbInventoryItemStock,
                InventoryItemStockTransaction = m_inventoryItemStockTransaction,
                Quantity = quantity,
                Cost = unitCost * quantity,
            };
            _ = m_ctx.InventoryItemStockTransactionItems.Add(dbInventoryItemStockTransactionItem);

            dbInventoryItemStockTransactionItem.InventoryItemStock.InventoryItem.Quantity += quantity;

            return this;
        }

        public InventoryItemStockTransactionBuilder Begin()
        {
            if (m_inventoryItemStockTransaction != null) throw new InvalidOperationException("Begin has already been called.");

            m_inventoryItemStockTransaction = new InventoryItemStockTransaction()
            {
                TransactionDateTimeUtc = m_utcNow,
                TotalCost = 0, // Updated by Create.
            };
            _ = m_ctx.InventoryItemStockTransactions.Add(m_inventoryItemStockTransaction);

            //m_order = null;
            m_inventoryItemStockTransactionItems = null;

            return this;
        }

        //public InventoryItemStockTransactionBuilder Begin(long orderId)
        //{
        //    if (m_inventoryItemStockTransaction != null) throw new InvalidOperationException("Begin has already been called.");

        //    m_inventoryItemStockTransaction = new InventoryItemStockTransaction()
        //    {
        //        TransactionDateTimeUtc = m_utcNow,
        //        TotalCost = 0 // Updated by Create.
        //    };
        //    _ = m_ctx.InventoryItemStockTransactions.Add(m_inventoryItemStockTransaction);

        //    m_order = m_ctx.Orders.Where(r => r.OrderId == orderId).Single();
        //    m_inventoryItemStockTransactionItems = new List<InventoryItemStockTransactionItem>();

        //    return this;
        //}

        public InventoryItemStockTransactionBuilder ConsumeInventoryItemStock(long inventoryItemId, string unitOfMeasureCode, int quantity)
        {
            if (m_inventoryItemStockTransaction == null) throw new InvalidOperationException("Begin has not been called.");

            //if (m_order == null) throw new InvalidOperationException("Cannot consume stock wihtout order.");

            while (quantity > 0)
            {
                var dbInventoryItemStockTransactionItem = m_inventoryItemStockTransactionItems.Where(r => r.InventoryItemStock.InventoryItemId == inventoryItemId).SingleOrDefault();
                if (dbInventoryItemStockTransactionItem == null)
                {
                    var dbInventoryItemStock = m_ctx.InventoryItemStocks.Where(r => r.InventoryItemId == inventoryItemId && r.CurrentQuantity > 0).OrderBy(r => r.StockDateTimeUtc).First();

                    dbInventoryItemStockTransactionItem = new InventoryItemStockTransactionItem()
                    {
                        InventoryItemStock = dbInventoryItemStock,
                        InventoryItemStockTransaction = m_inventoryItemStockTransaction,
                        Quantity = 0,
                        Cost = 0m,
                    };
                    _ = m_ctx.InventoryItemStockTransactionItems.Add(dbInventoryItemStockTransactionItem);
                }

                if (dbInventoryItemStockTransactionItem.InventoryItemStock.UnitOfMeasureCode != unitOfMeasureCode)
                {
                    throw new ArgumentException("Unit of measure mismatch.");
                }

                var transactionQuantity = Math.Min(quantity, dbInventoryItemStockTransactionItem.InventoryItemStock.CurrentQuantity);
                Debug.Assert(transactionQuantity > 0);

                dbInventoryItemStockTransactionItem.Quantity -= transactionQuantity;
                dbInventoryItemStockTransactionItem.InventoryItemStock.CurrentQuantity -= transactionQuantity;
                dbInventoryItemStockTransactionItem.InventoryItemStock.InventoryItem.Quantity -= transactionQuantity;

                dbInventoryItemStockTransactionItem.Cost = dbInventoryItemStockTransactionItem.Quantity * dbInventoryItemStockTransactionItem.InventoryItemStock.UnitCost;

                if (dbInventoryItemStockTransactionItem.InventoryItemStock.CurrentQuantity == 0)
                {
                    _ = m_inventoryItemStockTransactionItems.Remove(dbInventoryItemStockTransactionItem);
                }

                quantity -= transactionQuantity;
                Debug.Assert(quantity >= 0);
            }

            return this;
        }

        public InventoryItemStockTransaction Create()
        {
            if (m_inventoryItemStockTransaction == null) throw new InvalidOperationException("Begin has not been called.");

            var unitOfWork = new UnitOfWork("TEMP");

            m_inventoryItemStockTransaction.TotalCost = m_inventoryItemStockTransaction.InventoryItemStockTransactionItems.Sum(r => r.Cost);

            //if (m_order == null)
            //{
            var amount = m_inventoryItemStockTransaction.TotalCost;

            var postDateTime = m_localNow;

            var dbLedgerAccountTransaction = m_ctx.CreateLedgerAccountTransactionBuilder()
                .Begin(string.Format("InventoryItemStockTransaction {0:d}", m_localNow), postDateTime, m_utcNow)
                .UnitOfWork(unitOfWork)
                .Debit(LedgerAccountNumbers.FabricSupplyAsset, amount)
                .Credit(LedgerAccountNumbers.Cash, amount)
                .Create();

            m_inventoryItemStockTransaction.LedgerAccountTransaction = dbLedgerAccountTransaction;
            //}
            //else
            //{
            //    var amount = -m_inventoryItemStockTransaction.TotalCost;

            //    var dbOrderLedgerAccountTransaction = m_ctx.GetOrderLedgerAccountTransactionBuilder(m_order.OrderId, m_utcNow, m_localNow, string.Format("InventoryItemStockTransaction {0:d}", m_localNow))
            //        .Debit(LedgerAccountTypes.FabricSupplyExpense, amount)
            //        .Credit(LedgerAccountTypes.FabricSupplyAsset, amount)
            //        .Create();

            //    m_inventoryItemStockTransaction.OrderLedgerAccountTransaction = dbOrderLedgerAccountTransaction;
            //}

            return m_inventoryItemStockTransaction;
        }
    }
}