//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.IO;
using System.Text.Encodings.Web;

namespace Microsoft.AspNetCore.Mvc.Rendering
{
    public static class QuiltSystemTagBuilderExtensions
    {
        public static string GetHtml(this TagBuilder tagBuilder)
        {
            using var wtr = new StringWriter();

            tagBuilder.WriteTo(wtr, HtmlEncoder.Default);

            return wtr.ToString();
        }
    }
}
