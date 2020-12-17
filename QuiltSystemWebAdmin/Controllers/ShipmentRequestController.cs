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
using RichTodd.QuiltSystem.WebAdmin.Models.ShipmentRequest;

namespace RichTodd.QuiltSystem.WebAdmin.Controllers
{
    [Authorize(Policy = ApplicationPolicies.AllowViewFulfillment)]
    public class ShipmentRequestController : ApplicationController<ShipmentRequestModelFactory>
    {
        private IShipmentAdminService ShipmentAdminService { get; }

        public ShipmentRequestController(
            IApplicationLocale applicationLocale,
            IDomainMicroService domainMicroService,
            IShipmentAdminService shipmentAdminService)
            : base(
                  applicationLocale,
                  domainMicroService)
        {
            ShipmentAdminService = shipmentAdminService ?? throw new ArgumentNullException(nameof(shipmentAdminService));
        }

        public async Task<ActionResult> Index(long? id)
        {
            if (id.HasValue)
            {
                var model = await GetShipmentRequestAsync(id.Value);

                return View("Detail", model);
            }
            else
            {
                this.SetSortDefault(ModelFactory.GetDefaultSort());
                this.SetFilterDefault(ModelFactory.CreatePagingStateFilter(MFulfillment_ShipmentRequestStatus.MetaAll, ModelFactory.DefaultRecordCount));

                var model = await GetShipmentRequestListAsync();

                return View("List", model);
            }
        }

        public async Task<ActionResult> ListSubmit()
        {
            return await Index(null);
        }

        [HttpPost]
        public async Task<ActionResult> ListSubmit(ShipmentRequestList model)
        {
            this.SetPagingState(ModelFactory.CreatePagingStateFilter(model.Filter));

            model = await GetShipmentRequestListAsync();

            return View("List", model);
        }

        public async Task<ActionResult> DetailSubmit(int? id)
        {
            return await Index(id);
        }

        [HttpPost]
        public async Task<ActionResult> DetailSubmit(ShipmentRequestDetailSubmit model)
        {
            //ModelState.AddModelError<ShipmentRequestDetailModel, int>(m => m.ShipmentRequest.ShipmentRequestId, "Error");
            var actionData = this.GetActionData();
            switch (actionData?.ActionName)
            {
                case Actions.Process:
                    //await ProcessAccountReceiptJob.CreateJobRequest(model.AccountReceipt.AccountReceiptId).EnqueueAsync();
                    break;
            }

            return await Index(model.ShipmentRequestId);
        }

        public async Task<ActionResult> Print(int id)
        {
            var model = await GetShipmentRequestAsync(id);

            return View("Print", model);
        }

        #region Methods

        private async Task<ShipmentRequest> GetShipmentRequestAsync(long shipmentRequestId)
        {
            var aShipmentRequest = await ShipmentAdminService.GetShipmentRequestAsync(shipmentRequestId);

            var model = ModelFactory.CreateShipmentRequest(aShipmentRequest);

            return model;
        }

        private async Task<ShipmentRequestList> GetShipmentRequestListAsync()
        {
            var pagingState = this.GetPagingState(0);

            var (shipmentRequestStatus, recordCount) = ModelFactory.ParsePagingStateFilter(pagingState.Filter);

            var aShipmentRequests = await ShipmentAdminService.GetShipmentRequestSummariesAsync(shipmentRequestStatus, recordCount);

            var model = ModelFactory.CreateShipmentRequestList(aShipmentRequests, pagingState);

            return model;
        }

        #endregion Methods
    }
}