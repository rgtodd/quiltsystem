//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc.Rendering;

using PagedList;

using RichTodd.QuiltSystem.Resources.Web;
using RichTodd.QuiltSystem.Service.Admin.Abstractions.Data;
using RichTodd.QuiltSystem.Web;
using RichTodd.QuiltSystem.Web.Paging;

namespace RichTodd.QuiltSystem.WebAdmin.Models.InventoryItem
{
    public class InventoryItemModelFactory : ApplicationModelFactory
    {

        private static IDictionary<string, Func<InventoryItemModel, object>> s_sortFunctions;

        private ModelMetadata<InventoryItemModel> m_inventoryItemModelMetadata;

        private ModelMetadata<InventoryItemModel> InventoryItemModelMetadata
        {
            get
            {
                if (m_inventoryItemModelMetadata == null)
                {
                    m_inventoryItemModelMetadata = ModelMetadata<InventoryItemModel>.Create(HttpContext);
                }

                return m_inventoryItemModelMetadata;
            }
        }

        private IDictionary<string, Func<InventoryItemModel, object>> SortFunctions
        {
            get
            {
                if (s_sortFunctions == null)
                {
                    var heading = new InventoryItemModel();

                    var sortFunctions = new Dictionary<string, Func<InventoryItemModel, object>>
                    {
                        { InventoryItemModelMetadata.GetDisplayName(m => m.Id), r => r.Id },
                        { InventoryItemModelMetadata.GetDisplayName(m => m.Collection), r => r.Collection },
                        { InventoryItemModelMetadata.GetDisplayName(m => m.Manufacturer), r => r.Manufacturer },
                        { InventoryItemModelMetadata.GetDisplayName(m => m.Quantity), r => r.Quantity },
                        { InventoryItemModelMetadata.GetDisplayName(m => m.Sku), r => r.Sku },
                        { InventoryItemModelMetadata.GetDisplayName(m => m.TypeName), r => r.TypeName }
                    };

                    s_sortFunctions = sortFunctions;
                }

                return s_sortFunctions;
            }
        }

        public InventoryItemDetailModel CreateInventoryItemDetailModel(AInventory_InventoryItem svcInventoryItem)
        {
            var model = new InventoryItemDetailModel()
            {
                Id = svcInventoryItem.Id.ToString(),
                TypeName = svcInventoryItem.Type,
                Sku = svcInventoryItem.Sku,
                Name = svcInventoryItem.Name,
                Manufacturer = svcInventoryItem.Manufacturer,
                Collection = svcInventoryItem.Collection,
                Quantity = svcInventoryItem.Quantity,
                WebColor = svcInventoryItem.Color.WebColor,
                ReservedQuantity = svcInventoryItem.ReservedQuantity
            };

            return model;
        }

        public InventoryItemListModel CreateInventoryItemListModel(IReadOnlyList<AInventory_InventoryItem> svcInventoryItems, PagingState pagingState)
        {
            var inventoryItems = new List<InventoryItemModel>();
            foreach (var svcInventoryItem in svcInventoryItems)
            {
                var inventoryItemStocks = new List<InventoryItemStockModel>();
                foreach (var svcInventoryItemStock in svcInventoryItem.InventoryItemStocks)
                {
                    inventoryItemStocks.Add(new InventoryItemStockModel()
                    {
                        Id = svcInventoryItemStock.InventoryItemStockId.ToString(),
                        UnitOfMeasure = svcInventoryItemStock.UnitOfMeasure,
                        UnitCost = svcInventoryItemStock.UnitCost,
                        StockDateTime = Locale.GetLocalTimeFromUtc(svcInventoryItemStock.StockDateTimeUtc),
                        OriginalQuantity = svcInventoryItemStock.OriginalQuantity,
                        CurrentQuantity = svcInventoryItemStock.CurrentQuantity
                    });
                }

                inventoryItems.Add(new InventoryItemModel()
                {
                    Id = svcInventoryItem.Id.ToString(),
                    TypeName = svcInventoryItem.Type,
                    Sku = svcInventoryItem.Sku,
                    Name = svcInventoryItem.Name,
                    Manufacturer = svcInventoryItem.Manufacturer,
                    Collection = svcInventoryItem.Collection,
                    Quantity = svcInventoryItem.Quantity,
                    ReservedQuantity = svcInventoryItem.ReservedQuantity,
                    WebColor = svcInventoryItem.Color.WebColor,
                    Stocks = inventoryItemStocks
                });
            }

            IList<InventoryItemModel> sortedInventoryItems;
            var sortFunction = GetSortFunction(pagingState.Sort);
            sortedInventoryItems = sortFunction != null
                ? pagingState.Descending
                    ? inventoryItems.OrderByDescending(sortFunction).ToList()
                    : inventoryItems.OrderBy(sortFunction).ToList()
                : inventoryItems;

            var pageSize = 10;
            var pageNumber = WebMath.GetPageNumber(pagingState.Page, sortedInventoryItems.Count, pageSize);
            var pagedInventoryItems = sortedInventoryItems.ToPagedList(pageNumber, pageSize);

            var model = new InventoryItemListModel()
            {
                InventoryItems = pagedInventoryItems,
                Filter = pagingState.Filter ?? InventoryItemListModel.FILTER_ALL,
                Filters = new List<SelectListItem>
                {
                    new SelectListItem() { Value = InventoryItemListModel.FILTER_ALL, Text = "All" },
                    new SelectListItem() { Value = InventoryItemListModel.FILTER_LOW, Text = "Low" },
                    new SelectListItem() { Value = InventoryItemListModel.FILTER_OUT, Text = "Out of Stock" }
                }
            };

            return model;
        }

        private Func<InventoryItemModel, object> GetSortFunction(string sort)
        {
            return !string.IsNullOrEmpty(sort) ? SortFunctions[sort] : null;
        }

    }
}