//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using RichTodd.QuiltSystem.Service.Ajax.Abstractions;
using RichTodd.QuiltSystem.Service.Ajax.Abstractions.Data;

namespace RichTodd.QuiltSystem.Web.Controllers
{
    [Authorize]
    public class ApiController : ApplicationApiController
    {
        private readonly IDesignAjaxService m_designWebService;

        public ApiController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<ApiController> applicationLogger,
            IDesignAjaxService designWebService)
            : base(userManager, signInManager, applicationLogger)
        {
            m_designWebService = designWebService ?? throw new ArgumentNullException(nameof(designWebService));
        }

        [Route("api/blocks")]
        public async Task<XDesign_Block[]> GetBlocks(int size)
        {
            var result = await m_designWebService.GetBlocksAsync(size);
            return result;
        }

        [Route("api/layouts")]
        public async Task<XDesign_Layout[]> GetLayouts(int rowCount, int columnCount, int size)
        {
            var result = await m_designWebService.GetLayoutsAsync(rowCount, columnCount, size);
            return result;
        }

        [Route("api/fabricStyles")]
        public async Task<XDesign_FabricStyleCatalog> GetFabricStyles()
        {
            var result = await m_designWebService.GetFabricStyleCatalogAsync();
            return result;
        }

        [Route("api/designs/{designId?}")]
        public async Task<XDesign_Design> GetDesign(string designId = null)
        {
            var userId = HttpContext.GetUserId();
            var result = await m_designWebService.GetDesignAsync(userId, designId);
            return result;
        }

        [HttpPost]
        [Route("api/preview")]
        public async Task<XDesign_DesignInfo> GetDesignInfo([FromBody] XDesign_Design designData, int designSize, int layoutSize, int blockSize)
        {
            var result = await m_designWebService.GetDesignInfo(designData, designSize, layoutSize, blockSize);
            return result;
        }

        [HttpPost]
        [Route("api/save")]
        public async Task<Guid> Save([FromBody] XDesign_Design designData)
        {
            var userId = HttpContext.GetUserId();
            var result = await m_designWebService.SaveDesignAsync(userId, designData);
            return result;
        }
    }
}