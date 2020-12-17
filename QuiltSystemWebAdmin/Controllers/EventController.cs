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
using RichTodd.QuiltSystem.WebAdmin.Models.Event;

namespace RichTodd.QuiltSystem.WebAdmin.Controllers
{
    public class EventController : ApplicationController<EventModelFactory>
    {
        private IEventAdminService EventAdminService { get; }

        public EventController(
            IApplicationLocale applicationLocale,
            IDomainMicroService domainMicroService,
            IEventAdminService eventAdminService)
            : base(
                  applicationLocale,
                  domainMicroService)
        {
            EventAdminService = eventAdminService ?? throw new ArgumentNullException(nameof(eventAdminService));
        }

        public async Task<ActionResult> Index(string id, string unitOfWork, string source)
        {
            if (!string.IsNullOrEmpty(id))
            {
                throw new NotImplementedException();
            }
            else
            {
                string defaultSource = !string.IsNullOrEmpty(source)
                    ? source
                    : "*ANY";

                var defaultFilter = ModelFactory.CreateFilter(unitOfWork, defaultSource);

                this.SetSortDefault(ModelFactory.GetDefaultSort());
                this.SetFilterDefault(defaultFilter);

                var model = await GetFulfillmentEventLogListAsync(this.GetPagingState(0));

                return View("List", model);
            }
        }

        public async Task<ActionResult> ListSubmit()
        {
            return await Index(null, null, null);
        }

        [HttpPost]
        public async Task<ActionResult> ListSubmit(EventLogList model)
        {
            var filter = ModelFactory.CreateFilter(model.UnitOfWork, model.Source);

            this.SetPagingState(filter);

            model = await GetFulfillmentEventLogListAsync(this.GetPagingState(0));

            return View("List", model);
        }

        private async Task<EventLogList> GetFulfillmentEventLogListAsync(PagingState pagingState)
        {
            ModelFactory.ParseFilter(pagingState.Filter, out string unitOfWork, out string source);

            var aTrnsactionList = await EventAdminService.GetEventLogsAsync(unitOfWork, source != "*ANY" ? source : null);

            var model = ModelFactory.CreateEventLogList(aTrnsactionList, pagingState);

            return model;
        }
    }
}