//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Service.Core.Abstractions.Data
{
    public class ApplicationEmailRequest
    {
        public string SenderEmail { get; set; }
        public string SenderEmailName { get; set; }
        public string RecipientEmail { get; set; }
        public string RecipientEmailName { get; set; }
        public string Subject { get; set; }
        public string BodyText { get; set; }
        public string BodyHtml { get; set; }
    }
}
