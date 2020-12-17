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

namespace RichTodd.QuiltSystem.WebAdmin.Models.ReturnRequest
{
    public class ReturnRequestModelFactory : ApplicationModelFactory
    {
        public ReturnRequest CreateReturnRequest(AReturn_ReturnRequest aReturnRequest)
        {
            return new ReturnRequest(aReturnRequest, Locale);
        }

        public ReturnRequestList CreateReturnRequestList(IReadOnlyList<AReturn_ReturnRequestSummary> aSummaries, PagingState pagingState)
        {
            var summaries = aSummaries.Select(r => CreateReturnRequestListItem(r)).ToList();

            var sortFunction = GetSortFunction(pagingState.Sort);
            var sortedSummaries = sortFunction != null
                ? pagingState.Descending
                    ? summaries.OrderByDescending(sortFunction).ToList()
                    : summaries.OrderBy(sortFunction).ToList()
                : summaries;

            var pageSize = PagingState.PageSize;
            var pageNumber = WebMath.GetPageNumber(pagingState.Page, sortedSummaries.Count, pageSize);
            var pagedSummaries = sortedSummaries.ToPagedList(pageNumber, pageSize);

            var (returnRequestStatus, recordCount) = ParsePagingStateFilter(pagingState.Filter);

            var model = new ReturnRequestList()
            {
                Items = pagedSummaries,
                Filter = new ReturnRequestListFilter()
                {
                    ReturnRequestStatus = returnRequestStatus,
                    RecordCount = recordCount,

                    ReturnRequestStatusList = new List<SelectListItem>
                    {
                        new SelectListItem() { Text = "All", Value = MFulfillment_ReturnRequestStatus.MetaAll.ToString() },
                        new SelectListItem() { Text = "Active", Value = MFulfillment_ReturnRequestStatus.MetaActive.ToString() },
                        new SelectListItem() { Text = "Open", Value = MFulfillment_ReturnRequestStatus.Open.ToString() },
                        new SelectListItem() { Text = "Posted", Value = MFulfillment_ReturnRequestStatus.Posted.ToString() },
                        new SelectListItem() { Text = "Complete", Value = MFulfillment_ReturnRequestStatus.Complete.ToString() },
                        new SelectListItem() { Text = "Cancelled", Value = MFulfillment_ReturnRequestStatus.Cancelled.ToString() },
                        new SelectListItem() { Text = "Exception", Value = MFulfillment_ReturnRequestStatus.Exception.ToString() }
                    },
                    RecordCountList = CreateRecordCountList()
                }
            };

            return model;
        }

        public ReturnRequestListItem CreateReturnRequestListItem(AReturn_ReturnRequestSummary aSummary)
        {
            var model = new ReturnRequestListItem()
            {
                AReturnRequestSummary = aSummary,
                Locale = Locale
            };

            return model;
        }

        //#pragma warning disable CA1822 // Mark members as static
        //        public EditReturnRequest CreateEditReturnRequest(long orderId, long returnRequestId)
        //#pragma warning restore CA1822 // Mark members as static
        //        {
        //            var model = new EditReturnRequest()
        //            {
        //                OrderId = orderId,
        //                ReturnRequestId = returnRequestId
        //            };

        //            return model;
        //        }

