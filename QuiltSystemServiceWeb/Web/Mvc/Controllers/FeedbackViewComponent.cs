//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using RichTodd.QuiltSystem.Web.Feedback;
using RichTodd.QuiltSystem.Web.Mvc.Models;

namespace RichTodd.QuiltSystem.Web.Mvc.Controllers
{
    public class FeedbackViewComponent : ViewComponent
    {
        public FeedbackViewComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            await Task.CompletedTask;

            var messages = GetFeedbackMessages();
            if (messages.Count == 0)
            {
                return Content(string.Empty);
            }
            else
            {
                var model = new FeedbackMessageListVcModel()
                {
                    Messages = new List<FeedbackMessageVcModel>()
                };

                foreach (var message in messages)
                {
                    model.Messages.Add(new FeedbackMessageVcModel()
                    {
                        MessageType = message.MessageType.ToString(),
                        Message = message.Message
                    });
                }

                return View(model);
            }
        }

        private IReadOnlyList<FeedbackMessage> GetFeedbackMessages()
        {
            return FeedbackPool.Singleton.GetFeedbackMessages(GetFeedbackId());
        }

        private Guid GetFeedbackId()
        {
            // See if an ID has been defined in the HttpContext.
            //
            var contextId = HttpContext.Items[FeedbackActionFilterAttribute.Key];
            if (contextId != null)
            {
                return (Guid)contextId;
            }

            // See if an ID has been specified in the query string.
            //
            var queryStringId = Request.Query[FeedbackActionFilterAttribute.Key];
            if (queryStringId.Count == 1)
            {
                return Guid.Parse(queryStringId[0]);
            }

            // Create a new ID and add it to the HttpContext.
            //
            var id = Guid.NewGuid();
            HttpContext.Items[FeedbackActionFilterAttribute.Key] = id;
            return id;
        }
    }
}
