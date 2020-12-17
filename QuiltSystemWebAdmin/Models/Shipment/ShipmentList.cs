//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.Rendering;

using PagedList;

using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Shipment
{
    public class ShipmentList
    {
        public ShipmentListFilter Filter { get; set; }
        public IPagedList<ShipmentListItem> Items { get; set; }
    }

    public class ShipmentListFilter
    {
        [Display(Name = "Shipment Status")]
        public MFulfillment_ShipmentStatus ShipmentStatus { get; set; }

        [Display(Name = "Maximum Results")]
        public int RecordCount { get; set; }

        public IList<SelectListItem> ShipmentStatusList { get; set; }
        public IList<SelectListItem> RecordCountList { get; set; }
    }
}