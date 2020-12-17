//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RichTodd.QuiltSystem.Web.Models.Order
{
    public class OrderDetailTransactionModel
    {
        public string TransactionType { get; set; }
        public DateTime TransactionDateTime { get; set; }

        [Display(Name = "Items")]
        public IList<OrderDetailItemModel> Items { get; set; }
    }
}