//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using RichTodd.QuiltSystem.Security;
using RichTodd.QuiltSystem.Service.Admin.Abstractions;
using RichTodd.QuiltSystem.Service.Admin.Abstractions.Data;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;
using RichTodd.QuiltSystem.Web;
using RichTodd.QuiltSystem.Web.Extensions;
using RichTodd.QuiltSystem.WebAdmin.Models.ReturnRequest;

namespace RichTodd.QuiltSystem.WebAdmin.Controllers
{
    [Authorize(Policy = ApplicationPolicies.AllowViewFulfillment)]
    public class ReturnRequestController : ApplicationController<ReturnRequestModelFactory>
    {
        private IFulfillableAdminService FulfillableAdminService { get; }
        private IReturnAdminService ReturnAdminService { get; }

        public ReturnRequestController(
            IApplicationLocale applicationLocale,
            IDomainMicroService domainMicroService,
            IFulfillableAdminService fulfillableAdminService,
            IReturnAdminService returnAdminService)
            : base(
                  applicationLocale,
                  domainMicroService)
        {
            FulfillableAdminService = fulfillableAdminService ?? throw new ArgumentNullException(nameof(fulfillableAdminService));
            ReturnAdminService = returnAdminService ?? throw new ArgumentNullException(nameof(returnAdminService));
        }

        public async Task<ActionResult> Index(long? id)
        {
            if (id.HasValue)
            {
                var model = await GetReturnRequestAsync(id.Value);

                return View("Detail", model);
            }
            else
            {
                this.SetSortDefault(ModelFactory.GetDefaultSort());
                this.SetFilterDefault(ModelFactory.CreatePagingStateFilter(MFulfillment_ReturnRequestStatus.MetaActive, ModelFactory.DefaultRecordCount));

                var model = await GetReturnRequestListAsync();

                return View("List", model);
            }
        }

        public async Task<ActionResult> ListSubmit()
        {
            return await Index(null);
        }

        [HttpPost]
        public async Task<ActionResult> ListSubmit(ReturnRequestList model)
        {
            this.SetPagingState(ModelFactory.CreatePagingStateFilter(model.Filter));

            model = await GetReturnRequestListAsync();

            return View("List", model);
        }

        public async Task<ActionResult> DetailSubmit(int? id)
        {
            return await Index(id);
        }

        [HttpPost]
        public async Task<ActionResult> DetailSubmit(ReturnRequestDetailSubmit model)
        {
            var actionData = this.GetActionData();
            switch (actionData?.ActionName)
            {
                case Actions.Process:
                    //await ProcessAccountReceiptJob.CreateJobRequest(model.AccountReceipt.AccountReceiptId).EnqueueAsync();
                    break;
            }

            return await Index(model.ReturnRequestId);
        }

