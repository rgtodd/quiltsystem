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
using RichTodd.QuiltSystem.WebAdmin.Models.SquareCustomer;

namespace RichTodd.QuiltSystem.WebAdmin.Controllers
{
    [Authorize(Policy = ApplicationPolicies.AllowViewFinancial)]
    public class SquareCustomerController : ApplicationController<SquareCustomerModelFactory>
    {
        private ISquareAdminService SquareAdminService { get; }

        public SquareCustomerController(
            IApplicationLocale applicationLocale,
            IDomainMicroService domainMicroService,
            ISquareAdminService squareAdminService)
            : base(
                  applicationLocale,
                  domainMicroService)
        {
            SquareAdminService = squareAdminService ?? throw new ArgumentNullException(nameof(squareAdminService));
        }

        public async Task<ActionResult> Index(long? id)
        {
            if (id.HasValue)
            {
                var model = await GetSquareCustomerAsync(id.Value);

                return View("Detail", model);
            }
            else
            {
                this.SetSortDefault(ModelFactory.GetDefaultSort());
                this.SetFilterDefault(ModelFactory.CreatePagingStateFilter(ModelFactory.DefaultRecordCount));

                var model = await GetSquareCustomerListAsync();

                return View("List", model);
            }
        }

        public async Task<ActionResult> ListSubmit()
        {
            return await Index(null);
        }

        [HttpPost]
        public async Task<ActionResult> ListSubmit(SquareCustomerList model)
        {
            this.SetPagingState(ModelFactory.CreatePagingStateFilter(model.Filter));

            model = await GetSquareCustomerListAsync();

            return View("List", model);
        }

        private async Task<SquareCustomer> GetSquareCustomerAsync(long squareCustomerId)
        {
            var aSquareCustomer = await SquareAdminService.GetCustomerAsync(squareCustomerId);

            var model = ModelFactory.CreateSquareCustomer(aSquareCustomer);

            return model;
        }

        private async Task<SquareCustomerList> GetSquareCustomerListAsync()
        {
            var pagingState = this.GetPagingState(0);

            var recordCount = ModelFactory.ParsePagingStateFilter(pagingState.Filter);

            var aSquareCustomers = await SquareAdminService.GetCustomerSummariesAsync(null, recordCount);

            var model = ModelFactory.CreateSquareCustomerList(aSquareCustomers.MSummaries, pagingState);

            return model;
        }
    }
}