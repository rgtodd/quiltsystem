//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;
using RichTodd.QuiltSystem.Service.User.Abstractions;
using RichTodd.QuiltSystem.Web.Extensions;
using RichTodd.QuiltSystem.Web.Feedback;
using RichTodd.QuiltSystem.Web.Models.Kit;
using RichTodd.QuiltSystem.Web.Models.Prototype;
using RichTodd.QuiltSystem.Web.Mvc.Models;
using RichTodd.QuiltSystem.Web.Paging;

namespace RichTodd.QuiltSystem.Web.Controllers
{
    public class KitController : ApplicationController<KitModelFactory>
    {
        private ICartUserService CartUserService { get; }
        private IKitMicroService KitMicroService { get; }
        private IProjectUserService ProjectUserService { get; }

        public KitController(
            IApplicationLocale applicationLocale,
            IDomainMicroService domainMicroService,
            ICartUserService cartUserService,
            IKitMicroService kitMicroService,
            IProjectUserService projectUserService)
            : base(
                  applicationLocale,
                  domainMicroService)
        {
            CartUserService = cartUserService ?? throw new ArgumentNullException(nameof(cartUserService));
            KitMicroService = kitMicroService ?? throw new ArgumentNullException(nameof(kitMicroService));
            ProjectUserService = projectUserService ?? throw new ArgumentNullException(nameof(projectUserService));
        }

        public async Task<ActionResult> Create([Bind(Prefix = "id")] Guid designId)
        {
            var projectId = await ProjectUserService.CreateProjectAsync(GetUserId(), ProjectUserService.ProjectType_Kit, "Kit", designId);

            var kit = await KitMicroService.GetKitDetailAsync(GetUserId(), Guid.Parse(projectId), Constants.ThumbnailSize);

            var model = ModelFactory.CreateKitEditModel(kit);

            return View("Edit", model);
        }

        public async Task<ActionResult> Delete([Bind(Prefix = "id")] Guid kitId)
        {
            var result = await ProjectUserService.DeleteProjectAsync(GetUserId(), kitId);
            if (result)
            {
                AddFeedbackMessage(FeedbackMessageTypes.Informational, "Kit deleted.");
            }
            else
            {
                AddFeedbackMessage(FeedbackMessageTypes.Error, "Kit could not be deleted at this time.");
            }

            return RedirectToAction("Index");
        }

        public Task<ActionResult> EditSubmit([Bind(Prefix = "id")] Guid? kitId)
        {
            return Index(kitId, null);
        }

        [HttpPost]
        public async Task<ActionResult> EditSubmit(KitEditModel model)
        {
            //Kit_UpdateSpecificationResponseData response = null;
            //string errorMessage = null;

            var actionData = this.GetActionData();
            switch (actionData?.ActionName)
            {
                case Actions.Save:
                    {
                        var kitSpecification = CreateKitUpdateSpecification(model);

                        var response = await KitMicroService.UpdateKitSpecificationAsync(GetUserId(), model.Detail.ProjectId.Value, kitSpecification);
                        if (response.ServiceError == null)
                        {
                            return RedirectToAction("Index", new { id = "" });
                        }

                        var kit = model.Detail.ProjectId.HasValue
                            ? await KitMicroService.GetKitDetailPreviewAsync(GetUserId(), model.Detail.ProjectId.Value, Constants.ThumbnailSize, kitSpecification)
                            : await KitMicroService.GetKitDetailPreviewFromDesignAsync(GetUserId(), model.Detail.DesignId.Value, Constants.ThumbnailSize, kitSpecification);

                        var newModel = ModelFactory.CreateKitEditModel(kit);
                        newModel.Specification.CustomBindingWidth = model.Specification.CustomBindingWidth;
                        newModel.Specification.CustomBorderWidth = model.Specification.CustomBorderWidth;
                        newModel.Specification.CustomSizeHeight = model.Specification.CustomSizeHeight;
                        newModel.Specification.CustomSizeWidth = model.Specification.CustomSizeWidth;
                        if (kit.ServiceError != null)
                        {
                            newModel.JsonError = JsonConvert.SerializeObject(ErrorVcModelFactory.CreateErrorVcModel(kit.ServiceError));
                        }

                        model = newModel;
                    }
                    break;

                case Actions.SaveAsync:
                    {
                        var kitSpecification = CreateKitUpdateSpecification(model);

                        if (model.Detail.ProjectId == null)
                        {
                            var createResponse = await ProjectUserService.CreateProjectAsync(GetUserId(), ProjectUserService.ProjectType_Kit, "Kit", model.Detail.DesignId.Value);
                            model.Detail.ProjectId = Guid.Parse(createResponse);
                        }

                        var response = await KitMicroService.UpdateKitSpecificationAsync(GetUserId(), model.Detail.ProjectId.Value, kitSpecification);
                        return response.ServiceError == null
                            ? new EmptyResult()
                            : (ActionResult)Json(ErrorVcModelFactory.CreateErrorVcModel(response.ServiceError));
                    }

                case Actions.Refresh:
                    {
                        var kitSpecification = CreateKitUpdateSpecification(model);

                        var kit = model.Detail.ProjectId.HasValue
                            ? await KitMicroService.GetKitDetailPreviewAsync(GetUserId(), model.Detail.ProjectId.Value, Constants.ThumbnailSize, kitSpecification)
                            : await KitMicroService.GetKitDetailPreviewFromDesignAsync(GetUserId(), model.Detail.DesignId.Value, Constants.ThumbnailSize, kitSpecification);

                        model = ModelFactory.CreateKitEditModel(kit);
                        if (kit.ServiceError != null)
                        {
                            model.JsonError = JsonConvert.SerializeObject(ErrorVcModelFactory.CreateErrorVcModel(kit.ServiceError));
                        }
                    }
                    break;
            }

            ModelState.Clear();

            return View("Edit", model);
        }

