//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using RichTodd.QuiltSystem.Service.Admin.Abstractions;
using RichTodd.QuiltSystem.Service.Base;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;
using RichTodd.QuiltSystem.Web;
using RichTodd.QuiltSystem.Web.Extensions;
using RichTodd.QuiltSystem.WebAdmin.Models.Message;

namespace RichTodd.QuiltSystem.WebAdmin.Controllers
{
    public class MessageController : ApplicationController<MessageModelFactory>
    {
        private IMessageAdminService MessageAdminService { get; }

        public MessageController(
            IApplicationLocale applicationLocale,
            IDomainMicroService domainMicroService,
            IMessageAdminService messageAdminService)
            : base(
                  applicationLocale,
                  domainMicroService)
        {
            MessageAdminService = messageAdminService ?? throw new ArgumentNullException(nameof(messageAdminService));
        }

        public async Task<ActionResult> Index(long? id)
        {
            if (id.HasValue)
            {
                var model = await GetMessageAsync(id.Value);

                return View("Detail", model);
            }
            else
            {
                this.SetSortDefault(ModelFactory.GetDefaultSort());
                this.SetFilterDefault(ModelFactory.CreatePagingStateFilter(MCommunication_MessageMailbox.FromUser, MCommunication_MessageStatus.Unacknowledged, ModelFactory.DefaultRecordCount));

                var model = await GetMessageListAsync();

                return View("List", model);
            }
        }

        public async Task<ActionResult> ListSubmit()
        {
            return await Index(null);
        }

        [HttpPost]
        public async Task<ActionResult> ListSubmit(MessageList model)
        {
            //switch (this.GetAction())
            //{
            //}

            this.SetPagingState(ModelFactory.CreatePagingStateFilter(model.Filter));

            model = await GetMessageListAsync();

            return View("List", model);
        }

        public async Task<ActionResult> Create(string userId, int? replyToMessageId, long? orderId)
        {
            if (replyToMessageId.HasValue)
            {
                var aMessage = await MessageAdminService.GetMessageAsync(replyToMessageId.Value);

                if (!string.IsNullOrEmpty(userId))
                {
                    if (aMessage.MMessage.UserId != userId)
                    {
                        throw new InvalidOperationException("User ID mismatch.");
                    }
                }

                var model = ModelFactory.CreateReplyMessage(aMessage.MMessage);

                return View("Reply", model);
            }

            if (!string.IsNullOrEmpty(userId))
            {
                var model = ModelFactory.CreateCreateMessage(userId, orderId);

                return View("Create", model);
            }

            throw new ArgumentException();
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateMessage model)
        {
            if (!ModelState.IsValid)
            {
                ModelFactory.RefreshMessageCreateModel(model);

                return View("Create", model);
            }

            try
            {
                _ = await MessageAdminService.SendOutboundMessageAsync(model.UserId, model.Subject, model.Text, null, model.OrderId);

                return RedirectToAction("Index", new { id = "" });
            }
            catch (ServiceException ex)
            {
                AddModelErrors(ex);

                ModelFactory.RefreshMessageCreateModel(model);

                return View("Create", model);
            }
        }

        public async Task<ActionResult> DetailSubmit(int? id)
        {
            return await Index(id);
        }

        [HttpPost]
        public async Task<ActionResult> DetailSubmit(MessageDetailSubmit model)
        {
            var actionData = this.GetActionData();
            switch (actionData?.ActionName)
            {
                case Actions.Reply:
                    return RedirectToAction("Create", new { replyToMessageId = model.MessageId });

                case Actions.Acknowledge:
                    {
                        await MessageAdminService.AcknowledgeMessageAsync(model.MessageId);
                    }
                    break;
            }

            return await Index(model.MessageId);
        }

        [HttpPost]
        public async Task<ActionResult> Reply(ReplyMessage model)
        {
            if (!ModelState.IsValid)
            {
                return View("Reply", model);
            }

            try
            {
                _ = await MessageAdminService.SendOutboundMessageAsync(model.UserId, model.Subject, model.Text, model.ReplyToMessageId, model.OrderId);

                return RedirectToAction("Index", new { id = "" });
            }
            catch (ServiceException ex)
            {
                AddModelErrors(ex);

                return View("Reply", model);
            }
        }

        #region Methods

        private async Task<Message> GetMessageAsync(long messageId)
        {
            var aMessage = await MessageAdminService.GetMessageAsync(messageId);

            var model = ModelFactory.CreateMessage(aMessage, null, false);

            return model;
        }

        private async Task<MessageList> GetMessageListAsync()
        {
            var pagingState = this.GetPagingState(0);

            var (mailbox, status, recordCount) = ModelFactory.ParsePagingStateFilter(pagingState.Filter);

            var aMessageList = await MessageAdminService.GetMessagesAsync(mailbox, status, recordCount);

            var model = ModelFactory.CreateMessageList(aMessageList.Messages, pagingState);

            return model;
        }

        #endregion Methods
    }
}