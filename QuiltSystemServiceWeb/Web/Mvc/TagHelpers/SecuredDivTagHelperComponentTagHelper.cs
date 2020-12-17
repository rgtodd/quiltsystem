//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Logging;

namespace RichTodd.QuiltSystem.Web.Mvc.TagHelpers
{
    [HtmlTargetElement(SecuredDivTagHelperComponent.Tag)]
    public class SecuredDivTagHelperComponentTagHelper : TagHelperComponentTagHelper
    {
        public string Policy { get; set; }
        public string Tag { get; set; }

        public SecuredDivTagHelperComponentTagHelper(
            ITagHelperComponentManager componentManager,
            ILoggerFactory loggerFactory)
            : base(componentManager, loggerFactory)
        { }
    }
}
