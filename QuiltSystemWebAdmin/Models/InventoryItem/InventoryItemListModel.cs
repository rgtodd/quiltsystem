//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.Rendering;

using PagedList;

namespace RichTodd.QuiltSystem.WebAdmin.Models.InventoryItem
{
    public class InventoryItemListModel
    {
        public const string FILTER_ALL = "ALL";
        public const string FILTER_LOW = "LOW";
        public const string FILTER_OUT = "OUT";

        [Display(Name = "Inventory Items")]
        public IPagedList<InventoryItemModel> InventoryItems { get; set; }

        [Display(Name = "Search")]
        public string Search { get; set; }

        [Display(Name = "Filter")]
        public string Filter { get; set; }

        public IReadOnlyList<SelectListItem> Filters { get; set; }

    }
}