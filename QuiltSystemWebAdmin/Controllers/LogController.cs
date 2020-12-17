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
using RichTodd.QuiltSystem.Web.Paging;
using RichTodd.QuiltSystem.WebAdmin.Models.Log;

namespace RichTodd.QuiltSystem.WebAdmin.Controllers
{
    public class LogController : ApplicationController<LogModelFactory>
    {
        private ILogAdminService LogAdminService { get; }

        public LogController(
            IApplicationLocale applicationLocale,
            IDomainMicroService domainMicroService,
            ILogAdminService logAdminService)
            : base(
                  applicationLocale,
                  domainMicroService)
        {
            LogAdminService = logAdminService ?? throw new ArgumentNullException(nameof(logAdminService));
        }

        public Task<ActionResult> DetailSubmit(int? id)
        {
            return Index(id);
        }

        [HttpPost]
        public Task<ActionResult> DetailSubmit(LogEntryDetailModel model)
        {
            //switch (this.GetAction())
            //{
            //}

            return Index(model.LogEntry.LogEntryId);
        }

        public async Task<ActionResult> Index(long? id)
        {
            if (id.HasValue)
            {
                var model = await GetLogEntryDetailModel(id.Value);

                return View("Detail", model);
            }
            else
            {
                var model = await GetLogEntryListModel(this.GetPagingState(0));

                return View("List", model);
            }
        }

        public Task<ActionResult> ListSubmit()
        {
            return Index(null);
        }

        [HttpPost]
        public async Task<ActionResult> ListSubmit(LogEntryListModel model)
        {
            //switch (this.GetAction())
            //{
            //}

            this.SetPagingState(model.Filter);

            model = await GetLogEntryListModel(this.GetPagingState(0));

            return View("List", model);
        }

        #region Methods

        private async Task<LogEntryDetailModel> GetLogEntryDetailModel(long logId)
        {
            var svcLogEntry = await LogAdminService.GetLogEntryAsync(logId);

            var model = ModelFactory.CreateLogEntryDetailModel(svcLogEntry);

            return model;
        }

        private async Task<LogEntryListModel> GetLogEntryListModel(PagingState pagingSate)
        {
            var svcLogEntries = await LogAdminService.GetLogEntriesAsync(pagingSate.Filter);

            var model = ModelFactory.CreateLogEntryListModel(svcLogEntries, pagingSate, LogAdminService);

            return model;
        }

        #endregion Methods
    }
}