//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using RichTodd.QuiltSystem.Service.Admin.Abstractions;
using RichTodd.QuiltSystem.Service.Admin.Abstractions.Data;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Web;
using RichTodd.QuiltSystem.Web.Extensions;
using RichTodd.QuiltSystem.WebAdmin.Models.Report;

namespace RichTodd.QuiltSystem.WebAdmin.Controllers
{
    public class ReportController : ApplicationController<ReportModelFactory>
    {
        private IReportAdminService ReportAdminService { get; }

        public ReportController(
            IApplicationLocale applicationLocale,
            IDomainMicroService domainMicroService,
            IReportAdminService reportAdminService)
            : base(
                  applicationLocale,
                  domainMicroService)
        {
            ReportAdminService = reportAdminService ?? throw new ArgumentNullException(nameof(reportAdminService));
        }

        public async Task<ActionResult> Index()
        {
            var model = await GetReportModel(this.GetPagingState(0).Filter);

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Index(ReportModel model)
        {
            //switch (this.GetAction())
            //{
            //}

            model = await GetReportModel(model.Filter);

            return View(model);
        }

        #region Methods

        private async Task<ReportModel> GetReportModel(string filter)
        {
            string html = null;
            if (!string.IsNullOrEmpty(filter))
            {
                var request = new AReport_GetReport()
                {
                    ReportName = filter
                };

                var response = await ReportAdminService.GetReportAsync(request);
                html = response.Html;
            }

            var model = ModelFactory.CreateReportModel(html, filter);

            return model;
        }

        #endregion Methods
    }
}