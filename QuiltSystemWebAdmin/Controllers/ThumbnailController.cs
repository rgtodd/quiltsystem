﻿//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using RichTodd.QuiltSystem.Service.Base;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Service.User.Abstractions;
using RichTodd.QuiltSystem.Web;
using RichTodd.QuiltSystem.WebAdmin.Models.Thumbnail;

namespace RichTodd.QuiltSystem.WebAdmin.Controllers
{
    public class ThumbnailController : ApplicationController<ThumbnailModelFactory>
    {
        private IDesignMicroService DesignMicroService { get; }
        private IDesignUserService DesignUserService { get; }
        private IOrderMicroService OrderMicroService { get; }
        private IProjectUserService ProjectUserService { get; }

        public ThumbnailController(
            IApplicationLocale applicationLocale,
            IDomainMicroService domainMicroService,
            IDesignMicroService designMicroService,
            IDesignUserService designUserService,
            IOrderMicroService orderMicroService,
            IProjectUserService projectUserService)
            : base(
                  applicationLocale,
                  domainMicroService)
        {
            DesignMicroService = designMicroService ?? throw new ArgumentNullException(nameof(designMicroService));
            DesignUserService = designUserService ?? throw new ArgumentNullException(nameof(designUserService));
            OrderMicroService = orderMicroService ?? throw new ArgumentNullException(nameof(orderMicroService));
            ProjectUserService = projectUserService ?? throw new ArgumentNullException(nameof(projectUserService));
        }

        public async Task<ActionResult> Block(string id, string size)
        {
            if (!int.TryParse(size, out var sizeValue))
            {
                sizeValue = Constants.ThumbnailSize;
            }

            var image = await DesignMicroService.GetBlockThumbnailAsync(id, sizeValue);

            return new FileContentResult(image, "image/jpg");
        }

        public async Task<ActionResult> Design(Guid id, string size)
        {
            if (!int.TryParse(size, out var sizeValue))
            {
                sizeValue = Constants.ThumbnailSize;
            }

            var image = await DesignUserService.GetDesignThumbnailAsync(GetUserId(), id, sizeValue);

            return new FileContentResult(image, "image/jpg");
        }

        public async Task<ActionResult> DesignSnapshot(int id, string size)
        {
            if (!int.TryParse(size, out var sizeValue))
            {
                sizeValue = Constants.ThumbnailSize;
            }

            var image = await DesignUserService.GetDesignSnapshotThumbnailAsync(GetUserId(), id, sizeValue);

            return new FileContentResult(image, "image/jpg");
        }

        public async Task<ActionResult> Kit(Guid id, string size)
        {
            if (!int.TryParse(size, out var sizeValue))
            {
                sizeValue = Constants.ThumbnailSize;
            }

            var image = await ProjectUserService.GetProjectThumbnailAsync(GetUserId(), id, sizeValue);

            return new FileContentResult(image, "image/jpg");
        }

        public async Task<ActionResult> KitSnapshot(string id, string size)
        {
            if (!int.TryParse(size, out var sizeValue))
            {
                sizeValue = Constants.ThumbnailSize;
            }

            id = WebUtility.UrlDecode(id);

            var projectSnapshotId = (int)ParseProjectSnapshotId.FromOrderableReference(id);
            var image = await ProjectUserService.GetProjectSnapshotThumbnailAsync(GetUserId(), projectSnapshotId, sizeValue);

            return new FileContentResult(image, "image/jpg");
        }

        public async Task<ActionResult> FulfillableItem(string id, string size)
        {
            if (!int.TryParse(size, out var sizeValue))
            {
                sizeValue = Constants.ThumbnailSize;
            }

            id = WebUtility.UrlDecode(id);

            var orderItemId = ParseOrderItemId.FromFulfillableItemReference(id);
            var mOrderItem = await OrderMicroService.GetOrderItemAsync(orderItemId);
            var projectSnapshotId = (int)ParseProjectSnapshotId.FromOrderableReference(mOrderItem.OrderableReference);
            var image = await ProjectUserService.GetProjectSnapshotThumbnailAsync(GetUserId(), projectSnapshotId, sizeValue);

            return new FileContentResult(image, "image/jpg");
        }
    }
}