//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using RichTodd.QuiltSystem.Service.Admin.Abstractions;
using RichTodd.QuiltSystem.Service.Admin.Abstractions.Data;
using RichTodd.QuiltSystem.Service.Base;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Admin.Implementations
{
    internal class ReturnAdminService : BaseService, IReturnAdminService
    {
        private ICommunicationMicroService CommunicationMicroService { get; }
        private IEventProcessorMicroService EventProcessorMicroService { get; }
        private IFulfillmentMicroService FulfillmentMicroService { get; }

        public ReturnAdminService(
            IApplicationRequestServices requestServices,
            ILogger<ReturnAdminService> logger,
            ICommunicationMicroService communicationMicroService,
            IEventProcessorMicroService eventProcessorMicroService,
            IFulfillmentMicroService fulfillmentMicroService)
            : base(requestServices, logger)
        {
            CommunicationMicroService = communicationMicroService ?? throw new ArgumentNullException(nameof(communicationMicroService));
            EventProcessorMicroService = eventProcessorMicroService ?? throw new ArgumentNullException(nameof(eventProcessorMicroService));
            FulfillmentMicroService = fulfillmentMicroService ?? throw new ArgumentNullException(nameof(fulfillmentMicroService));
        }

        #region Return Request

        public async Task<IReadOnlyList<AReturn_ReturnRequestSummary>> GetReturnRequestSummariesAsync(MFulfillment_ReturnRequestStatus returnRequestStatus, int? recordCount)
        {
            using var log = BeginFunction(nameof(ReturnAdminService), nameof(GetReturnRequestSummariesAsync), returnRequestStatus, recordCount);
            try
            {
                await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

                var mReturnRequestSummaryList = await FulfillmentMicroService.GetReturnRequestSummariesAsync(returnRequestStatus, recordCount);

                var summaries = Create.AReturn_ReturnRequestSummaries(mReturnRequestSummaryList);

                var result = summaries;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<AReturn_ReturnRequest> GetReturnRequestAsync(long returnRequestId)
        {
            using var log = BeginFunction(nameof(ReturnAdminService), nameof(GetReturnRequestAsync), returnRequestId);
            try
            {
                await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

                var mReturnRequest = await FulfillmentMicroService.GetReturnRequestAsync(returnRequestId);
                var mTransactions = await FulfillmentMicroService.GetReturnRequestTransactionSummariesAsync(returnRequestId, null, null);
                var mEvents = await FulfillmentMicroService.GetReturnRequestEventLogSummariesAsync(returnRequestId, null, null);

                var mFulfillables = new List<MFulfillment_Fulfillable>();
                foreach (var mReturnRequestItem in mReturnRequest.ReturnRequestItems)
                {
                    // Retrieve the associated fulfillable if we haven't already.
                    //
                    var fulfillableId = mReturnRequestItem.FulfillableId;
                    if (!mFulfillables.Any(r => r.FulfillableId == fulfillableId))
                    {
                        var mFulfillable = await FulfillmentMicroService.GetFulfillableAsync(fulfillableId).ConfigureAwait(false);
                        mFulfillables.Add(mFulfillable);
                    }
                }

                var allowEdit = await SecurityPolicy.AllowEditFulfillment();

                var returnRequest = Create.AReturn_ReturnRequest(mReturnRequest, mTransactions, mEvents, mFulfillables, allowEdit);

                var result = returnRequest;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<AReturn_ReturnRequest> GetActiveReturnRequestAsync(Guid orderId)
        {
            using var log = BeginFunction(nameof(ReturnAdminService), nameof(GetActiveReturnRequestAsync), orderId);
            try
            {
                await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<AReturn_ReturnRequestReasonList> GetReturnRequestReasonsAsync()
        {
            using var log = BeginFunction(nameof(ReturnAdminService), nameof(GetReturnRequestReasonsAsync));
            try
            {
                await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

                var mReturnRequestReasons = await FulfillmentMicroService.GetReturnRequestReasonsAsync();

                var result = new AReturn_ReturnRequestReasonList()
                {
                    MReturnRequestReasons = new List<MFulfillment_ReturnRequestReason>(mReturnRequestReasons)
                };

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<long> CreateReturnRequestAsync(AReturn_CreateReturnRequest returnRequest)
        {
            using var log = BeginFunction(nameof(ReturnAdminService), nameof(CreateReturnRequestAsync), returnRequest);
            try
            {
                await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

                var returnRequestId = await FulfillmentMicroService.CreateReturnRequestAsync(returnRequest.MCreateReturnRequest).ConfigureAwait(false);

                var result = returnRequestId;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task UpdateReturnRequestAsync(AReturn_UpdateReturnRequest returnRequest)
        {
            using var log = BeginFunction(nameof(ReturnAdminService), nameof(UpdateReturnRequestAsync), returnRequest);
            try
            {
                await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

                await FulfillmentMicroService.UpdateReturnRequestAsync(returnRequest.MUpdateReturnRequest).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task PostReturnRequestAsync(long returnRequestId)
        {
            using var log = BeginFunction(nameof(ReturnAdminService), nameof(PostReturnRequestAsync), returnRequestId);
            try
            {
                await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

                await FulfillmentMicroService.PostReturnRequestAsync(returnRequestId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task CancelReturnRequestAsync(long returnRequestId)
        {
            using var log = BeginFunction(nameof(ReturnAdminService), nameof(CancelReturnRequestAsync), returnRequestId);
            try
            {
                await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

                await FulfillmentMicroService.CancelReturnRequestAsync(returnRequestId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        #endregion

        #region Return

        public async Task<IReadOnlyList<AReturn_ReturnSummary>> GetReturnSummariesAsync(MFulfillment_ReturnStatus returnStatus, int? recordCount)
        {
            using var log = BeginFunction(nameof(ReturnAdminService), nameof(GetReturnSummariesAsync), returnStatus);
            try
            {
                await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

                var mReturnSummaryList = await FulfillmentMicroService.GetReturnSummariesAsync(returnStatus, recordCount).ConfigureAwait(false);

                var summaries = Create.AReturn_ReturnSummaries(mReturnSummaryList);

                var result = summaries;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<AReturn_Return> GetReturnAsync(long returnId)
        {
            using var log = BeginFunction(nameof(ReturnAdminService), nameof(GetReturnAsync), returnId);
            try
            {
                await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

                var mReturn = await FulfillmentMicroService.GetReturnAsync(returnId).ConfigureAwait(false);
                var mTransactions = await FulfillmentMicroService.GetReturnTransactionSummariesAsync(returnId, null, null);
                var mEvents = await FulfillmentMicroService.GetReturnEventLogSummariesAsync(returnId, null, null);

                var mReturnRequests = new List<MFulfillment_ReturnRequest>();
                foreach (var mReturnItem in mReturn.ReturnItems)
                {
                    // Retrieve the associated fulfillable if we haven't already.
                    //
                    var returnRequestId = mReturnItem.ReturnRequestId;
                    if (!mReturnRequests.Any(r => r.ReturnRequestId == returnRequestId))
                    {
                        var mReturnRequest = await FulfillmentMicroService.GetReturnRequestAsync(returnRequestId).ConfigureAwait(false);
                        mReturnRequests.Add(mReturnRequest);
                    }
                }

                var mFulfillables = new List<MFulfillment_Fulfillable>();
                foreach (var mReturnItem in mReturn.ReturnItems)
                {
                    // Retrieve the associated fulfillable if we haven't already.
                    //
                    var fulfillableId = mReturnItem.FulfillableId;
                    if (!mFulfillables.Any(r => r.FulfillableId == fulfillableId))
                    {
                        var mFulfillable = await FulfillmentMicroService.GetFulfillableAsync(fulfillableId).ConfigureAwait(false);
                        mFulfillables.Add(mFulfillable);
                    }
                }

                var allowEdit = await SecurityPolicy.AllowEditFulfillment();

                var result = Create.AReturn_Return(mReturn, mTransactions, mEvents, mReturnRequests, mFulfillables, allowEdit);

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<AReturn_Return> GetActiveReturnAsync(long returnRequestId)
        {
            using var log = BeginFunction(nameof(ReturnAdminService), nameof(GetActiveReturnAsync), returnRequestId);
            try
            {
                await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<long> CreateReturnAsync(AReturn_CreateReturn returnData)
        {
            using var log = BeginFunction(nameof(ReturnAdminService), nameof(CreateReturnAsync), returnData);
            try
            {
                await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

                var returnId = await FulfillmentMicroService.CreateReturnAsync(returnData.MCreateReturn).ConfigureAwait(false);

                var result = returnId;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task UpdateReturnAsync(AReturn_UpdateReturn returnData)
        {
            using var log = BeginFunction(nameof(ReturnAdminService), nameof(UpdateReturnAsync), returnData);
            try
            {
                await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

                await FulfillmentMicroService.UpdateReturnAsync(returnData.MUpdateReturn).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task PostReturnAsync(long returnId)
        {
            using var log = BeginFunction(nameof(ReturnAdminService), nameof(PostReturnAsync), returnId);
            try
            {
                await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

                await FulfillmentMicroService.PostReturnAsync(returnId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task ProcessReturnAsync(long returnId)
        {
            using var log = BeginFunction(nameof(ReturnAdminService), nameof(ProcessReturnAsync), returnId);
            try
            {
                await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

                await FulfillmentMicroService.ProcessReturnAsync(returnId).ConfigureAwait(false);

                _ = await EventProcessorMicroService.ProcessPendingEvents().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task CancelReturnAsync(long returnId)
        {
            using var log = BeginFunction(nameof(ReturnAdminService), nameof(CancelReturnAsync), returnId);
            try
            {
                await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

                await FulfillmentMicroService.CancelReturnAsync(returnId).ConfigureAwait(false);

                _ = await EventProcessorMicroService.ProcessPendingEvents().ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        #endregion

        private static class Create
        {
            public static AReturn_ReturnRequestSummary AReturn_ReturnRequestSummary(MFulfillment_ReturnRequestSummary mReturnRequest)
            {
                return new AReturn_ReturnRequestSummary()
                {
                    ReturnRequestId = mReturnRequest.ReturnRequestId,
                    ReturnRequestNumber = mReturnRequest.ReturnRequestNumber,
                    FulfillableId = mReturnRequest.FulfillableId,
                    FulfillableName = mReturnRequest.FulfillableName,
                    FulfillableReference = mReturnRequest.FulfillableReference,
                    ReturnRequestStatus = mReturnRequest.ReturnRequestStatus.ToString(),
                    ReturnRequestDateTimeUtc = mReturnRequest.CreateDateTimeUtc,
                    ReturnRequestStatusDateTimeUtc = mReturnRequest.StatusDateTimeUtc
                };
            }

            public static IReadOnlyList<AReturn_ReturnRequestSummary> AReturn_ReturnRequestSummaries(MFulfillment_ReturnRequestSummaryList mReturnRequestSummaryList)
            {
                var result = new List<AReturn_ReturnRequestSummary>();
                foreach (var mReturnRequest in mReturnRequestSummaryList.Summaries)
                {
                    var returnRequest = Create.AReturn_ReturnRequestSummary(mReturnRequest);
                    result.Add(returnRequest);
                }

                return result;
            }

            public static AReturn_ReturnRequest AReturn_ReturnRequest(
                MFulfillment_ReturnRequest mReturnRequest,
                MFulfillment_ReturnRequestTransactionSummaryList mTransactions,
                MFulfillment_ReturnRequestEventLogSummaryList mEvents,
                IList<MFulfillment_Fulfillable> mFulfillables,
                bool allowEdit)
            {
                var result = new AReturn_ReturnRequest()
                {
                    MReturnRequest = mReturnRequest,
                    MTransactions = mTransactions,
                    MEvents = mEvents,
                    MFulfillables = mFulfillables,
                    AllowEdit = allowEdit
                };
                return result;
            }

            public static AReturn_Return AReturn_Return(
                MFulfillment_Return mReturn,
                MFulfillment_ReturnTransactionSummaryList mTransactions,
                MFulfillment_ReturnEventLogSummaryList mEvents,
                IList<MFulfillment_ReturnRequest> mReturnRequests,
                IList<MFulfillment_Fulfillable> mFulfillables,
                bool allowEdit)
            {
                var shipment = new AReturn_Return()
                {
                    MReturn = mReturn,
                    MTransactions = mTransactions,
                    MEvents = mEvents,
                    MReturnRequsts = mReturnRequests,
                    MFulfillables = mFulfillables,
                    AllowEdit = allowEdit
                };

                return shipment;
            }

            public static AReturn_ReturnSummary AReturn_ReturnSummary(MFulfillment_ReturnSummary mReturn)
            {
                var shipment = new AReturn_ReturnSummary()
                {
                    ReturnId = mReturn.ReturnId,
                    ReturnNumber = mReturn.ReturnNumber,
                    FulfillableId = mReturn.FulfillableId,
                    FulfillableName = mReturn.FulfillableName,
                    FulfillableReference = mReturn.FulfillableReference,
                    ReturnStatusName = mReturn.ReturnStatus.ToString(),
                    ReturnDateTimeUtc = mReturn.CreateDateTimeUtc,
                    StatusDateTimeUtc = mReturn.StatusDateTimeUtc
                };

                return shipment;
            }

            public static IReadOnlyList<AReturn_ReturnSummary> AReturn_ReturnSummaries(MFulfillment_ReturnSummaryList mReturns)
            {
                var result = new List<AReturn_ReturnSummary>();
                foreach (var mReturn in mReturns.Summaries)
                {
                    var returnSummary = Create.AReturn_ReturnSummary(mReturn);
                    result.Add(returnSummary);
                }

                return result;
            }
        }
    }
}
