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
using RichTodd.QuiltSystem.WebAdmin.Models.Alert;

namespace RichTodd.QuiltSystem.WebAdmin.Controllers
{
    public class AlertController : ApplicationController<AlertModelFactory>
    {
        private IAlertAdminService AlertAdminService { get; }

        public AlertController(
            IApplicationLocale applicationLocale,
            IDomainMicroService domainMicroService,
            IAlertAdminService alertMicroService)
            : base(
                  applicationLocale,
                  domainMicroService)
        {
            AlertAdminService = alertMicroService ?? throw new ArgumentNullException(nameof(alertMicroService));
        }

        public async Task<ActionResult> Index(long? id)
        {
            if (id.HasValue)
            {
                var model = await GetAlertAsync(id.Value);

                return View("Detail", model);
            }
            else
            {
                this.SetSortDefault(ModelFactory.GetDefaultSort());
                this.SetFilterDefault(ModelFactory.CreatePagingStateFilter(false, ModelFactory.DefaultRecordCount));

                var model = await GetAlertListAsync();

                return View("List", model);
            }
        }

        public async Task<ActionResult> DetailSubmit(long? id)
        {
            return await Index(id);
        }

        [HttpPost]
        public async Task<ActionResult> DetailSubmit(AlertDetailSubmit model)
        {
            var actionData = this.GetActionData();
            switch (actionData?.ActionName)
            {
                case Actions.MarkComplete:
                    await AlertAdminService.AcknowledgeAlertAsync(model.AlertId);
                    return RedirectToAction("Index", new { id = model.AlertId });
            }

            return RedirectToAction("Index", new { id = model.AlertId });
        }

        public async Task<ActionResult> ListSubmit()
        {
            return await Index(null);
        }

        [HttpPost]
        public async Task<ActionResult> ListSubmit(AlertList model)
        {
            var actionData = this.GetActionData();
            switch (actionData?.ActionName)
            {
                case Actions.MarkAllComplete:
                    await AlertAdminService.AcknowledgeAlertsAsync();
                    break;
            }

            this.SetPagingState(ModelFactory.CreatePagingStateFilter(model.Filter));

            model = await GetAlertListAsync();

            return View("List", model);
        }

        #region Methods

        private async Task<Alert> GetAlertAsync(long alertId)
        {
            var aAlert = await AlertAdminService.GetAlertAsync(alertId);

            var model = ModelFactory.CreateAlert(aAlert);

            return model;
        }

        private async Task<AlertList> GetAlertListAsync()
        {
            var pagingState = this.GetPagingState(0);

            var (acknowledged, recordCount) = ModelFactory.ParsePagingStateFilter(pagingState.Filter);

            var aAlertList = await AlertAdminService.GetAlertsAsync(acknowledged, recordCount);

            var model = ModelFactory.CreateAlertList(aAlertList.Alerts, pagingState);

            return model;
        }

        #endregion Methods
    }
}