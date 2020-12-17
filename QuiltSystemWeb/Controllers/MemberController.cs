//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Service.User.Abstractions;
using RichTodd.QuiltSystem.Web.Models.Member;

namespace RichTodd.QuiltSystem.Web.Controllers
{
    public class MemberController : ApplicationController<MemberModelFactory>
    {
        private IDesignUserService DesignUserService { get; }
        private IProjectUserService ProjectUserService { get; }

        public MemberController(
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

        public async Task<ActionResult> Index()
        {
            var model = await GetMemberSummaryModel();

            return View(model);
        }

        private async Task<MemberSummaryModel> GetMemberSummaryModel()
        {
            var svcDesignSummaryList = await DesignUserService.GetDesignSummariesAsync(GetUserId(), 0, 5);

            var svcProjectSummaryList = await ProjectUserService.GetProjectSummariesAsync(GetUserId(), 0, 5);

            var model = ModelFactory.CreateMemberSummaryModel(svcDesignSummaryList, svcProjectSummaryList);

            return model;
        }

    }
}