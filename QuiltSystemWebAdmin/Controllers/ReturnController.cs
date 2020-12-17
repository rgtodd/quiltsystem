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
using RichTodd.QuiltSystem.WebAdmin.Models.Return;

namespace RichTodd.QuiltSystem.WebAdmin.Controllers
{
    [Authorize(Policy = ApplicationPolicies.AllowViewFulfillment)]
    public class ReturnController : ApplicationController<ReturnModelFactory>
    {
        private IReturnAdminService ReturnAdminService { get; }

        public ReturnController(
            IApplicationLocale applicationLocale,
            IDomainMicroService domainMicroService,
            IReturnAdminService returnAdminService)
            : base(
                  applicationLocale,
                  domainMicroService)
        {
            ReturnAdminService = returnAdminService ?? throw new ArgumentNullException(nameof(returnAdminService));
        }

        public async Task<ActionResult> Index(long? id)
        {
            if (id.HasValue)
            {
                var model = await GetReturnAsync(id.Value);

                return View("Detail", model);
            }
            else
            {
                this.SetSortDefault(ModelFactory.GetDefaultSort());
                this.SetFilterDefault(ModelFactory.CreatePagingStateFilter(MFulfillment_ReturnStatus.MetaActive, ModelFactory.DefaultRecordCount));

                var model = await GetReturnListAsync();

                return View("List", model);
            }
        }

        public async Task<ActionResult> ListSubmit()
        {
            return await Index(null);
        }

        [HttpPost]
        public async Task<ActionResult> ListSubmit(ReturnList model)
        {
            this.SetPagingState(ModelFactory.CreatePagingStateFilter(model.Filter));

            model = await GetReturnListAsync();

            return View("List", model);
        }

        public async Task<ActionResult> DetailSubmit(int? id)
        {
            return await Index(id);
        }

        [HttpPost]
        public async Task<ActionResult> DetailSubmit(ReturnDetailSubmit model)
        {
            var actionData = this.GetActionData();
            switch (actionData?.ActionName)
            {
                case Actions.Update:
                    return RedirectToAction("Edit", new { returnId = model.ReturnId });

                case Actions.Post:
                    await ReturnAdminService.PostReturnAsync(model.ReturnId);
                    break;

                case Actions.Process:
                    await ReturnAdminService.ProcessReturnAsync(model.ReturnId);
                    break;

                case Actions.Cancel:
                    await ReturnAdminService.CancelReturnAsync(model.ReturnId);
                    break;
            }

            return await Index(model.ReturnId);
        }

        public async Task<ActionResult> Edit(int? returnRequestId, int? returnId)
        {
            EditReturn model;

            if (returnId.HasValue)
            {
                var aReturn = await ReturnAdminService.GetReturnAsync(returnId.Value);
                var mReturnRequest = aReturn.MReturnRequsts.First();
                var mFulfillable = aReturn.MFulfillables.First();

                model = ModelFactory.CreateEditReturn(mFulfillable, mReturnRequest, aReturn.MReturn);
            }
            else if (returnRequestId.HasValue)
            {
                var aReturnRequest = await ReturnAdminService.GetReturnRequestAsync(returnRequestId.Value);
                var mFulfillable = aReturnRequest.MFulfillables.First();

                model = ModelFactory.CreateEditReturn(mFulfillable, aReturnRequest.MReturnRequest, null);
            }
            else
            {
                throw new InvalidOperationException("Arguments not specified.");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(EditReturn model)
        {
            if (ModelState.IsValid)
            {
                var totalQuantity = model.ReturnItems.Sum(r => r.Quantity);
                if (totalQuantity == 0)
                {
                    ModelState.AddModelError(string.Empty, "Quantity must be specified for each least one item.");
                }
            }

            if (!ModelState.IsValid)
            {
                foreach (var itemModel in model.ReturnItems)
                {
                    itemModel.Quantities = ModelFactory.GetQuantitySelectList(itemModel.MaxQuantity);
                }

                return View(model);
            }

            var returnId = model.ReturnId;

            var actionData = this.GetActionData();
            switch (actionData?.ActionName)
            {
                case Actions.Save:
                    returnId = await SaveReturn(model);
                    break;
            }

            return RedirectToAction("Index", "Return", new { id = returnId });
        }

        #region Methods

        private async Task<Return> GetReturnAsync(long returnId)
        {
            var svcReturn = await ReturnAdminService.GetReturnAsync(returnId);

            var model = ModelFactory.CreateReturnDetailModel(svcReturn);

            return model;
        }

        private async Task<ReturnList> GetReturnListAsync()
        {
            var pagingState = this.GetPagingState(0);

            var (returnStatus, recordCount) = ModelFactory.ParsePagingStateFilter(pagingState.Filter);

            var svcReturns = await ReturnAdminService.GetReturnSummariesAsync(returnStatus, recordCount);

            var model = ModelFactory.CreateReturnListModel(svcReturns, pagingState);

            return model;
        }

        private async Task<long> SaveReturn(EditReturn model)
        {
            if (model.ReturnId == null)
            {
                var mCreateReturnItems = new List<MFulfillment_CreateReturnItem>();
                foreach (var item in model.ReturnItems)
                {
                    mCreateReturnItems.Add(new MFulfillment_CreateReturnItem()
                    {
                        ReturnRequestItemId = item.ReturnRequestItemId,
                        Quantity = item.Quantity
                    });
                }

                var mCreateReturn = new MFulfillment_CreateReturn()
                {
                    CreateDateTimeUtc = Locale.GetUtcFromLocalTime(model.ReturnDate.Value),
                    CreateReturnItems = mCreateReturnItems
                };

                var aCreateReturn = new AReturn_CreateReturn()
                {
                    MCreateReturn = mCreateReturn
                };

                var returnId = await ReturnAdminService.CreateReturnAsync(aCreateReturn);

                return returnId;
            }
            else
            {
                var mUpdateReturnItems = new List<MFulfillment_UpdateReturnItem>();
                foreach (var item in model.ReturnItems)
                {
                    mUpdateReturnItems.Add(new MFulfillment_UpdateReturnItem()
                    {
                        ReturnItemId = item.ReturnItemId.Value,
                        Quantity = item.Quantity
                    });
                }

                var mUpdateReturn = new MFulfillment_UpdateReturn()
                {
                    CreateDateTimeUtc = Locale.GetUtcFromLocalTime(model.ReturnDate.Value),
                    UpdateReturnItems = mUpdateReturnItems
                };

                var aUpdateReturn = new AReturn_UpdateReturn()
                {
                    MUpdateReturn = mUpdateReturn
                };

                await ReturnAdminService.UpdateReturnAsync(aUpdateReturn);

                return model.ReturnId.Value;
            }
        }

        #endregion Methods
    }
}