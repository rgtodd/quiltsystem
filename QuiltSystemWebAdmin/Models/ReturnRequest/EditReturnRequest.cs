//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.Rendering;

namespace RichTodd.QuiltSystem.WebAdmin.Models.ReturnRequest
{
    public class EditReturnRequest
    {
        [Display(Name = "Return Request ID")]
        public long? ReturnRequestId { get; set; }

        [Display(Name = "Return Request Number")]
        public string ReturnRequestNumber { get; set; }

        [Display(Name = "Fulfillable ID")]
        public long FulfillableId { get; set; }

        [Display(Name = "Fulfillable Name")]
        public string FulfillableName { get; set; }

        [Required]
        [Display(Name = "Return Request Type")]
        public string ReturnRequestType { get; set; }

        [Display(Name = "Return Request Types")]
        public IList<SelectListItem> ReturnRequestTypes { get; set; }

        [Required]
        [Display(Name = "Return Request Reason")]
        public string ReturnRequestReason { get; set; }

        [Display(Name = "Return Request Reasons")]
        public IList<SelectListItem> ReturnRequestReasons { get; set; }

        [Display(Name = "Items")]
        public IList<ReturnRequestItem> ReturnRequestItems { get; set; }

        public class ReturnRequestItem
        {
            [Display(Name = "Return Request Item ID")]
            public long? ReturnRequestItemId { get; set; }

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