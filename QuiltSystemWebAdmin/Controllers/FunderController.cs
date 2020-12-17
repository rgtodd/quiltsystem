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
using RichTodd.QuiltSystem.WebAdmin.Models.Funder;

namespace RichTodd.QuiltSystem.WebAdmin.Controllers
{
    [Authorize(Policy = ApplicationPolicies.AllowViewFinancial)]
    public class FunderController : ApplicationController<FunderModelFactory>
    {
        private IFundingMicroService FundingMicroService { get; }
        private IFundingAdminService FundingAdminService { get; }

        public FunderController(
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
                var model = await GetFunderAsync(id.Value);

                return View("Detail", model);
            }
            else
            {
                this.SetSortDefault(ModelFactory.GetDefaultSort());
                this.SetFilterDefault(ModelFactory.CreatePagingStateFilter(null, null, ModelFactory.DefaultRecordCount));

                var model = await GetFunderListAsync();

                return View("List", model);
            }
        }

        public Task<ActionResult> ListSubmit()
        {
            return Index(null);
        }

        [HttpPost]
        public async Task<ActionResult> ListSubmit(FunderList model)
        {
            this.SetPagingState(ModelFactory.CreatePagingStateFilter(model.Filter));

            model = await GetFunderListAsync();

            return View("List", model);
        }

        private async Task<Funder> GetFunderAsync(long funderId)
        {
            var aFunder = await FundingAdminService.GetFunderAsync(funderId);

            var model = ModelFactory.CreateFunder(aFunder);

            return model;
        }

        private async Task<FunderList> GetFunderListAsync()
        {
            var pagingState = this.GetPagingState(0);

            var (hasFundsAvailable, hasFundsRefundable, recordCount) = ModelFactory.ParsePagingStateFilter(pagingState.Filter);

            var mFunders = await FundingMicroService.GetFunderSummariesAsync(null, hasFundsAvailable, hasFundsRefundable, recordCount);

            var model = ModelFactory.CreateFunderList(mFunders.Summaries, pagingState);

            return model;
        }
    }
}