//
// Source code derived from the GitHub project at https://github.com/troygoode/PagedList
// Licensed under the MIT license.  See LICENSE.txt project file for more information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PagedList.Mvc
{
    ///<summary>
    ///	Extension methods for generating paging controls that can operate on instances of IPagedList.
    ///</summary>
    public static class HtmlHelper
    {
        private static TagBuilder WrapInListItem(string text)
        {
            var li = new TagBuilder("li");
            _ = li.InnerHtml.Append(text);
            return li;
        }

        private static TagBuilder WrapInListItem(TagBuilder inner, PagedListRenderOptions options, params string[] classes)
        {
            var li = new TagBuilder("li");
            foreach (var @class in classes)
                li.AddCssClass(@class);
            if (options.FunctionToTransformEachPageLink != null)
                return options.FunctionToTransformEachPageLink(li, inner);
            _ = li.InnerHtml.AppendHtml(inner.GetHtml());
            return li;
        }

        private static TagBuilder First(IPagedList list, Func<int, string> generatePageUrl, PagedListRenderOptions options)
        {
            const int targetPageNumber = 1;
            var first = new TagBuilder("a");
            {
                _ = first.InnerHtml.AppendHtml(string.Format(options.LinkToFirstPageFormat, targetPageNumber));
            }
            first.AddCssClass("page-link");

            if (list.IsFirstPage)
                return WrapInListItem(first, options, "PagedList-skipToFirst", "disabled");

            first.Attributes["href"] = generatePageUrl(targetPageNumber);
            return WrapInListItem(first, options, "PagedList-skipToFirst");
        }

        private static TagBuilder Previous(IPagedList list, Func<int, string> generatePageUrl, PagedListRenderOptions options)
        {
            var targetPageNumber = list.PageNumber - 1;
            var previous = new TagBuilder("a");
            {
                _ = previous.InnerHtml.AppendHtml(string.Format(options.LinkToPreviousPageFormat, targetPageNumber));
            }
            previous.AddCssClass("page-link");
            previous.Attributes["rel"] = "prev";

            if (!list.HasPreviousPage)
                return WrapInListItem(previous, options, "PagedList-skipToPrevious", "disabled");

            previous.Attributes["href"] = generatePageUrl(targetPageNumber);
            return WrapInListItem(previous, options, "PagedList-skipToPrevious");
        }

        private static TagBuilder Page(int i, IPagedList list, Func<int, string> generatePageUrl, PagedListRenderOptions options)
        {
            var format = options.FunctionToDisplayEachPageNumber
                ?? (pageNumber => string.Format(options.LinkToIndividualPageFormat, pageNumber));
            var targetPageNumber = i;
            var page = new TagBuilder("a");
            page.AddCssClass("page-link");
            _ = page.InnerHtml.AppendHtml(format(targetPageNumber));

            if (i == list.PageNumber)
                return WrapInListItem(page, options, "active");

            page.Attributes["href"] = generatePageUrl(targetPageNumber);
            return WrapInListItem(page, options);
        }

        private static TagBuilder Next(IPagedList list, Func<int, string> generatePageUrl, PagedListRenderOptions options)
        {
            var targetPageNumber = list.PageNumber + 1;
            var next = new TagBuilder("a");
            {
                _ = next.InnerHtml.AppendHtml(string.Format(options.LinkToNextPageFormat, targetPageNumber));
            }
            next.AddCssClass("page-link");
            next.Attributes["rel"] = "next";

            if (!list.HasNextPage)
                return WrapInListItem(next, options, "PagedList-skipToNext", "disabled");

            next.Attributes["href"] = generatePageUrl(targetPageNumber);
            return WrapInListItem(next, options, "PagedList-skipToNext");
        }

        private static TagBuilder Last(IPagedList list, Func<int, string> generatePageUrl, PagedListRenderOptions options)
        {
            var targetPageNumber = list.PageCount;
            var last = new TagBuilder("a");
            {
                _ = last.InnerHtml.AppendHtml(string.Format(options.LinkToLastPageFormat, targetPageNumber));
            }
            last.AddCssClass("page-link");
            if (list.IsLastPage)
                return WrapInListItem(last, options, "PagedList-skipToLast", "disabled");

            last.Attributes["href"] = generatePageUrl(targetPageNumber);
            return WrapInListItem(last, options, "PagedList-skipToLast");
        }

        private static TagBuilder PageCountAndLocationText(IPagedList list, PagedListRenderOptions options)
        {
            var text = new TagBuilder("a");
            text.AddCssClass("page-link");
            _ = text.InnerHtml.AppendHtml(string.Format(options.PageCountAndCurrentLocationFormat, list.PageNumber, list.PageCount));

            return WrapInListItem(text, options, "PagedList-pageCountAndLocation", "disabled");
        }

        private static TagBuilder ItemSliceAndTotalText(IPagedList list, PagedListRenderOptions options)
        {
            var text = new TagBuilder("a");
            text.AddCssClass("page-link");
            _ = text.InnerHtml.AppendHtml(string.Format(options.ItemSliceAndTotalFormat, list.FirstItemOnPage, list.LastItemOnPage, list.TotalItemCount));

            return WrapInListItem(text, options, "PagedList-pageCountAndLocation", "disabled");
        }

        private static TagBuilder Ellipses(PagedListRenderOptions options)
        {
            var a = new TagBuilder("a");
            {
                _ = a.InnerHtml.AppendHtml(options.EllipsesFormat);
            }
            a.AddCssClass("page-link");
            return WrapInListItem(a, options, "PagedList-ellipses", "disabled");
        }

        ///<summary>
        ///	Displays a configurable paging control for instances of PagedList.
        ///</summary>
        ///<param name = "html">This method is meant to hook off HtmlHelper as an extension method.</param>
        ///<param name = "list">The PagedList to use as the data source.</param>
        ///<param name = "generatePageUrl">A function that takes the page number of the desired page and returns a URL-string that will load that page.</param>
        ///<returns>Outputs the paging control HTML.</returns>
        public static HtmlString PagedListPager(this IHtmlHelper html,
                                                   IPagedList list,
                                                   Func<int, string> generatePageUrl)
        {
            return PagedListPager(html, list, generatePageUrl, new PagedListRenderOptions());
        }

        ///<summary>
        ///	Displays a configurable paging control for instances of PagedList.
        ///</summary>
        ///<param name = "html">This method is meant to hook off HtmlHelper as an extension method.</param>
        ///<param name = "list">The PagedList to use as the data source.</param>
        ///<param name = "generatePageUrl">A function that takes the page number  of the desired page and returns a URL-string that will load that page.</param>
        ///<param name = "options">Formatting options.</param>
        ///<returns>Outputs the paging control HTML.</returns>
#pragma warning disable IDE0060 // Remove unused parameter
        public static HtmlString PagedListPager(this IHtmlHelper html,
#pragma warning restore IDE0060 // Remove unused parameter
                                                   IPagedList list,
                                                   Func<int, string> generatePageUrl,
                                                   PagedListRenderOptions options)
        {
            if (options.Display == PagedListDisplayMode.Never || (options.Display == PagedListDisplayMode.IfNeeded && list.PageCount <= 1))
                return null;

            var listItemLinks = new List<TagBuilder>();

            //calculate start and end of range of page numbers
            var firstPageToDisplay = 1;
            var lastPageToDisplay = list.PageCount;
            var pageNumbersToDisplay = lastPageToDisplay;
            if (options.MaximumPageNumbersToDisplay.HasValue && list.PageCount > options.MaximumPageNumbersToDisplay)
            {
                // cannot fit all pages into pager
                var maxPageNumbersToDisplay = options.MaximumPageNumbersToDisplay.Value;
                firstPageToDisplay = list.PageNumber - (maxPageNumbersToDisplay / 2);
                if (firstPageToDisplay < 1)
                    firstPageToDisplay = 1;
                pageNumbersToDisplay = maxPageNumbersToDisplay;
                lastPageToDisplay = firstPageToDisplay + pageNumbersToDisplay - 1;
                if (lastPageToDisplay > list.PageCount)
                    firstPageToDisplay = list.PageCount - maxPageNumbersToDisplay + 1;
            }

            //first
            if (options.DisplayLinkToFirstPage == PagedListDisplayMode.Always || (options.DisplayLinkToFirstPage == PagedListDisplayMode.IfNeeded && firstPageToDisplay > 1))
                listItemLinks.Add(First(list, generatePageUrl, options));

            //previous
            if (options.DisplayLinkToPreviousPage == PagedListDisplayMode.Always || (options.DisplayLinkToPreviousPage == PagedListDisplayMode.IfNeeded && !list.IsFirstPage))
                listItemLinks.Add(Previous(list, generatePageUrl, options));

            //text
            if (options.DisplayPageCountAndCurrentLocation)
                listItemLinks.Add(PageCountAndLocationText(list, options));

            //text
            if (options.DisplayItemSliceAndTotal)
                listItemLinks.Add(ItemSliceAndTotalText(list, options));

            //page
            if (options.DisplayLinkToIndividualPages)
            {
                //if there are previous page numbers not displayed, show an ellipsis
                if (options.DisplayEllipsesWhenNotShowingAllPageNumbers && firstPageToDisplay > 1)
                    listItemLinks.Add(Ellipses(options));

                foreach (var i in Enumerable.Range(firstPageToDisplay, pageNumbersToDisplay))
                {
                    //show delimiter between page numbers
                    if (i > firstPageToDisplay && !string.IsNullOrWhiteSpace(options.DelimiterBetweenPageNumbers))
                        listItemLinks.Add(WrapInListItem(options.DelimiterBetweenPageNumbers));

                    //show page number link
                    listItemLinks.Add(Page(i, list, generatePageUrl, options));
                }

                //if there are subsequent page numbers not displayed, show an ellipsis
                if (options.DisplayEllipsesWhenNotShowingAllPageNumbers && (firstPageToDisplay + pageNumbersToDisplay - 1) < list.PageCount)
                    listItemLinks.Add(Ellipses(options));
            }

            //next
            if (options.DisplayLinkToNextPage == PagedListDisplayMode.Always || (options.DisplayLinkToNextPage == PagedListDisplayMode.IfNeeded && !list.IsLastPage))
                listItemLinks.Add(Next(list, generatePageUrl, options));

            //last
            if (options.DisplayLinkToLastPage == PagedListDisplayMode.Always || (options.DisplayLinkToLastPage == PagedListDisplayMode.IfNeeded && lastPageToDisplay < list.PageCount))
                listItemLinks.Add(Last(list, generatePageUrl, options));

            if (listItemLinks.Any())
            {
                //append class to first item in list?
                if (!string.IsNullOrWhiteSpace(options.ClassToApplyToFirstListItemInPager))
                    listItemLinks.First().AddCssClass(options.ClassToApplyToFirstListItemInPager);

                //append class to last item in list?
                if (!string.IsNullOrWhiteSpace(options.ClassToApplyToLastListItemInPager))
                    listItemLinks.Last().AddCssClass(options.ClassToApplyToLastListItemInPager);

                //append classes to all list item links
                foreach (var li in listItemLinks)
                {
                    foreach (var c in options.LiElementClasses ?? Enumerable.Empty<string>())
                        li.AddCssClass(c);
                }
            }

            //collapse all of the list items into one big string
            var listItemLinksString = listItemLinks.Aggregate(
                new StringBuilder(),
                (sb, listItem) => sb.Append(listItem.GetHtml()),
                sb => sb.ToString()
                );

            var ul = new TagBuilder("ul");
            {
                _ = ul.InnerHtml.AppendHtml(listItemLinksString);
            }
            foreach (var c in options.UlElementClasses ?? Enumerable.Empty<string>())
                ul.AddCssClass(c);

            var outerDiv = new TagBuilder("div");
            foreach (var c in options.ContainerDivClasses ?? Enumerable.Empty<string>())
                outerDiv.AddCssClass(c);
            _ = outerDiv.InnerHtml.AppendHtml(ul.GetHtml());

            return new HtmlString(outerDiv.GetHtml());
        }

        ///<summary>
        /// Displays a configurable "Go To Page:" form for instances of PagedList.
        ///</summary>
        ///<param name="html">This method is meant to hook off HtmlHelper as an extension method.</param>
        ///<param name="list">The PagedList to use as the data source.</param>
        ///<param name="formAction">The URL this form should submit the GET request to.</param>
        ///<returns>Outputs the "Go To Page:" form HTML.</returns>
        public static HtmlString PagedListGoToPageForm(this IHtmlHelper html,
                                                          IPagedList list,
                                                          string formAction)
        {
            return PagedListGoToPageForm(html, list, formAction, "page");
        }

        ///<summary>
        /// Displays a configurable "Go To Page:" form for instances of PagedList.
        ///</summary>
        ///<param name="html">This method is meant to hook off HtmlHelper as an extension method.</param>
        ///<param name="list">The PagedList to use as the data source.</param>
        ///<param name="formAction">The URL this form should submit the GET request to.</param>
        ///<param name="inputFieldName">The querystring key this form should submit the new page number as.</param>
        ///<returns>Outputs the "Go To Page:" form HTML.</returns>
        public static HtmlString PagedListGoToPageForm(this IHtmlHelper html,
                                                          IPagedList list,
                                                          string formAction,
                                                          string inputFieldName)
        {
            return PagedListGoToPageForm(html, list, formAction, new GoToFormRenderOptions(inputFieldName));
        }

        ///<summary>
        /// Displays a configurable "Go To Page:" form for instances of PagedList.
        ///</summary>
        ///<param name="html">This method is meant to hook off HtmlHelper as an extension method.</param>
        ///<param name="list">The PagedList to use as the data source.</param>
        ///<param name="formAction">The URL this form should submit the GET request to.</param>
        ///<param name="options">Formatting options.</param>
        ///<returns>Outputs the "Go To Page:" form HTML.</returns>
#pragma warning disable IDE0060 // Remove unused parameter
        public static HtmlString PagedListGoToPageForm(this IHtmlHelper html,
#pragma warning restore IDE0060 // Remove unused parameter
                                                 IPagedList list,
                                                 string formAction,
                                                 GoToFormRenderOptions options)
        {
            var form = new TagBuilder("form");
            form.AddCssClass("PagedList-goToPage");
            form.Attributes.Add("action", formAction);
            form.Attributes.Add("method", "get");

            var fieldset = new TagBuilder("fieldset");

            var label = new TagBuilder("label");
            label.Attributes.Add("for", options.InputFieldName);
            _ = label.InnerHtml.AppendHtml(options.LabelFormat);

            var input = new TagBuilder("input");
            input.Attributes.Add("type", options.InputFieldType);
            input.Attributes.Add("name", options.InputFieldName);
            input.Attributes.Add("value", list.PageNumber.ToString());

            var submit = new TagBuilder("input");
            submit.Attributes.Add("type", "submit");
            submit.Attributes.Add("value", options.SubmitButtonFormat);

            _ = fieldset.InnerHtml.AppendHtml(label.GetHtml());
            _ = fieldset.InnerHtml.AppendHtml(input.GetHtml());
            _ = fieldset.InnerHtml.AppendHtml(submit.GetHtml());

            _ = form.InnerHtml.AppendHtml(fieldset.GetHtml());

            return new HtmlString(form.GetHtml());
        }
    }
}