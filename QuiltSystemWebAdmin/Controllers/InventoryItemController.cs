//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using RichTodd.QuiltSystem.Service.Admin.Abstractions;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Web;
using RichTodd.QuiltSystem.Web.Extensions;
using RichTodd.QuiltSystem.Web.Paging;
using RichTodd.QuiltSystem.WebAdmin.Models.InventoryItem;

namespace RichTodd.QuiltSystem.WebAdmin.Controllers
{
    public class InventoryItemController : ApplicationController<InventoryItemModelFactory>
    {
        private IInventoryAdminService InventoryAdminService { get; }

        public InventoryItemController(
            IApplicationLocale applicationLocale,
            IDomainMicroService domainMicroService,
            IInventoryAdminService inventoryAdminService)
            : base(
                  applicationLocale,
                  domainMicroService)
        {
            InventoryAdminService = inventoryAdminService ?? throw new ArgumentNullException(nameof(inventoryAdminService));
        }

        public async Task<ActionResult> Index(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var model = await GetInventoryItemDetailModel(id);

                return View("Detail", model);
            }
            else
            {
                var model = await GetInventoryItemListModel(this.GetPagingState(0));

                return View("List", model);
            }
        }

        public ActionResult ListSubmit()
        {
            return FallbackRedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> ListSubmit(InventoryItemListModel model)
        {
            var actionData = this.GetActionData();
            switch (actionData?.ActionName)
            {
                case Actions.Refresh:
                    {
                        this.SetPagingState(model.Filter);

                        model = await GetInventoryItemListModel(this.GetPagingState(0));

                        return View("List", model);
                    }
            }

            return FallbackRedirectToAction("Index");
        }

        private async Task<InventoryItemDetailModel> GetInventoryItemDetailModel(string sku)
        {
            var svcInventoryItem = await InventoryAdminService.GetItemAsync(sku);

            var model = ModelFactory.CreateInventoryItemDetailModel(svcInventoryItem);

            return model;
        }

        private async Task<InventoryItemListModel> GetInventoryItemListModel(PagingState pagingState)
        {
            var svcInventoryItems = await InventoryAdminService.GetItemsAsync();

            //if (!string.IsNullOrEmpty(pagingState.Search))
            //{
            //    svcInventoryItems = svcInventoryItems.Where(r => r.Sku.Contains(pagingState.Search)).ToList();
            //}

            if (pagingState.Filter == InventoryItemListModel.FILTER_LOW)
            {
                svcInventoryItems = svcInventoryItems.Where(r => r.Quantity < 10).ToList();
            }
            else if (pagingState.Filter == InventoryItemListModel.FILTER_OUT)
            {
                svcInventoryItems = svcInventoryItems.Where(r => r.Quantity == 0).ToList();
            }

            var model = ModelFactory.CreateInventoryItemListModel(svcInventoryItems, pagingState);

            return model;
        }

    }
}