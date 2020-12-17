//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Linq.Expressions;
using System.Text;

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;

using RichTodd.QuiltSystem.Web.View;

namespace RichTodd.QuiltSystem.Web.Extensions
{
    public static class HtmlHelperExtensions
    {
        public const string DescendingParameter = "descending";
        public const string FilterParameter = "filter";
        public const string PageParameter = "page";
        public const string SearchParameter = "search";
        public const string SortParameter = "sort";

        private const char Delimiter = '~';
        private static readonly string[] EmptyStringArray = Array.Empty<string>();

        public static bool EcommerceEnabled(this IHtmlHelper htmlHelper)
        {
            var viewOptions = ViewOptions.Lookup(htmlHelper.ViewContext.HttpContext);

            return viewOptions != null && viewOptions.EcommerceEnabled;
        }

        public static IHtmlContent ListActionLink(this IHtmlHelper htmlHelper, string linkText, string actionName, object htmlAttributes, object id = null, int index = 0, string filter = null)
        {
            var filterValue = htmlHelper.GetFilterValue();
            if (filter != null)
            {
                filterValue = ReplaceItem(filterValue, filter, index);
            }

            return htmlHelper.ActionLink(
                linkText,
                actionName,
                new
                {
                    id,
                    descending = htmlHelper.GetDescendingValue(),
                    filter = filterValue,
                    page = htmlHelper.GetPageValue(),
                    sort = htmlHelper.GetSortValue()
                },
                htmlAttributes);
        }

        public static string ListActionUrl(this IHtmlHelper htmlHelper, string actionName, int page, int index = 0)
        {
            var urlHelperFactory = htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
            var urlHelper = urlHelperFactory.GetUrlHelper(htmlHelper.ViewContext);

            var pageValue = htmlHelper.GetPageValue();
            pageValue = ReplaceItem(pageValue, page.ToString(), index);

            var actionContext = new UrlActionContext()
            {
                Action = actionName,
                Values = new
                {
                    descending = htmlHelper.GetDescendingValue(),
                    filter = htmlHelper.GetFilterValue(),
                    page = pageValue,
                    sort = htmlHelper.GetSortValue()
                }
            };

            var url = urlHelper.Action(actionContext);

            return url;
        }

        public static MvcForm ListBeginForm(this IHtmlHelper htmlHelper, string actionName, string controllerName, FormMethod method, object htmlAttributes)
        {
            return htmlHelper.BeginForm(
                actionName,
                controllerName,
                new
                {
                    descending = htmlHelper.GetDescendingValue(),
                    filter = htmlHelper.GetFilterValue(),
                    page = htmlHelper.GetPageValue(),
                    sort = htmlHelper.GetSortValue()
                },
                method,
                false, // new antiforgery parameter.
                htmlAttributes);
        }

        public static IHtmlContent ListHeadingActionLink<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, int index = 0)
        {
            var column = htmlHelper.DisplayNameFor(expression);

            return ListHeadingActionLink(htmlHelper, column, index);
        }

        public static IHtmlContent ListHeadingActionLink<TModel>(this IHtmlHelper<TModel> htmlHelper, string column, int index = 0)
        {
            var sort = column;

            var descending = htmlHelper.GetSort(index) == column && !htmlHelper.GetDescending(index);

            return htmlHelper.ActionLink(
                column,
                "Index",
                new
                {
                    descending = ReplaceItem(htmlHelper.GetDescendingValue(), descending.ToString(), index),
                    filter = htmlHelper.GetFilterValue(),
                    page = htmlHelper.GetPageValue(),
                    sort = ReplaceItem(htmlHelper.GetSortValue(), sort, index)
                });
        }

        public static bool PublicRegistrationEnabled(this IHtmlHelper htmlHelper)
        {
            var viewOptions = ViewOptions.Lookup(htmlHelper.ViewContext.HttpContext);

            return viewOptions != null && viewOptions.PublicRegistrationEnabled;
        }

        public static IHtmlContent MultlineText(this IHtmlHelper htmlHelper, string text)
        {
            var sb = new StringBuilder();

            var lines = TextUtility.DecodeMultilineText(text);
            for (int idx = 0; idx < lines.Length; ++idx)
            {
                if (idx > 0)
                {
                    _ = sb.Append("<br>");
                }
                _ = sb.Append(htmlHelper.Encode(lines[idx]));
            }

            return new HtmlString(sb.ToString());
        }

        public static IHtmlContent DisplayForMultilineText<TModel>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, string>> expression)
        {
            var expressionValue = expression.Compile()(htmlHelper.ViewData.Model);

            var sb = new StringBuilder();

            var lines = TextUtility.DecodeMultilineText(expressionValue);
            for (int idx = 0; idx < lines.Length; ++idx)
            {
                if (idx > 0)
                {
                    _ = sb.Append("<br>");
                }
                _ = sb.Append(htmlHelper.Encode(lines[idx]));
            }

            return new HtmlString(sb.ToString());
        }

        private static bool GetDescending(this IHtmlHelper htmlHelper, int index)
        {
            var descending = ViewBagHelper.GetDescending(htmlHelper.ViewBag, index);

            return descending;
        }

        private static string GetDescendingValue(this IHtmlHelper htmlHelper)
        {
            var descending = ViewBagHelper.GetDescendingValue(htmlHelper.ViewBag);

            return descending;
        }

        private static string GetFilter(this IHtmlHelper htmlHelper, int index)
        {
            var filter = ViewBagHelper.GetFilter(htmlHelper.ViewBag, index);

            return filter;
        }

        private static string GetFilterValue(this IHtmlHelper htmlHelper)
        {
            var filter = ViewBagHelper.GetFilterValue(htmlHelper.ViewBag);

            return filter;
        }

        private static string GetPageValue(this IHtmlHelper htmlHelper)
        {
            var page = ViewBagHelper.GetPageValue(htmlHelper.ViewBag);

            return page;
        }

        private static string GetSort(this IHtmlHelper htmlHelper, int index)
        {
            var sort = ViewBagHelper.GetSort(htmlHelper.ViewBag, index);

            return sort;
        }

        private static string GetSortValue(this IHtmlHelper htmlHelper)
        {
            var sort = ViewBagHelper.GetSortValue(htmlHelper.ViewBag);

            return sort;
        }

        private static string GetItem(string value, int index)
        {
            if (string.IsNullOrEmpty(value)) return value;

            if (index == 0 && value.IndexOf(Delimiter) == -1)
            {
                return value;
            }

            var items = value.Split(new char[] { Delimiter });
            return items[index];
        }

        private static string ReplaceItem(string value, string item, int index)
        {
            if (index == 0 && (string.IsNullOrEmpty(value) || value.IndexOf(Delimiter) == -1))
            {
                return item;
            }

            var items = !string.IsNullOrEmpty(value)
                ? value.Split(new char[] { Delimiter }) :
                EmptyStringArray;

            var sb = new StringBuilder();
            for (var idx = 0; idx < Math.Max(index + 1, items.Length); ++idx)
            {
                if (idx > 0) _ = sb.Append(Delimiter);

                if (idx == index)
                {
                    _ = sb.Append(item);
                }
                else if (idx < items.Length)
                {
                    _ = sb.Append(items[idx]);
                }
            }

            return sb.ToString();
        }
    }
}