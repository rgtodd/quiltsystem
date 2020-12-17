//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RichTodd.QuiltSystem.Web.Models.Prototype
{
    public class PrototypeItemListModel
    {
        [Display(Name = "Items")]
        public IList<PrototypeItemModel> Items { get; set; }
    }
}