//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using RichTodd.QuiltSystem.Security;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Service.User.Abstractions;
using RichTodd.QuiltSystem.Web.Extensions;
using RichTodd.QuiltSystem.Web.Feedback;
using RichTodd.QuiltSystem.Web.Models.Design;
using RichTodd.QuiltSystem.Web.Paging;

namespace RichTodd.QuiltSystem.Web.Controllers
{
    [Authorize(Policy = ApplicationPolicies.IsEndUser)]
    public class DesignController : ApplicationController<DesignModelFactory>
    {
        private IDesignUserService DesignUserService { get; }
        private IProjectUserService ProjectUserService { get; }

        public DesignController(
            IApplicationLocale applicationLocale,
            IDomainMicroService domainMicroService,
            IDesignUserService designUserService,
            IProjectUserService projectUserService)
            : base(
                  applicationLocale,
                  domainMicroService)
        {
            DesignUserService = designUserService ?? throw new ArgumentNullException(nameof(designUserService));
            ProjectUserService = projectUserService ?? throw new ArgumentNullException(nameof(projectUserService));
        }

        public async Task<ActionResult> Delete([Bind(Prefix = "id")] Guid designId)
        {
            var result = await DesignUserService.DeleteDesignAsync(GetUserId(), designId);
            if (result)
            {
                AddFeedbackMessage(FeedbackMessageTypes.Informational, "Design deleted.");
            }
            else
            {
                AddFeedbackMessage(FeedbackMessageTypes.Error, "Design could not be deleted at this time.");
            }

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Index([Bind(Prefix = "id")] Guid? designId)
        {
            if (designId.HasValue)
            {
                var model = ModelFactory.CreateDesignDetailModel(designId.Value);

                return View("Edit", model);
            }
            else
            {
                var model = await GetDesignSummaryListModel(this.GetPagingState(0));

                return View("List", model);
            }
        }

        public async Task<ActionResult> ListSubmit()
        {
            return await Index(null);
        }

        [HttpPost]
        public async Task<ActionResult> ListSubmit(DesignSummaryListModel model)
        {
            var actionData = this.GetActionData();
            switch (actionData?.ActionName)
            {
                case Actions.New:
                    {
                        return RedirectToAction("Index", new { id = Guid.Empty });
                    }

                case Actions.Delete:
                    {
                        var designId = Guid.Parse(actionData.ActionParameter);
                        var result = await DesignUserService.DeleteDesignAsync(GetUserId(), designId);
                        if (!result)
                        {
                            throw new Exception("DeleteCartItemAsync failure.");
                        }
                    }
                    break;

                case Actions.Create:
                    {
                        var designId = Guid.Parse(actionData.ActionParameter);
                        var kitId = await ProjectUserService.CreateProjectAsync(GetUserId(), ProjectUserService.ProjectType_Kit, "Kit", designId);
                        return RedirectToAction("Index", "Kit", new { id = kitId });
                    }

                case Actions.Undelete:
                    {
                        var result = await DesignUserService.UndeleteDesignAsync(GetUserId());
                        if (string.IsNullOrEmpty(result))
                        {
                            AddFeedbackMessage(FeedbackMessageTypes.Error, "Design could not be undeleted at this time.");
                        }
                        else
                        {
                            AddFeedbackMessage(FeedbackMessageTypes.Informational, "Design undeleted.");
                        }
                    }
                    break;
            }

            this.SetPagingState(model.Filter);

            model = await GetDesignSummaryListModel(this.GetPagingState(0));

            return View("List", model);
        }

        [HttpPost]
        public async Task<ActionResult> RenameDesign(DesignRenameModel renameDesign)
        {
            var result = await DesignUserService.RenameDesignAsync(GetUserId(), renameDesign.DesignId, renameDesign.NewDesignName);
            if (!result)
            {
                throw new Exception("RenameDesignAsync failure.");
            }

            return RedirectToAction("Index");
        }

        private async Task<DesignSummaryListModel> GetDesignSummaryListModel(PagingState pagingState)
        {
            var svcDesignSummaryList = await DesignUserService.GetDesignSummariesAsync(GetUserId(), null, null);

            var model = ModelFactory.CreateDesignSummaryListModel(svcDesignSummaryList, pagingState);

            return model;
        }

    }
}