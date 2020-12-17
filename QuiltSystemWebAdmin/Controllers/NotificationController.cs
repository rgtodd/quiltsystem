//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using RichTodd.QuiltSystem.Service.Admin.Abstractions;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Web;
using RichTodd.QuiltSystem.Web.Extensions;
using RichTodd.QuiltSystem.WebAdmin.Models.Notification;

namespace RichTodd.QuiltSystem.WebAdmin.Controllers
{
    public class NotificationController : ApplicationController<NotificationModelFactory>
    {
        private INotificationAdminService NotificationAdminService { get; }

        public NotificationController(
            IApplicationLocale applicationLocale,
            IDomainMicroService domainMicroService,
            INotificationAdminService notificationAdminService)
            : base(
                  applicationLocale,
                  domainMicroService)
        {
            NotificationAdminService = notificationAdminService ?? throw new ArgumentNullException(nameof(notificationAdminService));
        }

        public async Task<ActionResult> Index(long? id)
        {
            if (id.HasValue)
            {
                var model = await GetNotificationAsync(id.Value);

                return View("Detail", model);
            }
            else
            {
                this.SetSortDefault(ModelFactory.GetDefaultSort());
                this.SetFilterDefault(ModelFactory.CreatePagingStateFilter(false, ModelFactory.DefaultRecordCount));

                var model = await GetNotificationListAsync();

                return View("List", model);
            }
        }

        public async Task<ActionResult> ListSubmit()
        {
            return await Index(null);
        }

        [HttpPost]
        public async Task<ActionResult> ListSubmit(NotificationList model)
        {
            var actionData = this.GetActionData();
            switch (actionData?.ActionName)
            {
                case Actions.Acknowledge:
                    {
                        await NotificationAdminService.AcknowledgeNotificationsAsync();
                    }
                    break;
            }

            this.SetPagingState(ModelFactory.CreatePagingStateFilter(model.Filter));

            model = await GetNotificationListAsync();

            return View("List", model);
        }

        public async Task<ActionResult> DetailSubmit(int? id)
        {
            return await Index(id);
        }

        [HttpPost]
        public async Task<ActionResult> DetailSubmit(NotificationDetailSubmit model)
        {
            var actionData = this.GetActionData();
            switch (actionData?.ActionName)
            {
                case Actions.Acknowledge:
                    {
                        await NotificationAdminService.AcknowledgeNotificationAsync(model.NotificationId);
                    }
                    break;
            }

            return await Index(model.NotificationId);
        }

        #region Methods

        private async Task<Notification> GetNotificationAsync(long notificationId)
        {
            var svcNotification = await NotificationAdminService.GetNotificationAsync(notificationId);

            var model = ModelFactory.CreateNotification(svcNotification);

            return model;
        }

        private async Task<NotificationList> GetNotificationListAsync()
        {
            var pagingState = this.GetPagingState(0);

            var (acknowledged, recordCount) = ModelFactory.ParsePagingStateFilter(pagingState.Filter);

            var aNotificationList = await NotificationAdminService.GetNotificationsAsync(acknowledged, recordCount);

            var model = ModelFactory.CreateNotificationListModel(aNotificationList.Notifications, pagingState);

            return model;
        }

        #endregion Methods
    }
}