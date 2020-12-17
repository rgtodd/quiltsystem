//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RichTodd.QuiltSystem.Web.Models.Return
{
    public class ReturnRequestViewModel
    {
        public long? OrderReturnRequestId { get; set; }

        [Display(Name = "Return Authorization Number")]
        public string ReturnRequestNumber { get; set; }

        [Display(Name = "Return Type")]
        public int ReturnTypeCode { get; set; }

        [Display(Name = "Return Type")]
        public string ReturnTypeName { get; set; }

        [Display(Name = "Reason")]
        public string ReasonTypeCode { get; set; }

        [Display(Name = "Reason")]
        public string ReasonTypeName { get; set; }

        [Display(Name = "Notes")]
        public string Notes { get; set; }

        public long OrderId { get; set; }

        public string OrderNumber { get; set; }

        public IList<ReturnRequestViewItemModel> Items { get; set; }
    }
}