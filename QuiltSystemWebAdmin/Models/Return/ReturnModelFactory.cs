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
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;
using RichTodd.QuiltSystem.Web;
using RichTodd.QuiltSystem.Web.Paging;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Return
{
    public class ReturnModelFactory : ApplicationModelFactory
    {
        public Return CreateReturnDetailModel(AReturn_Return aReturn)
        {
            return new Return(aReturn, Locale);
        }

        public ReturnList CreateReturnListModel(IReadOnlyList<AReturn_ReturnSummary> aReturns, PagingState pagingState)
        {
            var summaries = aReturns.Select(r => CreateReturnDetailModel(r)).ToList();

            var sortFunction = GetSortFunction(pagingState.Sort);
            var sortedSummaries = sortFunction != null
                ? pagingState.Descending
                    ? summaries.OrderByDescending(sortFunction).ToList()
                    : summaries.OrderBy(sortFunction).ToList()
                : summaries;

            var pageSize = PagingState.PageSize;
            var pageNumber = WebMath.GetPageNumber(pagingState.Page, sortedSummaries.Count, pageSize);
            var pagedSummaries = sortedSummaries.ToPagedList(pageNumber, pageSize);

            var (returnStatus, recordCount) = ParsePagingStateFilter(pagingState.Filter);

            var model = new ReturnList()
            {
                Items = pagedSummaries,
                Filter = new ReturnListFilter()
                {
                    ReturnStatus = returnStatus,
                    RecordCount = recordCount,

                    ReturnStatusList = new List<SelectListItem>
                    {
                        new SelectListItem() { Text = "All", Value = MFulfillment_ReturnStatus.MetaAll.ToString() },
                        new SelectListItem() { Text = "Active", Value = MFulfillment_ReturnStatus.MetaActive.ToString() },
                        new SelectListItem() { Text = "Open", Value = MFulfillment_ReturnStatus.Open.ToString() },
                        new SelectListItem() { Text = "Posted", Value = MFulfillment_ReturnStatus.Posted.ToString() },
                        new SelectListItem() { Text = "Complete", Value = MFulfillment_ReturnStatus.Complete.ToString() },
                        new SelectListItem() { Text = "Cancelled", Value = MFulfillment_ReturnStatus.Cancelled.ToString() },
                        new SelectListItem() { Text = "Exception", Value = MFulfillment_ReturnStatus.Exception.ToString() }
                    },
                    RecordCountList = CreateRecordCountList()
                }
            };

            return model;
        }

        public ReturnListItem CreateReturnDetailModel(AReturn_ReturnSummary aReturn)
        {
            var model = new ReturnListItem(aReturn, Locale);

            return model;
        }

        public EditReturn CreateEditReturn(
            MFulfillment_Fulfillable mFulfillable,
            MFulfillment_ReturnRequest mReturnRequest,
            MFulfillment_Return mReturn)
        {
            var returnRequestItems = new List<EditReturn.ReturnItem>();
            foreach (var mReturnRequestItem in mReturnRequest.ReturnRequestItems)
            {
                var mFulfillableItem = mFulfillable.FulfillableItems.Where(r => r.FulfillableItemId == mReturnRequestItem.FulfillableItemId).First();

                var mReturnItem = mReturn?.ReturnItems.Where(r => r.ReturnRequestItemId == mReturnRequestItem.ReturnRequestItemId).First();

                var currentQuantity = mReturnItem != null
                    ? mReturnItem.Quantity
                    : 0;

                var maxQuantity = mReturnRequestItem.Quantity;

                var returnRequestItem = new EditReturn.ReturnItem()
                {
                    ReturnItemId = mReturnItem?.ReturnItemId,
                    ReturnRequestItemId = mReturnRequestItem.ReturnRequestItemId,
                    FulfillableItemId = mFulfillableItem.FulfillableItemId,
                    FulfillableItemReference = mFulfillableItem.FulfillableItemReference,
                    Description = mFulfillableItem.Description,
                    Quantity = currentQuantity,
                    MaxQuantity = maxQuantity,
                    Quantities = GetQuantitySelectList(maxQuantity)
                };

                returnRequestItems.Add(returnRequestItem);
            }

            var model = new EditReturn()
            {
                ReturnId = mReturn?.ReturnId,
                ReturnNumber = mReturn?.ReturnNumber,

                ReturnRequestId = mReturnRequest.ReturnRequestId,
                ReturnRequestNumber = mReturnRequest?.ReturnRequestNumber,
                FulfillableId = mFulfillable.FulfillableId,
                FulfillableName = mFulfillable.Name,
                ReturnRequestType = mReturnRequest.ReturnRequestType.ToString(),
                ReturnRequestReason = mReturnRequest.ReturnRequestReasonCode,

                ReturnDate = mReturn?.CreateDateTimeUtc,

                ReturnItems = returnRequestItems
            };

            return model;
        }

        public IList<SelectListItem> GetQuantitySelectList(int maxQuantity)
        {
            var quantities = new List<SelectListItem>();

            for (var quantity = 0; quantity <= maxQuantity; ++quantity)
            {
                quantities.Add(
                    new SelectListItem()
                    {
                        Text = quantity.ToString(),
                        Value = quantity.ToString()
                    });
            }

            return quantities;
        }

        //private EditReturn.Item CreateReturnEditModelItem(MFulfillment_ReturnRequestItem mReturnRequestItem)
        //{
        //    var model = new EditReturn.Item()
        //    {
        //        ReturnItemId = null,
        //        FulfillableId = mReturnRequestItem.FulfillableId,
        //        Quantity = mReturnRequestItem.Quantity
        //    };

        //    return model;
        //}

        public string GetDefaultSort()
        {
            return ListItemMetadata.GetDisplayName(m => m.ReturnId);
        }


        public string CreatePagingStateFilter(MFulfillment_ReturnStatus returnStatus, int recordCount)
        {
            return $"{returnStatus}|{recordCount}";
        }

        public string CreatePagingStateFilter(ReturnListFilter returnListFilter)
        {
            return $"{returnListFilter.ReturnStatus}|{returnListFilter.RecordCount}";
        }

        public (MFulfillment_ReturnStatus returnStatus, int recordCount) ParsePagingStateFilter(string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                return (MFulfillment_ReturnStatus.MetaAll, DefaultRecordCount);
            }

            var fields = filter.Split('|');

            var returnStatus = fields.Length >= 1 && Enum.TryParse(fields[0], out MFulfillment_ReturnStatus returnStatusField)
                ? returnStatusField
                : MFulfillment_ReturnStatus.MetaAll;

            var recordCount = fields.Length >= 2 && int.TryParse(fields[1], out var recordCountField)
                ? recordCountField
                : DefaultRecordCount;

            return (returnStatus, recordCount);
        }

        private ModelMetadata<ReturnListItem> m_listItemMetadata;
        private ModelMetadata<ReturnListItem> ListItemMetadata
        {
            get
            {
                if (m_listItemMetadata == null)
                {
                    m_listItemMetadata = ModelMetadata<ReturnListItem>.Create(HttpContext);
                }

                return m_listItemMetadata;
            }
        }

        private static IDictionary<string, Func<ReturnListItem, object>> s_sortFunctions;
        private IDictionary<string, Func<ReturnListItem, object>> SortFunctions
        {
            get
            {
                if (s_sortFunctions == null)
                {
                    var heading = new ReturnListItem(null, null);

                    var sortFunctions = new Dictionary<string, Func<ReturnListItem, object>>
                    {
                        { ListItemMetadata.GetDisplayName(m => m.ReturnId), r => r.ReturnId },
                        { ListItemMetadata.GetDisplayName(m => m.ReturnNumber), r => r.ReturnNumber },
                        { ListItemMetadata.GetDisplayName(m => m.FulfillableId), r => r.FulfillableId },
                        { ListItemMetadata.GetDisplayName(m => m.FulfillableName), r => r.FulfillableName },
                        { ListItemMetadata.GetDisplayName(m => m.FulfillableReference), r => r.FulfillableReference },
                        { ListItemMetadata.GetDisplayName(m => m.ReturnDateTime), r => r.ReturnDateTime },
                        { ListItemMetadata.GetDisplayName(m => m.ReturnStatusName), r => r.ReturnStatusName },
                        { ListItemMetadata.GetDisplayName(m => m.ShippingVendorId), r => r.ShippingVendorId },
                        { ListItemMetadata.GetDisplayName(m => m.StatusDateTime), r => r.StatusDateTime },
                        { ListItemMetadata.GetDisplayName(m => m.TrackingCode), r => r.TrackingCode }
                    };

                    s_sortFunctions = sortFunctions;
                }

                return s_sortFunctions;
            }
        }

        private Func<ReturnListItem, object> GetSortFunction(string sort)
        {
            return !string.IsNullOrEmpty(sort)
                ? SortFunctions[sort]
                : null;
        }
    }
}