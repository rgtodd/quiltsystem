//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Linq;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

using RichTodd.QuiltSystem.Service.Database.Abstractions;

namespace RichTodd.QuiltSystem.Web.Locale
{
    public sealed class UserLocaleActionFilterAttribute : ActionFilterAttribute, IFilterFactory
    {
        private readonly IQuiltContextFactory m_quiltContextFactory;

        public UserLocaleActionFilterAttribute()
        { }

        public UserLocaleActionFilterAttribute(IQuiltContextFactory quiltContextFactory)
        {
            m_quiltContextFactory = quiltContextFactory ?? throw new ArgumentNullException(nameof(quiltContextFactory));
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            LoadHttpContext(filterContext.HttpContext);
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            SaveHttpContext(filterContext.HttpContext);
        }

        #region IFilterFactory

        public bool IsReusable
        {
            get { return false; }
        }

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            return new UserLocaleActionFilterAttribute(
                serviceProvider.GetService<IQuiltContextFactory>());
        }

        #endregion

        private string GetTimeZoneId(HttpContext httpContext)
        {
            var userId = httpContext.GetUserId();
            if (!string.IsNullOrEmpty(userId))
            {
                using var ctx = m_quiltContextFactory.Create();

                var dbAspNetUser = ctx.AspNetUsers.Where(r => r.Id == userId).SingleOrDefault();
                if (dbAspNetUser != null)
                {
                    var dbUserProfile = dbAspNetUser.UserProfileAspNetUsers.SingleOrDefault()?.UserProfile;
                    if (dbUserProfile != null &&
                        !string.IsNullOrEmpty(dbUserProfile.TimeZoneId))
                    {
                        return dbUserProfile.TimeZoneId;
                    }
                }
            }

            return TimeZoneInfo.Local.Id;
        }

        private void LoadHttpContext(HttpContext httpContext)
        {
            var requestCookie = httpContext.Request.Cookies[UserLocale.CookieName];
            if (requestCookie != null)
            {
                var userLocale = UserLocale.Parse(requestCookie);
                userLocale.AddTo(httpContext);
            }
            else
            {
                var timeZoneId = GetTimeZoneId(httpContext);
                var userLocale = new UserLocale(timeZoneId);
                userLocale.AddTo(httpContext);
            }
        }

        private void SaveHttpContext(HttpContext httpContext)
        {
            var userLocale = UserLocale.Lookup(httpContext);
            var requestCookie = httpContext.Request.Cookies[UserLocale.CookieName];

            if (userLocale != null)
            {
                if (requestCookie != null)
                {
                    if (requestCookie != userLocale.ToString())
                    {
                        httpContext.Response.Cookies.Append(UserLocale.CookieName, userLocale.ToString());
                    }
                    else
                    {
                        // No change required.
                    }
                }
                else // requestCookie == null
                {
                    httpContext.Response.Cookies.Append(UserLocale.CookieName, userLocale.ToString());
                }
            }
            else // userLocale == null
            {
                if (requestCookie != null)
                {
                    httpContext.Response.Cookies.Delete(UserLocale.CookieName);
                }
                else // requestCookie == null
                {
                    // No change required.
                }
            }
        }
    }
}