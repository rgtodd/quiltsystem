//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using RichTodd.QuiltSystem.Web.Extensions;

namespace RichTodd.QuiltSystem.Web.Paging
{
    public sealed class CapturePagingStateActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controller = (Controller)filterContext.Controller;
            ViewBagHelper.SetDescendingValue(controller.ViewBag, filterContext.HttpContext.Request.Query[HtmlHelperExtensions.DescendingParameter]);
            ViewBagHelper.SetFilterValue(controller.ViewBag, filterContext.HttpContext.Request.Query[HtmlHelperExtensions.FilterParameter]);
            ViewBagHelper.SetPageValue(controller.ViewBag, filterContext.HttpContext.Request.Query[HtmlHelperExtensions.PageParameter]);
            ViewBagHelper.SetSortValue(controller.ViewBag, filterContext.HttpContext.Request.Query[HtmlHelperExtensions.SortParameter]);
        }
    }
}