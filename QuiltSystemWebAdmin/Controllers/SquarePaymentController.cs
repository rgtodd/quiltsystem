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
using RichTodd.QuiltSystem.WebAdmin.Models.SquarePayment;

namespace RichTodd.QuiltSystem.WebAdmin.Controllers
{
    [Authorize(Policy = ApplicationPolicies.AllowViewFinancial)]
    public class SquarePaymentController : ApplicationController<SquarePaymentModelFactory>
    {
        private ISquareAdminService SquareAdminService { get; }

        public SquarePaymentController(
            IApplicationLocale applicationLocale,
            IDomainMicroService domainMicroService,
            ISquareAdminService squareAdminService)
            : base(
                  applicationLocale,
                  domainMicroService)
        {
            SquareAdminService = squareAdminService ?? throw new ArgumentNullException(nameof(squareAdminService));
        }

        public async Task<ActionResult> Index(long? id, long? refundId)
        {
            if (id.HasValue)
            {
                var model = await GetSquarePaymentAsync(id.Value);

                return View("Detail", model);
            }
            else if (refundId.HasValue)
            {
                var model = await GetSquarePaymentByRefundAsync(refundId.Value);

                return View("Detail", model);
            }
            else
            {
                this.SetSortDefault(ModelFactory.GetDefaultSort());
                this.SetFilterDefault(ModelFactory.CreatePagingStateFilter(null, ModelFactory.DefaultRecordCount));

                var model = await GetSquarePaymentListAsync();

                return View("List", model);
            }
        }

        public async Task<ActionResult> ListSubmit()
        {
            return await Index(null, null);
        }

        [HttpPost]
        public async Task<ActionResult> ListSubmit(SquarePaymentList model)
        {
            this.SetPagingState(ModelFactory.CreatePagingStateFilter(model.Filter));

            model = await GetSquarePaymentListAsync();

            return View("List", model);
        }

        private async Task<SquarePayment> GetSquarePaymentAsync(long squarePaymentId)
        {
            var aSquarePayment = await SquareAdminService.GetPaymentAsync(squarePaymentId);

            var model = ModelFactory.CreateSquarePayment(aSquarePayment);

            return model;
        }

        private async Task<SquarePayment> GetSquarePaymentByRefundAsync(long squareRefundId)
        {
            var aSquarePayment = await SquareAdminService.GetPaymentByRefundAsync(squareRefundId);

            var model = ModelFactory.CreateSquarePayment(aSquarePayment);

            return model;
        }

        private async Task<SquarePaymentList> GetSquarePaymentListAsync()
        {
            var pagingState = this.GetPagingState(0);

            var (paymentDate, recordCount) = ModelFactory.ParsePagingStateFilter(pagingState.Filter);

            var paymentDateUtc = paymentDate != null
                ? (DateTime?)Locale.GetUtcFromLocalTime(paymentDate.Value)
                : null;

            var aSquarePayments = await SquareAdminService.GetPaymentSummariesAsync(null, paymentDateUtc, recordCount);

            var model = ModelFactory.CreateSquarePaymentList(aSquarePayments.MSummaries, pagingState);

            return model;
        }
    }
}