//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Diagnostics.CodeAnalysis;

using Microsoft.AspNetCore.Mvc;

using RichTodd.QuiltSystem.Web.Paging;

namespace RichTodd.QuiltSystem.Web.Extensions
{
    public static class ControllerExtensions
    {
        public static ActionData GetActionData(this Controller controller)
        {
            foreach (var key in controller.Request.Form.Keys)
            {
                if (ActionData.HasActionData(key))
                {
                    return ActionData.Parse(key);
                }
            }

            return null;
        }

        public static PagingState GetPagingState(this Controller controller, int index)
        {
            var pagingState = new PagingState(controller.GetSort(index), controller.GetDescending(index), controller.GetPage(index), controller.GetFilter(index));

            return pagingState;
        }

        public static void SetPagingState(this Controller controller, string filter)
        {
            if (filter != null)
            {
                controller.ViewBag.Filter = filter;
            }
        }

        [SuppressMessage("Style", "IDE0058:Expression value is never used", Justification = "Warning caused by dyanmic parameter.")]
        public static void SetSortDefault(this Controller controller, string sort, int index = 0)
        {
            var existingSort = ViewBagHelper.GetSort(controller.ViewBag, index);
            if (string.IsNullOrEmpty(existingSort))
            {
                ViewBagHelper.SetSort(controller.ViewBag, sort, index);
            }
        }

        [SuppressMessage("Style", "IDE0058:Expression value is never used", Justification = "Warning caused by dynamic parameter.")]
        public static void SetFilterDefault(this Controller controller, string filter, int index = 0)
        {
            var existingFilter = ViewBagHelper.GetFilter(controller.ViewBag, index);
            if (string.IsNullOrEmpty(existingFilter))
            {
                ViewBagHelper.SetFilter(controller.ViewBag, filter, index);
            }
        }

        private static bool GetDescending(this Controller controller, int index)
        {
            return ViewBagHelper.GetDescending(controller.ViewBag, index);
        }

        private static string GetFilter(this Controller controller, int index)
        {
            return ViewBagHelper.GetFilter(controller.ViewBag, index);
        }

        private static int? GetPage(this Controller controller, int index)
        {
            return ViewBagHelper.GetPage(controller.ViewBag, index);
        }

        private static string GetSort(this Controller controller, int index)
        {
            return ViewBagHelper.GetSort(controller.ViewBag, index);
        }
    }
}