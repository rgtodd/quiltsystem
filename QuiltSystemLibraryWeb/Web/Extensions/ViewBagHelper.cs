//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Text;

using Microsoft.Extensions.Primitives;

namespace RichTodd.QuiltSystem.Web.Extensions
{
    internal static class ViewBagHelper
    {
        private const char Delimiter = '~';
        private static readonly string[] EmptyStringArray = Array.Empty<string>();

        public static bool GetDescending(dynamic viewBag, int index)
        {
            var value = GetDescendingValue(viewBag);

            var item = GetItem(value, index);

            return !string.IsNullOrEmpty(item) ? (bool)bool.Parse(item) : false;
        }

        public static string GetDescendingValue(dynamic viewBag)
        {
            string descending = viewBag.Descending;

            return descending;
        }

        public static string GetFilter(dynamic viewBag, int index)
        {
            var value = GetFilterValue(viewBag);

            var item = GetItem(value, index);

            return item;
        }

        public static string GetFilterValue(dynamic viewBag)
        {
            string filter = viewBag.Filter;

            return filter;
        }

        public static int? GetPage(dynamic viewBag, int index)
        {
            var value = GetPageValue(viewBag);

            var item = GetItem(value, index);

            return !string.IsNullOrEmpty(item) ? (int?)int.Parse(item) : null;
        }

        public static string GetPageValue(dynamic viewBag)
        {
            string page = viewBag.Page;

            return page;
        }

        public static string GetSort(dynamic viewBag, int index)
        {
            var value = GetSortValue(viewBag);

            var item = GetItem(value, index);

            return item;
        }

        public static string GetSortValue(dynamic viewBag)
        {
            string sort = viewBag.Sort;

            return sort;
        }

        public static void SetDescendingValue(dynamic viewBag, StringValues value)
        {
            viewBag.Descending = GetFirstValue(value);
        }

        public static void SetFilter(dynamic viewBag, string value, int index)
        {
            var filterValue = GetFilterValue(viewBag);
            filterValue = ReplaceItem(filterValue, value, index);
            SetFilterValue(viewBag, filterValue);
        }

        public static void SetFilterValue(dynamic viewBag, StringValues value)
        {
            viewBag.Filter = GetFirstValue(value);
        }

        public static void SetPageValue(dynamic viewBag, StringValues value)
        {
            viewBag.Page = GetFirstValue(value);
        }

        public static void SetSort(dynamic viewBag, string value, int index)
        {
            var sortValue = GetSortValue(viewBag);
            sortValue = ReplaceItem(sortValue, value, index);
            SetSortValue(viewBag, sortValue);
        }

        public static void SetSortValue(dynamic viewBag, StringValues value)
        {
            viewBag.Sort = GetFirstValue(value);
        }

        private static string GetFirstValue(StringValues value)
        {
            return string.IsNullOrEmpty(value) ? null : value[0];
        }

        private static string GetItem(string value, int index)
        {
            if (string.IsNullOrEmpty(value)) return value;

            if (index == 0 && value.IndexOf(Delimiter) == -1)
            {
                return value;
            }

            var items = value.Split(new char[] { Delimiter });

            return index < items.Length ? items[index] : null;
        }

        private static string ReplaceItem(string value, string item, int index)
        {
            if (index == 0 && (string.IsNullOrEmpty(value) || value.IndexOf(Delimiter) == -1))
            {
                return item;
            }

            var items = !string.IsNullOrEmpty(value) ? value.Split(new char[] { Delimiter }) : EmptyStringArray;

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