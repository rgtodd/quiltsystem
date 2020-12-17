//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using PagedList.Mvc;

namespace RichTodd.QuiltSystem.Web.Bootstrap
{
    public static class BootstrapPaging
    {
        public static PagedListRenderOptions Options { get; } = new PagedListRenderOptions()
        {
            Display = PagedListDisplayMode.IfNeeded,
            LiElementClasses = new string[] { "page-item" }
        };
    }
}