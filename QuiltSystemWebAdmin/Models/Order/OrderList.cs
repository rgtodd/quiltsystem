//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.Rendering;

using PagedList;

using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;
using RichTodd.QuiltSystem.Web;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Order
{
    public class OrderList
    {
        public OrderListFilter Filter { get; set; }
        public IPagedList<OrderListItem> Items { get; set; }
    }

    public class OrderListFilter
    {
        [MaxLength(15)]
        [Display(Name = "Order Number")]
        public string OrderNumber { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = Standard.IsoDateFormat, ApplyFormatInEditMode = true)]
        [Display(Name = "Order Date")]
        public DateTime? OrderDate { get; set; }

        [MaxLength(256)]
        [Display(Name = "User")]
        public string UserName { get; set; }

        [Display(Name = "Order Status")]
        public MOrder_OrderStatus OrderStatus { get; set; }

        [Display(Name = "Maximum Results")]
        public int RecordCount { get; set; }

        public IList<SelectListItem> OrderStatusList { get; set; }
        public IList<SelectListItem> RecordCountList { get; set; }
    }
}