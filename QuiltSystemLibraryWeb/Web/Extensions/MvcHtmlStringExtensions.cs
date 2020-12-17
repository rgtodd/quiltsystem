//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using Microsoft.AspNetCore.Html;

namespace RichTodd.QuiltSystem.Web.Extensions
{
    public static class MvcHtmlStringExtensions
    {
        public static HtmlString Append(this HtmlString mvcHtmlString, HtmlString otherMvcHtmlString)
        {
            return !string.IsNullOrEmpty(otherMvcHtmlString.Value)
                ? new HtmlString(mvcHtmlString.Value + otherMvcHtmlString.Value)
                : mvcHtmlString;
        }
    }
}
