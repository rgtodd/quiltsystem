//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.ComponentModel.DataAnnotations;

using PagedList;

namespace RichTodd.QuiltSystem.Web.Models.Order
{
    public class OrderDetailListModel
    {
        [Display(Name = "Orders")]
        public IPagedList<OrderDetailModel> Orders { get; set; }
    }
}