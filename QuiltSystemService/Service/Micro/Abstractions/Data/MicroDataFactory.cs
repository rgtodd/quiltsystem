//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

using RichTodd.QuiltSystem.Service.Base;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    internal static class MicroDataFactory
    {
        public static MOrder_AllocateOrderable MOrder_AllocateOrderable(MProject_ProjectSnapshot mProjectSnapshot)
        {
            var components = new List<MOrder_AllocateOrderableComponent>();
            foreach (var mProjectSnapshotComponent in mProjectSnapshot.Components)
            {
                components.Add(new MOrder_AllocateOrderableComponent()
                {
                    OrderableComponentReference = CreateOrderableCompnentReference.FromProjectSnapshotComponentId(mProjectSnapshotComponent.ProjectSnapshotComponentId),
                    Description = TextUtility.EncodeMultilineText(
                        mProjectSnapshotComponent.Sku,
                        mProjectSnapshotComponent.UnitOfMeasureName,
                        $"{mProjectSnapshotComponent.Manufacturer} - {mProjectSnapshotComponent.Collection} - {mProjectSnapshotComponent.Description}"),
                    ConsumableReference = mProjectSnapshotComponent.ConsumableReference,
                    Quantity = mProjectSnapshotComponent.Quantity,
                    UnitPrice = mProjectSnapshotComponent.UnitPrice,
                    TotalPrice = mProjectSnapshotComponent.TotalPrice
                });
            }

            var result = new MOrder_AllocateOrderable()
            {
                OrderableReference = CreateOrderableReference.FromProjectSnapshotId(mProjectSnapshot.ProjectSnapshotId),
                Name = $"{mProjectSnapshot.Name}",
                Price = mProjectSnapshot.Price,
                Components = components
            };

            return result;
        }
    }
}
