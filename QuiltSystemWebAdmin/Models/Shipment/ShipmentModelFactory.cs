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

namespace RichTodd.QuiltSystem.WebAdmin.Models.Shipment
{
    public class ShipmentModelFactory : ApplicationModelFactory
    {
        public Shipment CreateShipment(AShipment_Shipment aShipment)
        {
            return new Shipment(aShipment, Locale);
        }

        public ShipmentList CreateShipmentList(IReadOnlyList<AShipment_ShipmentSummary> aSummaries, PagingState pagingState)
        {
            var summaries = aSummaries.Select(r => CreateShipmentListItem(r)).ToList();

            var sortFunction = GetSortFunction(pagingState.Sort);
            var sortedShipments = sortFunction != null
                ? pagingState.Descending
                    ? summaries.OrderByDescending(sortFunction).ToList()
                    : summaries.OrderBy(sortFunction).ToList()
                : summaries;

            int pageSize = PagingState.PageSize;
            int pageNumber = WebMath.GetPageNumber(pagingState.Page, sortedShipments.Count, pageSize);
            var pagedShipments = sortedShipments.ToPagedList(pageNumber, pageSize);

            var (shipmentStatus, recordCount) = ParsePagingStateFilter(pagingState.Filter);

            var model = new ShipmentList()
            {
                Items = pagedShipments,
                Filter = new ShipmentListFilter()
                {
                    ShipmentStatus = shipmentStatus,
                    RecordCount = recordCount,

                    ShipmentStatusList = new List<SelectListItem>
                    {
                        new SelectListItem() { Text = "All", Value = MFulfillment_ShipmentStatus.MetaAll.ToString() },
                        new SelectListItem() { Text = "Active", Value = MFulfillment_ShipmentStatus.MetaActive.ToString() },
                        new SelectListItem() { Text = "Open", Value = MFulfillment_ShipmentStatus.Open.ToString() },
                        new SelectListItem() { Text = "Posted", Value = MFulfillment_ShipmentStatus.Posted.ToString() },
                        new SelectListItem() { Text = "Complete", Value = MFulfillment_ShipmentStatus.Complete.ToString() },
                        new SelectListItem() { Text = "Cancelled", Value = MFulfillment_ShipmentStatus.Cancelled.ToString() },
                        new SelectListItem() { Text = "Exception", Value = MFulfillment_ShipmentStatus.Exception.ToString() }
                    },
                    RecordCountList = CreateRecordCountList()
                }
            };

            return model;
        }

        public EditShipment CreateEditShipment(AShipment_Shipment aShipment, IList<AShipment_ShippingVendor> aShippingVendors)
        {
            var model = new EditShipment()
            {
                ShipmentId = aShipment.MShipment.ShipmentId,
                ShipmentNumber = aShipment.MShipment.ShipmentNumber,
                ShipmentStatus = aShipment.MShipment.ShipmentStatus.ToString(),
                TrackingNumber = aShipment.MShipment.TrackingCode,
                ShipmentDate = Locale.GetLocalTimeFromUtc(aShipment.MShipment.ShipmentDateTimeUtc),
                ShippingVendorId = aShipment.MShipment.ShippingVendorId,
                ShippingVendors = GetShippingVendorSelectList(aShippingVendors)
            };

            var shipmentItems = new List<EditShipment.ShipmentItem>();
            foreach (var mShipmentItem in aShipment.MShipment.ShipmentItems)
            {
                var shipmentItem = new EditShipment.ShipmentItem()
                {
                    ShipmentItemId = mShipmentItem.ShipmentItemId,
                    FulfillableItemId = mShipmentItem.FulfillableItemId,
                    FulfillableItemReference = mShipmentItem.FulfillableItemReference,
                    Quantity = mShipmentItem.Quantity
                };
                shipmentItems.Add(shipmentItem);
            }
            model.ShipmentItems = shipmentItems;

            return model;
        }

