//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using RichTodd.QuiltSystem.Service.Admin.Abstractions;
using RichTodd.QuiltSystem.Service.Admin.Abstractions.Data;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;
using RichTodd.QuiltSystem.Web;
using RichTodd.QuiltSystem.Web.Extensions;
using RichTodd.QuiltSystem.WebAdmin.Models.Shipment;

namespace RichTodd.QuiltSystem.WebAdmin.Controllers
{
    //[Authorize(Policy = ApplicationPolicies.CanViewFulfillment)]
    public class ShipmentController : ApplicationController<ShipmentModelFactory>
    {
        private IShipmentAdminService ShipmentAdminService { get; }

        public ShipmentController(
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
                var model = await GetShipmentAsync(id.Value);

                return View("Detail", model);
            }
            else
            {
                this.SetSortDefault(ModelFactory.GetDefaultSort());
                this.SetFilterDefault(ModelFactory.CreatePagingStateFilter(MFulfillment_ShipmentStatus.MetaAll, ModelFactory.DefaultRecordCount));

                var model = await GetShipmentListAsync();

                return View("List", model);
            }
        }

        public async Task<ActionResult> ListSubmit()
        {
            return await Index(null);
        }

        [HttpPost]
        public async Task<ActionResult> ListSubmit(ShipmentList model)
        {
            //switch (this.GetAction())
            //{
            //}

            this.SetPagingState(ModelFactory.CreatePagingStateFilter(model.Filter));

            model = await GetShipmentListAsync();

            return View("List", model);
        }

        public async Task<ActionResult> DetailSubmit(int? id)
        {
            return await Index(id);
        }

        [HttpPost]
        public async Task<ActionResult> DetailSubmit(ShipmentDetailSubmit model)
        {
            var actionData = this.GetActionData();
            switch (actionData?.ActionName)
            {
                case Actions.Update:
                    return RedirectToAction("Edit", new { shipmentId = model.ShipmentId });

                case Actions.Cancel:
                    await ShipmentAdminService.CancelShipmentAsync(model.ShipmentId);
                    break;

                case Actions.Post:
                    await ShipmentAdminService.PostShipmentAsync(model.ShipmentId);
                    break;

                case Actions.Process:
                    await ShipmentAdminService.ProcessShipmentAsync(model.ShipmentId);
                    break;
            }

            return await Index(model.ShipmentId);
        }

        public async Task<ActionResult> Edit(int? shipmentRequestId, long? shipmentId)
        {
            EditShipment model;

            if (shipmentId.HasValue)
            {
                var aShipment = await ShipmentAdminService.GetShipmentAsync(shipmentId.Value);
                var aShippingVendors = await ShipmentAdminService.GetShippingVendorsAsync();

                model = ModelFactory.CreateEditShipment(aShipment, aShippingVendors);
            }
            else if (shipmentRequestId.HasValue)
            {
                var aShipmentRequest = await ShipmentAdminService.GetShipmentRequestAsync(shipmentRequestId.Value);
                var aShippingVendors = await ShipmentAdminService.GetShippingVendorsAsync();

                model = ModelFactory.CreateEditShipment(aShipmentRequest, aShippingVendors);
            }
            else
            {
                throw new InvalidOperationException("Arguments not specified.");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(EditShipment model)
        {
            if (ModelState.IsValid)
            {
                var totalQuantity = model.ShipmentItems.Sum(r => r.Quantity);
                if (totalQuantity == 0)
                {
                    ModelState.AddModelError(string.Empty, "Quantity must be specified for each least one item.");
                }
            }

            if (!ModelState.IsValid)
            {
                //foreach (var itemModel in model.ReturnItems)
                //{
                //    itemModel.Quantities = ModelFactory.GetQuantitySelectList(itemModel.MaxQuantity);
                //}

                var aShippingVendors = await ShipmentAdminService.GetShippingVendorsAsync();
                model.ShippingVendors = ModelFactory.GetShippingVendorSelectList(aShippingVendors);

                return View(model);
            }

            var shipmentId = model.ShipmentId;

            var actionData = this.GetActionData();
            switch (actionData?.ActionName)
            {
                case Actions.Save:
                    _ = await SaveShipment(model);
                    break;
            }

            return RedirectToAction("Index", "Shipment");
        }

        #region Methods

        private async Task<Shipment> GetShipmentAsync(long shipmentId)
        {
            var aShipment = await ShipmentAdminService.GetShipmentAsync(shipmentId);

            var model = ModelFactory.CreateShipment(aShipment);

            return model;
        }

        private async Task<ShipmentList> GetShipmentListAsync()
        {
            var pagingState = this.GetPagingState(0);

            var (shipmentStatus, recordCount) = ModelFactory.ParsePagingStateFilter(pagingState.Filter);

            var aShipmentSummaries = await ShipmentAdminService.GetShipmentSummariesAsync(shipmentStatus, recordCount);

            var model = ModelFactory.CreateShipmentList(aShipmentSummaries, pagingState);

            return model;
        }

        private async Task<long> SaveShipment(EditShipment model)
        {
            if (model.ShipmentId == null)
            {
                var mCreateShipmentItems = new List<MFulfillment_CreateShipmentItem>();
                foreach (var item in model.ShipmentItems)
                {
                    mCreateShipmentItems.Add(new MFulfillment_CreateShipmentItem()
                    {
                        ShipmentRequestItemId = item.ShipmentRequestItemId,
                        Quantity = item.Quantity
                    });
                }

                var mCreateShipment = new MFulfillment_CreateShipment()
                {
                    ShipmentDateTimeUtc = Locale.GetUtcFromLocalTime(model.ShipmentDate),
                    ShippingVendorId = model.ShippingVendorId,
                    TrackingCode = model.TrackingNumber,
                    CreateShipmentItems = mCreateShipmentItems
                };

                var aCreateShipment = new AShipment_CreateShipment()
                {
                    MCreateShipment = mCreateShipment
                };

                var shipmentId = await ShipmentAdminService.CreateShipmentAsync(aCreateShipment);

                return shipmentId;
            }
            else
            {
                var mUpdateShipmentItems = new List<MFulfillment_UpdateShipmentItem>();
                foreach (var item in model.ShipmentItems)
                {
                    mUpdateShipmentItems.Add(new MFulfillment_UpdateShipmentItem()
                    {
                        ShipmentItemId = item.ShipmentItemId.Value,
                        Quantity = item.Quantity
                    });
                }

                var mUpdateShipment = new MFulfillment_UpdateShipment()
                {
                    ShipmentDateTimeUtc = Locale.GetUtcFromLocalTime(model.ShipmentDate),
                    ShippingVendorId = model.ShippingVendorId,
                    TrackingCode = model.TrackingNumber,
                    UpdateShipmentItems = mUpdateShipmentItems
                };

                var aUpdateShipment = new AShipment_UpdateShipment()
                {
                    MUpdateShipment = mUpdateShipment
                };

                await ShipmentAdminService.UpdateShipmentAsync(aUpdateShipment);

                return model.ShipmentId.Value;
            }
        }

        #endregion Methods
    }
}