        public EditReturnRequest CreateEditReturnRequest(
            MFulfillment_Fulfillable mFulfillable,
            IList<MFulfillment_ReturnRequestReason> mReturnRequestReasons,
            MFulfillment_ReturnRequest mReturnRequest)
        {
            var returnRequestItems = new List<EditReturnRequest.ReturnRequestItem>();
            foreach (var mFulfillableItem in mFulfillable.FulfillableItems)
            {
                // Find corresponding return request item if a return request exists.
                //
                var mReturnRequestItem = mReturnRequest?.ReturnRequestItems.Where(r => r.FulfillableItemId == mFulfillableItem.FulfillableItemId).First();

                var currentQuantity = mReturnRequestItem != null
                    ? mReturnRequestItem.Quantity
                    : 0;

                var maxQuantity = mFulfillableItem.CompleteQuantity - mFulfillableItem.ReturnQuantity;

                var returnRequestItem = new EditReturnRequest.ReturnRequestItem()
                {
                    ReturnRequestItemId = mReturnRequestItem?.ReturnRequestItemId,
                    FulfillableItemId = mFulfillableItem.FulfillableItemId,
                    FulfillableItemReference = mFulfillableItem.FulfillableItemReference,
                    Description = mFulfillableItem.Description,
                    Quantity = currentQuantity,
                    MaxQuantity = maxQuantity,
                    Quantities = GetQuantitySelectList(maxQuantity)
                };

                returnRequestItems.Add(returnRequestItem);
            }

            var currentReturnRequestType = mReturnRequest?.ReturnRequestType.ToString();
            var currentReturnRequestReason = mReturnRequest?.ReturnRequestReasonCode;

            var model = new EditReturnRequest()
            {
                ReturnRequestId = mReturnRequest?.ReturnRequestId,
                ReturnRequestNumber = mReturnRequest?.ReturnRequestNumber,
                FulfillableId = mFulfillable.FulfillableId,
                FulfillableName = mFulfillable.Name,

                ReturnRequestType = currentReturnRequestType,
                ReturnRequestTypes = GetReturnRequestTypeSelectList(mReturnRequest == null),

                ReturnRequestReason = currentReturnRequestReason,
                ReturnRequestReasons = GetReturnRequestReasonSelectList(mReturnRequestReasons, mReturnRequest == null),

                ReturnRequestItems = returnRequestItems
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

        public IList<SelectListItem> GetReturnRequestTypeSelectList(bool includeSelectionRequired)
        {
            var result = new List<SelectListItem>
            {
                new SelectListItem() { Text = "Return", Value = MFulfillment_ReturnRequestTypes.Return.ToString() },
                new SelectListItem() { Text = "Replace", Value = MFulfillment_ReturnRequestTypes.Replace.ToString() }
            };

            if (includeSelectionRequired)
            {
                result.Insert(0, new SelectListItem() { Text = "(Selection Required)", Value = null });
            }

            return result;
        }

        public IList<SelectListItem> GetReturnRequestReasonSelectList(IList<MFulfillment_ReturnRequestReason> mReturnRequestReasons, bool includeSelectionRequired)
        {
            var result = mReturnRequestReasons.Select(r => new SelectListItem() { Text = r.Name, Value = r.ReturnRequestReasonTypeCode }).ToList();

            if (includeSelectionRequired)
            {
                result.Insert(0, new SelectListItem() { Text = "(Selection Required)", Value = null });
            }

            return result;
        }

        public string GetDefaultSort()
        {
            return ListItemMetadata.GetDisplayName(m => m.ReturnRequestId);
        }

        public string CreatePagingStateFilter(MFulfillment_ReturnRequestStatus returnRequestStatus, int recordCount)
        {
            return $"{returnRequestStatus}|{recordCount}";
        }

        public string CreatePagingStateFilter(ReturnRequestListFilter returnRequestListFilter)
        {
            return $"{returnRequestListFilter.ReturnRequestStatus}|{returnRequestListFilter.RecordCount}";
        }

        public (MFulfillment_ReturnRequestStatus returnRequestStatus, int recordCount) ParsePagingStateFilter(string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                return (MFulfillment_ReturnRequestStatus.MetaAll, DefaultRecordCount);
            }

            var fields = filter.Split('|');

            var returnRequestStatus = fields.Length >= 1 && Enum.TryParse(fields[0], out MFulfillment_ReturnRequestStatus returnRequestStatusField)
                ? returnRequestStatusField
                : MFulfillment_ReturnRequestStatus.MetaAll;

            var recordCount = fields.Length >= 2 && int.TryParse(fields[1], out var recordCountField)
                ? recordCountField
                : DefaultRecordCount;

            return (returnRequestStatus, recordCount);
        }

        private ModelMetadata<ReturnRequestListItem> m_listItemMetadata;
        private ModelMetadata<ReturnRequestListItem> ListItemMetadata
        {
            get
            {
                if (m_listItemMetadata == null)
                {
                    m_listItemMetadata = ModelMetadata<ReturnRequestListItem>.Create(HttpContext);
                }

                return m_listItemMetadata;
            }
        }

        private static IDictionary<string, Func<ReturnRequestListItem, object>> s_sortFunctions;
        private IDictionary<string, Func<ReturnRequestListItem, object>> SortFunctions
        {
            get
            {
                if (s_sortFunctions == null)
                {
                    var heading = new ReturnRequestListItem();

                    var sortFunctions = new Dictionary<string, Func<ReturnRequestListItem, object>>
                    {
                        { ListItemMetadata.GetDisplayName(m => m.ReturnRequestId), r => r.ReturnRequestId },
                        { ListItemMetadata.GetDisplayName(m => m.ReturnRequestNumber), r => r.ReturnRequestNumber },
                        { ListItemMetadata.GetDisplayName(m => m.FulfillableId), r => r.FulfillableId },
                        { ListItemMetadata.GetDisplayName(m => m.FulfillableName), r => r.FulfillableName},
                        { ListItemMetadata.GetDisplayName(m => m.FulfillableReference), r => r.FulfillableReference },
                        { ListItemMetadata.GetDisplayName(m => m.ReturnRequestDateTime), r => r.ReturnRequestDateTime },
                        { ListItemMetadata.GetDisplayName(m => m.ReturnRequestStatus), r => r.ReturnRequestStatus },
                        { ListItemMetadata.GetDisplayName(m => m.ReturnRequestStatusDateTime), r => r.ReturnRequestStatusDateTime }
                    };

                    s_sortFunctions = sortFunctions;
                }

                return s_sortFunctions;
            }
        }

        private Func<ReturnRequestListItem, object> GetSortFunction(string sort)
        {
            return !string.IsNullOrEmpty(sort) ? SortFunctions[sort] : null;
        }
    }
}