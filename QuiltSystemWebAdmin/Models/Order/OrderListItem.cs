//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.Rendering;

using RichTodd.QuiltSystem.Service.Admin.Abstractions.Data;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Web;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Order
{
    public class OrderListItem
    {
        public AOrder_OrderSummary AOrderSummary { get; }
        public IApplicationLocale Locale { get; }

        public OrderListItem(
            AOrder_OrderSummary aOrderSummary,
            IApplicationLocale locale)
        {
            AOrderSummary = aOrderSummary;
            Locale = locale;
        }

        public long OrderId => AOrderSummary.OrderId;

        public string OrderNumber => AOrderSummary.OrderNumber;

        public string UserId => AOrderSummary.UserId;

        public string UserName => AOrderSummary.UserName;

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        public decimal Total => AOrderSummary.Total;

        public string OrderStatusType => AOrderSummary.OrderStatusType;

        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime StatusDateTime => Locale.GetLocalTimeFromUtc(AOrderSummary.StatusDateTimeUtc);

        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime OrderDateTime => Locale.GetLocalTimeFromUtc(AOrderSummary.OrderDateTimeUtc);

        [Display(Name = "Processing Status")]
        public string ProcessStatus { get; set; }

        public IList<SelectListItem> ProcessStatusList { get; set; }
    }
}