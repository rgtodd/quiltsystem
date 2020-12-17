//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.Rendering;

namespace RichTodd.QuiltSystem.Web.Models.Return
{
    public class ReturnRequestEditModel
    {
        public long? OrderReturnRequestId { get; set; }

        public long OrderId { get; set; }

        public string OrderNumber { get; set; }

        [Display(Name = "Return Type")]
        public string ReturnTypeCode { get; set; }

        [Display(Name = "Return Type")]
        public string ReturnTypeName { get; set; }

        [Display(Name = "Why are you returning these items?")]
        public string ReasonTypeCode { get; set; }

        [Display(Name = "Reason")]
        public string ReasonTypeName { get; set; }

        public IList<SelectListItem> ReasonTypes { get; set; }

        [Display(Name = "Notes")]
        public string Notes { get; set; }

        public IList<ReturnRequestEditItemModel> Items { get; set; }
    }
}