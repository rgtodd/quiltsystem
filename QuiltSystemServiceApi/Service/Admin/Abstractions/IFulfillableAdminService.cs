//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Threading.Tasks;

using RichTodd.QuiltSystem.Service.Admin.Abstractions.Data;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Admin.Abstractions
{
    public interface IFulfillableAdminService
    {
        Task<AFulfillable_Fulfillable> GetFulfillableAsync(long fulfillableId);

        Task<AFulfillable_FulfillableSummaryList> GetFulfillableSummariesAsync(MFulfillment_FulfillableStatus fulfillableStatus, int? recordCount);
    }
}
