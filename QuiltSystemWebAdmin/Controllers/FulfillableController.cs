//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using RichTodd.QuiltSystem.Security;
using RichTodd.QuiltSystem.Service.Admin.Abstractions;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;
using RichTodd.QuiltSystem.Web;
using RichTodd.QuiltSystem.Web.Extensions;
using RichTodd.QuiltSystem.WebAdmin.Models.Fulfillable;

namespace RichTodd.QuiltSystem.WebAdmin.Controllers
{
    [Authorize(Policy = ApplicationPolicies.AllowViewFulfillment)]
    public class FulfillableController : ApplicationController<FulfillableModelFactory>
    {
        private IFulfillableAdminService FulfillmentAdminService { get; }

        public FulfillableController(
            IApplicationLocale applicationLocale,
            IDomainMicroService domainMicroService,
            IFulfillableAdminService fulfillmentAdminService)
            : base(
                  applicationLocale,
                  domainMicroService)
        {
            FulfillmentAdminService = fulfillmentAdminService ?? throw new ArgumentNullException(nameof(fulfillmentAdminService));
        }

        public async Task<ActionResult> Index(long? id)
        {
            if (id != null)
            {
                var model = await GetFulfillableAsync(id.Value);

                return View("Detail", model);
            }
            else
            {
                this.SetSortDefault(ModelFactory.GetDefaultSort());
                this.SetFilterDefault(ModelFactory.CreatePagingStateFilter(MFulfillment_FulfillableStatus.MetaAll, ModelFactory.DefaultRecordCount));

                var model = await GetFulfillableListAsync();

                return View("List", model);
            }
        }

        public async Task<ActionResult> ListSubmit()
        {
            return await Index(null);
        }

        [HttpPost]
        public async Task<ActionResult> ListSubmit(FulfillableList model)
        {
            this.SetPagingState(ModelFactory.CreatePagingStateFilter(model.Filter));

            model = await GetFulfillableListAsync();

            return View("List", model);
        }

        private async Task<Fulfillable> GetFulfillableAsync(long fulfillableId)
        {
            var aFulfillable = await FulfillmentAdminService.GetFulfillableAsync(fulfillableId);

            var model = ModelFactory.CreateFulfillable(aFulfillable);

            return model;
        }

        private async Task<FulfillableList> GetFulfillableListAsync()
        {
            var pagingState = this.GetPagingState(0);

            var (fulfillableStatus, recordCount) = ModelFactory.ParsePagingStateFilter(pagingState.Filter);

            var aFulfillables = await FulfillmentAdminService.GetFulfillableSummariesAsync(fulfillableStatus, recordCount);

            var model = ModelFactory.CreateFulfillableList(aFulfillables.MSummaries, pagingState);

            return model;
        }
    }
}