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
using RichTodd.QuiltSystem.Web;
using RichTodd.QuiltSystem.Web.Extensions;
using RichTodd.QuiltSystem.WebAdmin.Models.Fundable;

namespace RichTodd.QuiltSystem.WebAdmin.Controllers
{
    [Authorize(Policy = ApplicationPolicies.AllowViewFinancial)]
    public class FundableController : ApplicationController<FundableModelFactory>
    {
        private IFundingMicroService FundingMicroService { get; }
        private IFundingAdminService FundingAdminService { get; }

        public FundableController(
            IApplicationLocale applicationLocale,
            IDomainMicroService domainMicroService,
            IFundingMicroService fundingMicroService,
            IFundingAdminService fundingAdminService)
            : base(
                  applicationLocale,
                  domainMicroService)
        {
            FundingMicroService = fundingMicroService ?? throw new ArgumentNullException(nameof(fundingMicroService));
            FundingAdminService = fundingAdminService ?? throw new ArgumentNullException(nameof(fundingAdminService));
        }

        public async Task<ActionResult> Index(long? id)
        {
            if (id != null)
            {
                var model = await GetFundableAsync(id.Value);

                return View("Detail", model);
            }
            else
            {
                this.SetSortDefault(ModelFactory.GetDefaultSort());
                this.SetFilterDefault(ModelFactory.CreatePagingStateFilter(null, ModelFactory.DefaultRecordCount));

                var model = await GetFundableListAsync();

                return View("List", model);
            }
        }

        public async Task<ActionResult> ListSubmit()
        {
            return await Index(null);
        }

        [HttpPost]
        public async Task<ActionResult> ListSubmit(FundableList model)
        {
            this.SetPagingState(ModelFactory.CreatePagingStateFilter(model.Filter));

            model = await GetFundableListAsync();

            return View("List", model);
        }

        private async Task<Fundable> GetFundableAsync(long fundableId)
        {
            var aFundable = await FundingAdminService.GetFundableAsync(fundableId);

            var model = ModelFactory.CreateFundable(aFundable);

            return model;
        }

        private async Task<FundableList> GetFundableListAsync()
        {
            var pagingState = this.GetPagingState(0);

            var (hasFundsRequired, recordCount) = ModelFactory.ParsePagingStateFilter(pagingState.Filter);

            var mFundables = await FundingMicroService.GetFundableSummariesAsync(null, hasFundsRequired, recordCount);

            var model = ModelFactory.CreateFundableList(mFundables.Summaries, pagingState);

            return model;
        }
    }
}