//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace RichTodd.QuiltSystem.Web.Feedback
{
    public sealed class FeedbackActionFilterAttribute : ActionFilterAttribute
    {
        public const string Key = "feedback";

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (!(filterContext.Result is RedirectToActionResult redirectResult))
            {
                return;
            }

            var contextId = filterContext.HttpContext.Items[Key];
            if (contextId == null)
            {
                return;
            }

            if (redirectResult.RouteValues == null)
            {
                redirectResult.RouteValues = new RouteValueDictionary();
            }
            redirectResult.RouteValues[Key] = contextId.ToString();

            //if (!filterContext.RouteData.Values.ContainsKey(Key))
            ////if (!redirectResult.RouteValues.ContainsKey(Key))
            //{
            //    var contextId = filterContext.HttpContext.Items[Key];
            //    if (contextId != null)
            //    {
            //        //filterContext.ActionArguments[Key]= contextId.ToString();
            //    }
            //    else
            //    {
            //        var queryStringId = filterContext.HttpContext.Request.Query[Key];
            //        if (queryStringId.Count == 1)
            //        {
            //            filterContext.RouteData.Values.Add(Key, queryStringId[0]);
            //        }
            //    }
            //}
        }
    }
}