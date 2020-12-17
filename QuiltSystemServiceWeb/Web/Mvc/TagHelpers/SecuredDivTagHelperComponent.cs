//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

using RichTodd.QuiltSystem.Service.Core.Abstractions.Data;

namespace RichTodd.QuiltSystem.Web.Mvc.TagHelpers
{
    public class SecuredDivTagHelperComponent : TagHelperComponent
    {
        public const string Tag = "secured-div";
        public const string PolicyAttribute = "policy";
        public const string TagAttribute = "tag";

        private IAuthorizationService AuthorizationService { get; }
        private IHttpContextAccessor HttpContextAccessor { get; }
        private IOptionsMonitor<ApplicationOptions> OptionsMonitor { get; }

        public SecuredDivTagHelperComponent(
            IHttpContextAccessor httpContextAccessor,
            IAuthorizationService authorizationService,
            IOptionsMonitor<ApplicationOptions> optionsMonitor)
        {
            HttpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            AuthorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
            OptionsMonitor = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (!string.Equals(context.TagName, Tag, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (!context.AllAttributes.ContainsName(PolicyAttribute))
            {
                return;
            }
            var policyAttribute = context.AllAttributes[PolicyAttribute];
            if (!(policyAttribute.Value is string policy))
            {
                return;
            }

            string tag = null;
            if (context.AllAttributes.ContainsName(TagAttribute))
            {
                var tagAttribute = context.AllAttributes[TagAttribute];
                tag = tagAttribute.Value as string;
            }
            if (string.IsNullOrEmpty(tag))
            {
                tag = "div";
            }

            var user = HttpContextAccessor.HttpContext.User;

            var result = await AuthorizationService.AuthorizeAsync(user, policy);

            if (result.Succeeded)
            {
                output.TagName = tag;
            }
            else
            {
                if (OptionsMonitor.CurrentValue.RenderUnauthorizedContent)
                {
                    output.TagName = tag;
                    output.AddClass("bg-warning", HtmlEncoder.Default);
                    output.AddClass("p-1", HtmlEncoder.Default);
                }
                else
                {
                    output.SuppressOutput();
                }
            }
        }
    }
}
