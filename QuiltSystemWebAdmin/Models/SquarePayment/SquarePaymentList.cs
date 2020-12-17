//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.Rendering;

using PagedList;

using RichTodd.QuiltSystem.Web;

namespace RichTodd.QuiltSystem.WebAdmin.Models.SquarePayment
{
    public class SquarePaymentList
    {
        public SquarePaymentListFilter Filter { get; set; }
        public IPagedList<SquarePaymentListItem> Items { get; set; }
    }

    public class SquarePaymentListFilter
    {
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = Standard.IsoDateFormat, ApplyFormatInEditMode = true)]
        [Display(Name = "Payment Date")]
        public DateTime? PaymentDate { get; set; }

        [Display(Name = "Maximum Results")]
        public int RecordCount { get; set; }

        public IList<SelectListItem> RecordCountList { get; set; }
    }
}
