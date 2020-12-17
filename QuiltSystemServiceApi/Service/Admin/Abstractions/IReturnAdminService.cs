//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using RichTodd.QuiltSystem.Service.Admin.Abstractions.Data;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Admin.Abstractions
{
    public interface IReturnAdminService
    {
        // Return Request Methods

        Task<IReadOnlyList<AReturn_ReturnRequestSummary>> GetReturnRequestSummariesAsync(MFulfillment_ReturnRequestStatus returnRequestStatus, int? recordCount);
        Task<AReturn_ReturnRequest> GetReturnRequestAsync(long returnRequestId);
        Task<AReturn_ReturnRequest> GetActiveReturnRequestAsync(Guid orderId);
        Task<AReturn_ReturnRequestReasonList> GetReturnRequestReasonsAsync();

        Task<long> CreateReturnRequestAsync(AReturn_CreateReturnRequest returnRequest);
        Task UpdateReturnRequestAsync(AReturn_UpdateReturnRequest returnRequest);
        Task PostReturnRequestAsync(long returnRequestId);
        Task CancelReturnRequestAsync(long returnRequestId);

        // Return Methods

        Task<IReadOnlyList<AReturn_ReturnSummary>> GetReturnSummariesAsync(MFulfillment_ReturnStatus returnStatus, int? recordCount);
        Task<AReturn_Return> GetReturnAsync(long returnId);
        Task<AReturn_Return> GetActiveReturnAsync(long returnRequestId);

        Task<long> CreateReturnAsync(AReturn_CreateReturn returnData);
        Task UpdateReturnAsync(AReturn_UpdateReturn returnData);
        Task PostReturnAsync(long returnId);
        Task ProcessReturnAsync(long returnId);
        Task CancelReturnAsync(long returnId);
    }
}