        public EditShipment CreateEditShipment(AShipment_ShipmentRequest aShipmentRequest, IList<AShipment_ShippingVendor> aShippingVendors)
        {
            var model = new EditShipment()
            {
                ShipmentId = null,
                ShipmentNumber = null,
                ShipmentStatus = "Open",
                TrackingNumber = null,
                ShipmentDate = Locale.GetLocalNow().Date,
                ShippingVendorId = null,
                ShippingVendors = GetShippingVendorSelectList(aShippingVendors)
            };

            var shipmentItems = new List<EditShipment.ShipmentItem>();
            foreach (var mShipmentRequestItem in aShipmentRequest.MShipmentRequest.ShipmentRequestItems)
            {
                var shipmentItem = new EditShipment.ShipmentItem()
                {
                    ShipmentItemId = null,
                    ShipmentRequestItemId = mShipmentRequestItem.ShipmentRequestItemId,
                    FulfillableItemId = mShipmentRequestItem.FulfillableItemId,
                    FulfillableItemReference = mShipmentRequestItem.FulfillableItemReference,
                    Quantity = mShipmentRequestItem.Quantity
                };
                shipmentItems.Add(shipmentItem);
            }
            model.ShipmentItems = shipmentItems;

            return model;
        }

        public SelectList GetShippingVendorSelectList(IList<AShipment_ShippingVendor> aShippingVendors)
        {
            return new SelectList(aShippingVendors, "ShippingVendorId", "Name");
        }

        public string GetDefaultSort()
        {
            return ListItemMetadata.GetDisplayName(m => m.ShipmentId);
        }

        public string CreatePagingStateFilter(MFulfillment_ShipmentStatus shipmentStatus, int recordCount)
        {
            return $"{shipmentStatus}|{recordCount}";
        }

        public string CreatePagingStateFilter(ShipmentListFilter shipmentListFilter)
        {
            return $"{shipmentListFilter.ShipmentStatus}|{shipmentListFilter.RecordCount}";
        }

        public (MFulfillment_ShipmentStatus shipmentStatus, int recordCount) ParsePagingStateFilter(string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                return (MFulfillment_ShipmentStatus.MetaAll, DefaultRecordCount);
            }

            var fields = filter.Split('|');

            var shipmentStatus = fields.Length >= 1 && Enum.TryParse(fields[0], out MFulfillment_ShipmentStatus shipmentStatusField)
                ? shipmentStatusField
                : MFulfillment_ShipmentStatus.MetaAll;

            var recordCount = fields.Length >= 2 && int.TryParse(fields[1], out var recordCountField)
                ? recordCountField
                : DefaultRecordCount;

            return (shipmentStatus, recordCount);
        }

        private ShipmentListItem CreateShipmentListItem(AShipment_ShipmentSummary aSummary)
        {
            var model = new ShipmentListItem(aSummary, Locale);

            return model;
        }

        private ModelMetadata<ShipmentListItem> m_listItemMetadata;
        private ModelMetadata<ShipmentListItem> ListItemMetadata
        {
            get
            {
                if (m_listItemMetadata == null)
                {
                    m_listItemMetadata = ModelMetadata<ShipmentListItem>.Create(HttpContext);
                }

                return m_listItemMetadata;
            }
        }

        private static IDictionary<string, Func<ShipmentListItem, object>> s_sortFunctions;
        private IDictionary<string, Func<ShipmentListItem, object>> SortFunctions
        {
            get
            {
                if (s_sortFunctions == null)
                {
                    var sortFunctions = new Dictionary<string, Func<ShipmentListItem, object>>
                    {
                        { ListItemMetadata.GetDisplayName(m => m.ShipmentId), r => r.ShipmentId },
                        { ListItemMetadata.GetDisplayName(m => m.ShipmentNumber), r => r.ShipmentNumber },
                        { ListItemMetadata.GetDisplayName(m => m.FulfillableId), r => r.FulfillableId },
                        { ListItemMetadata.GetDisplayName(m => m.FulfillableName), r => r.FulfillableName },
                        { ListItemMetadata.GetDisplayName(m => m.FulfillableReference), r => r.FulfillableReference },
                        { ListItemMetadata.GetDisplayName(m => m.CreateDateTime), r => r.CreateDateTime },
                        { ListItemMetadata.GetDisplayName(m => m.ShipmentStatusName), r => r.ShipmentStatusName },
                        { ListItemMetadata.GetDisplayName(m => m.ShippingVendorId), r => r.ShippingVendorId },
                        { ListItemMetadata.GetDisplayName(m => m.StatusDateTime), r => r.StatusDateTime },
                        { ListItemMetadata.GetDisplayName(m => m.TrackingCode), r => r.TrackingCode }
                    };

                    s_sortFunctions = sortFunctions;
                }

                return s_sortFunctions;
            }
        }

        private Func<ShipmentListItem, object> GetSortFunction(string sort)
        {
            return !string.IsNullOrEmpty(sort) ? SortFunctions[sort] : null;
        }
    }
}