        public async Task<ActionResult> Index([Bind(Prefix = "id")] Guid? projectId, Guid? designId)
        {
            if (projectId.HasValue)
            {
                var kit = await KitMicroService.GetKitDetailAsync(GetUserId(), projectId.Value, Constants.ThumbnailSize);

                var model = ModelFactory.CreateKitEditModel(kit);

                return View("Edit", model);
            }
            else if (designId.HasValue)
            {
                var kit = await KitMicroService.GetKitDetailFromDesignAsync(GetUserId(), designId.Value, Constants.ThumbnailSize);

                var model = ModelFactory.CreateKitEditModel(kit);

                return View("Edit", model);
            }
            else
            {
                var model = await GetKitSummaryListModelAsync(this.GetPagingState(0));

                return View("List", model);
            }
        }

        public async Task<ActionResult> ListSubmit()
        {
            return await Index(null, null);
        }

        [HttpPost]
        public async Task<ActionResult> ListSubmit(KitSummaryListModel model)
        {
            var actionData = this.GetActionData();
            switch (actionData?.ActionName)
            {
                case Actions.Undelete:
                    {
                        var result = await ProjectUserService.UndeleteProjectAsync(GetUserId());
                        if (result == null)
                        {
                            AddFeedbackMessage(FeedbackMessageTypes.Error, "Kit could not be undeleted at this time.");
                        }
                        else
                        {
                            AddFeedbackMessage(FeedbackMessageTypes.Informational, "Kit undeleted.");
                        }
                    }
                    break;
            }

            this.SetPagingState(model.Filter);

            var kitSummaries = await GetKitSummaryListModelAsync(this.GetPagingState(0));

            return View("List", kitSummaries);
        }

        public async Task<ActionResult> Order(Guid id)
        {
            var result = await CartUserService.AddProjectAsync(GetUserId(), id, 1);
            if (!result)
            {
                throw new Exception("AddProjectAsync failure.");
            }

            return RedirectToAction("Index", "Cart");
        }

        [HttpPost]
        public async Task<ActionResult> RenameKit(KitRenameModel renameKit)
        {
            var result = await ProjectUserService.RenameProjectAsync(GetUserId(), renameKit.KitId, renameKit.NewKitName);
            if (!result)
            {
                throw new Exception("RenameProjectAsync failure.");
            }

            return RedirectToAction("Index");
        }

        public ActionResult Update(string kitId)
        {
            var model = PrototypeModelFactory.CreatePrototypeItemModel(kitId);

            return View(model);
        }

        private MKit_KitSpecificationUpdate CreateKitUpdateSpecification(KitEditModel model)
        {
            var kitSpecification = new MKit_KitSpecificationUpdate()
            {
                Size = model.Specification.Size,
                CustomWidth = model.Specification.CustomSizeWidth,
                CustomHeight = model.Specification.CustomSizeHeight,

                BorderWidth = model.Specification.BorderWidth,
                CustomBorderWidth = model.Specification.CustomBorderWidth,
                BorderFabricStyle = new MCommon_FabricStyle()
                {
                    Sku = model.Specification.BorderFabricStyle.Sku
                },

                BindingWidth = model.Specification.BindingWidth,
                CustomBindingWidth = model.Specification.CustomBindingWidth,
                BindingFabricStyle = new MCommon_FabricStyle()
                {
                    Sku = model.Specification.BindingFabricStyle.Sku
                },

                HasBacking = model.Specification.HasBacking,
                BackingFabricStyle = new MCommon_FabricStyle()
                {
                    Sku = model.Specification.BackingFabricStyle.Sku
                },

                TrimTriangles = model.Specification.TrimTriangles
            };

            return kitSpecification;
        }

        private async Task<KitSummaryListModel> GetKitSummaryListModelAsync(PagingState pagingState)
        {
            var svcKitSummaries = await ProjectUserService.GetProjectSummariesAsync(GetUserId(), null, null);

            var model = ModelFactory.CreateKitSummaryListModel(svcKitSummaries, pagingState);

            return model;
        }

    }
}