        public async Task<ActionResult> Edit(long? fulfillableId, int? returnRequestId)
        {
            EditReturnRequest model;

            var aReturnRequestReasons = await ReturnAdminService.GetReturnRequestReasonsAsync();

            if (returnRequestId.HasValue)
            {
                var aReturnRequest = await ReturnAdminService.GetReturnRequestAsync(returnRequestId.Value);
                var mFulfillable = aReturnRequest.MFulfillables.First();

                model = ModelFactory.CreateEditReturnRequest(mFulfillable, aReturnRequestReasons.MReturnRequestReasons, aReturnRequest.MReturnRequest);
            }
            else if (fulfillableId.HasValue)
            {
                var aFulfillable = await FulfillableAdminService.GetFulfillableAsync(fulfillableId.Value);
                model = ModelFactory.CreateEditReturnRequest(aFulfillable.MFulfillable, aReturnRequestReasons.MReturnRequestReasons, null);
            }
            else
            {
                throw new InvalidOperationException("Arguments not specified.");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(EditReturnRequest model)
        {
            if (ModelState.IsValid)
            {
                var totalQuantity = model.ReturnRequestItems.Sum(r => r.Quantity);
                if (totalQuantity == 0)
                {
                    ModelState.AddModelError(string.Empty, "Quantity must be specified for each least one item.");
                }
            }

            if (!ModelState.IsValid)
            {
                var aReturnRequestReasons = await ReturnAdminService.GetReturnRequestReasonsAsync();

                model.ReturnRequestTypes = ModelFactory.GetReturnRequestTypeSelectList(model.ReturnRequestId == null);
                model.ReturnRequestReasons = ModelFactory.GetReturnRequestReasonSelectList(aReturnRequestReasons.MReturnRequestReasons, model.ReturnRequestId == null);
                foreach (var itemModel in model.ReturnRequestItems)
                {
                    itemModel.Quantities = ModelFactory.GetQuantitySelectList(itemModel.MaxQuantity);
                }

                return View(model);
            }

            var returnRequestId = model.ReturnRequestId;

            var actionData = this.GetActionData();
            switch (actionData?.ActionName)
            {
                case Actions.Save:
                    returnRequestId = await SaveReturnRequest(model);
                    break;
            }

            return RedirectToAction("Index", "ReturnRequest", new { id = returnRequestId });
        }

        #region Methods

        private async Task<ReturnRequest> GetReturnRequestAsync(long returnRequestId)
        {
            var svcReturnRequest = await ReturnAdminService.GetReturnRequestAsync(returnRequestId);

            var model = ModelFactory.CreateReturnRequest(svcReturnRequest);

            return model;
        }

        private async Task<ReturnRequestList> GetReturnRequestListAsync()
        {
            var pagingState = this.GetPagingState(0);

            var (returnRequestStatus, recordCount) = ModelFactory.ParsePagingStateFilter(pagingState.Filter);

            var aReturnRequests = await ReturnAdminService.GetReturnRequestSummariesAsync(returnRequestStatus, recordCount);

            var model = ModelFactory.CreateReturnRequestList(aReturnRequests, pagingState);

            return model;
        }

        private async Task<long> SaveReturnRequest(EditReturnRequest model)
        {
            if (model.ReturnRequestId == null)
            {
                var mCreateReturnRequestItems = new List<MFulfillment_CreateReturnRequestItem>();
                foreach (var item in model.ReturnRequestItems)
                {
                    mCreateReturnRequestItems.Add(new MFulfillment_CreateReturnRequestItem()
                    {
                        FulfillableItemId = item.FulfillableItemId,
                        Quantity = item.Quantity
                    });
                }

                var mCreateReturnRequest = new MFulfillment_CreateReturnRequest()
                {
                    ReturnRequestType = Enum.Parse<MFulfillment_ReturnRequestTypes>(model.ReturnRequestType),
                    ReturnRequestReasonCode = model.ReturnRequestReason,
                    CreateReturnRequestItems = mCreateReturnRequestItems
                };

                var aCreateReturnRequest = new AReturn_CreateReturnRequest()
                {
                    MCreateReturnRequest = mCreateReturnRequest
                };

                var returnRequestId = await ReturnAdminService.CreateReturnRequestAsync(aCreateReturnRequest);

                return returnRequestId;
            }
            else
            {
                var mUpdateReturnRequestItems = new List<MFulfillment_UpdateReturnRequestItem>();
                foreach (var item in model.ReturnRequestItems)
                {
                    mUpdateReturnRequestItems.Add(new MFulfillment_UpdateReturnRequestItem()
                    {
                        ReturnRequestItemId = item.ReturnRequestItemId.Value,
                        Quantity = item.Quantity
                    });
                }

                var mUpdateReturnRequest = new MFulfillment_UpdateReturnRequest()
                {
                    ReturnRequestId = model.ReturnRequestId.Value,
                    ReturnRequestType = Enum.Parse<MFulfillment_ReturnRequestTypes>(model.ReturnRequestType),
                    ReturnRequestReasonCode = model.ReturnRequestReason,
                    UpdateReturnRequestItems = mUpdateReturnRequestItems
                };

                var aUpdateReturnRequest = new AReturn_UpdateReturnRequest()
                {
                    MUpdateReturnRequest = mUpdateReturnRequest
                };

                await ReturnAdminService.UpdateReturnRequestAsync(aUpdateReturnRequest);

                return model.ReturnRequestId.Value;
            }
        }

        #endregion Methods
    }
}