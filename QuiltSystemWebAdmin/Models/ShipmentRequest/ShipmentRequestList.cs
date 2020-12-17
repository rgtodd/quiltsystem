//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.Rendering;

using PagedList;

using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.WebAdmin.Models.ShipmentRequest
{
    public class ShipmentRequestList
    {
        public ShipmentRequestListFilter Filter { get; set; }
        public IPagedList<ShipmentRequestListItem> Items { get; set; }
    }

    public class ShipmentRequestListFilter
    {
        [Display(Name = "Shipment Request Status")]
        public MFulfillment_ShipmentRequestStatus ShipmentRequestStatus { get; set; }

        [Display(Name = "Maximum Results")]
        public int RecordCount { get; set; }

        public IList<SelectListItem> ShipmentRequestStatusList { get; set; }
        public IList<SelectListItem> RecordCountList { get; set; }
    }
}