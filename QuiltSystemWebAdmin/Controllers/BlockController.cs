//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;
using RichTodd.QuiltSystem.Web;
using RichTodd.QuiltSystem.Web.Extensions;
using RichTodd.QuiltSystem.WebAdmin.Models.Block;

namespace RichTodd.QuiltSystem.WebAdmin.Controllers
{
    public class BlockController : ApplicationController<BlockModelFactory>
    {
        private IDesignMicroService DesignMicroService { get; }

        public BlockController(
            IApplicationLocale applicationLocale,
            IDomainMicroService domainMicroService,
            IDesignMicroService designMicroService)
            : base(
                  applicationLocale,
                  domainMicroService)
        {
            DesignMicroService = designMicroService ?? throw new ArgumentNullException(nameof(designMicroService));
        }

        public async Task<ActionResult> Index(string id)
        {
            if (id != null)
            {
                throw new NotSupportedException();
                //var model = await GetBlockAsync(id.Value);

                //return View("Detail", model);
            }
            else
            {
                this.SetSortDefault(ModelFactory.GetDefaultSort());
                this.SetFilterDefault(ModelFactory.CreatePagingStateFilter(null, ModelFactory.DefaultRecordCount));

                var model = await GetBlockListAsync();

                return View("List", model);
            }
        }

        public async Task<ActionResult> ListSubmit()
        {
            return await Index(null);
        }

        [HttpPost]
        public async Task<ActionResult> ListSubmit(BlockList model)
        {
            var actionData = this.GetActionData();
            switch (actionData?.ActionName)
            {
                case Actions.Create:
                    await DesignMicroService.GenerateStandardBlocks();
                    break;
            }

            this.SetPagingState(ModelFactory.CreatePagingStateFilter(model.Filter));

            model = await GetBlockListAsync();

            return View("List", model);
        }

        //private async Task<Block> GetBlockAsync(long blockId)
        //{
        //    var aBlock = await FulfillmentAdminService.GetBlockAsync(blockId);

        //    var model = ModelFactory.CreateBlock(aBlock);

        //    return model;
        //}

        private async Task<BlockList> GetBlockListAsync()
        {
            var pagingState = this.GetPagingState(0);

            (var selectedTags, var recordCount) = ModelFactory.ParsePagingStateFilter(pagingState.Filter);

            var mBlockCollection = await DesignMicroService.GetBlockCollectionAsync(250);

            var query = (IEnumerable<MDesign_Block>)mBlockCollection.Blocks;
            if (selectedTags.Count > 0)
            {
                query = query.Where(r => selectedTags.All(t => r.Tags.Contains(t)));
                //query = query.Where(r => selectedTags.All()
            }
            query = query.Take(recordCount);

            var mBlocks = query.ToList();

            var model = ModelFactory.CreateBlockList(mBlocks, mBlockCollection.AllTags, pagingState);

            return model;
        }
    }
}