//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RichTodd.QuiltSystem.Web.Extensions
{
    public static class TagBuilderExtensions
    {
        public static HtmlString ToMvcHtmlString(this TagBuilder tagBuilder)
        {
            return new HtmlString(tagBuilder.GetHtml());
        }

        //public static HtmlString ToMvcHtmlString(this TagBuilder tagBuilder, TagRenderMode tagRenderMode)
        //{
        //    return new HtmlString(tagBuilder.ToString(tagRenderMode));
        //}
    }
}
