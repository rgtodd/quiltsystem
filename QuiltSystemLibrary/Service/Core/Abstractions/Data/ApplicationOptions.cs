//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Service.Core.Abstractions.Data
{
    public class ApplicationOptions
    {
        public bool DeferJobProcessing { get; set; }
        public string SendGridApiKey { get; set; }
        public bool RenderUnauthorizedContent { get; set; }
    }
}
