//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.Rendering;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Order
{
    public class EditTransaction
    {
        [Display(Name = "Order ID")]
        public long OrderId { get; set; }

        [Display(Name = "Order Transaction Type ID")]
        public int OrderTransactionTypeCode { get; set; }

        [Display(Name = "Transaction Types")]
        public SelectList TransactionTypes { get; set; }

        [Display(Name = "Order Item Status Type ID")]
        public int OrderItemStatusTypeCode { get; set; }

        [Display(Name = "Entries")]
        public IList<TransactionEntry> TransactionEntries { get; set; }

        public class TransactionEntry
        {
            [Display(Name = "Order Item ID")]
            public long OrderItemId { get; set; }

            [Display(Name = "Quantity")]
            public int Quantity { get; set; }

            [Display(Name = "Selected")]
            public bool Selected { get; set; }
        }
    }
}