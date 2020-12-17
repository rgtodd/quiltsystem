//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.Rendering;

using PagedList;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Block
{
    public class BlockList
    {
        public BlockListFilter Filter { get; set; }
        public IPagedList<BlockListItem> Items { get; set; }
    }

    public class BlockListFilter
    {
        [Display(Name = "Maximum Results")]
        public int RecordCount { get; set; }

        public IList<SelectListItem> RecordCountList { get; set; }
        public IList<BlockListFilterTagItem> Tags { get; set; }
    }

    public class BlockListFilterTagItem
    {
        public string TagName { get; set; }
        public bool Selected { get; set; }
    }
}
