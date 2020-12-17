//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace RichTodd.QuiltSystem.Web
{
    public sealed class LoggingActionFilterAttribute : ActionFilterAttribute, IFilterFactory
    {
        public bool IsReusable
        {
            get { return false; }
        }

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            return new LoggingActionFilterAttribute(
                serviceProvider.GetService<ILogger>());
        }

        private readonly ILogger m_logger;

        public LoggingActionFilterAttribute()
        { }

        public LoggingActionFilterAttribute(ILogger logger)
        {
            m_logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.Controller is IApplicationController)
            {
                var controllerName = (string)filterContext.RouteData.Values["controller"];
                var actionName = (string)filterContext.RouteData.Values["action"];

                //m_logger.LogBeginFunction(controllerName, actionName);

                foreach (var key in filterContext.RouteData.Values.Keys)
                {
                    if (key != "controller" && key != "action")
                    {
                        var value = filterContext.RouteData.Values[key];
                        //m_logger.LogParameter("Route", key, value);
                    }
                }

                foreach (var key in filterContext.ActionArguments.Keys)
                {
                    if (key != "model")
                    {
                        var value = filterContext.ActionArguments[key];
                        //m_logger.LogParameter("Action", key, value);
                    }
                }

                foreach (var key in filterContext.HttpContext.Request.Query.Keys)
                {
                    var value = filterContext.HttpContext.Request.Query[key];
                    //m_logger.LogParameter("Query", key, value);
                }

                // Note: filterContext.HttpContext.Request.Form unavailable during OnActionExecuting.
            }
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            if (filterContext.Controller is IApplicationController)
            {
                //m_logger.LogEndFunction();
            }
        }

    }
}