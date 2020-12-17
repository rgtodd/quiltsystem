//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.Rendering;

using RichTodd.QuiltSystem.Web;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Return
{
    public class EditReturn
    {
        [Display(Name = "Return ID")]
        public long? ReturnId { get; set; }

        [Display(Name = "Return Number")]
        public string ReturnNumber { get; set; }

        [Display(Name = "Return Request ID")]
        public long ReturnRequestId { get; set; }

        [Display(Name = "Return Request Number")]
        public string ReturnRequestNumber { get; set; }

        [Display(Name = "Fulfillable ID")]
        public long FulfillableId { get; set; }

        [Display(Name = "Fulfillable Name")]
        public string FulfillableName { get; set; }

        [Display(Name = "Return Request Type")]
        public string ReturnRequestType { get; set; }

        [Display(Name = "Return Request Reason")]
        public string ReturnRequestReason { get; set; }

        [Required]
        [Display(Name = "Return Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime? ReturnDate { get; set; }

        [Display(Name = "Items")]
        public IList<ReturnItem> ReturnItems { get; set; }

        public class ReturnItem
        {
            [Display(Name = "Return Item ID")]
            public long? ReturnItemId { get; set; }

            [Display(Name = "Return Request Item ID")]
            public long ReturnRequestItemId { get; set; }

            [Display(Name = "Fulfillable Item ID")]
            public long FulfillableItemId { get; set; }

            [Display(Name = "Fulfillable Item Reference")]
            public string FulfillableItemReference { get; set; }

            [Display(Name = "Description")]
            public string Description { get; set; }

            [Display(Name = "Quantity")]
            public int Quantity { get; set; }

            [Display(Name = "Maximum Quantity")]
            public int MaxQuantity { get; set; }

            [Display(Name = "Quantities")]
            public IList<SelectListItem> Quantities { get; set; }
        }
    }
}