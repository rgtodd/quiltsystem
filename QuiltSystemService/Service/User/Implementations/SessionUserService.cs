//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using RichTodd.QuiltSystem.Database.Domain;
using RichTodd.QuiltSystem.Service.Base;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Service.User.Abstractions;
using RichTodd.QuiltSystem.Service.User.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.User.Implementations
{
    internal class SessionUserService : BaseService, ISessionUserService
    {
        private static Session_ViewOptionsData m_cachedViewOptions;

        private ICommunicationMicroService CommunicationMicroService { get; }
        private IDomainMicroService DomainMicroService { get; }

        public SessionUserService(
            IApplicationRequestServices requestServices,
            ILogger<SessionUserService> logger,
            ICommunicationMicroService communicationMicroService,
            IDomainMicroService domainMicroService)
            : base(requestServices, logger)
        {
            CommunicationMicroService = communicationMicroService ?? throw new ArgumentNullException(nameof(communicationMicroService));
            DomainMicroService = domainMicroService ?? throw new ArgumentNullException(nameof(domainMicroService));
        }

        public async Task<Session_Data> GetSession(string userId)
        {
            using var log = BeginFunction(nameof(SessionUserService), nameof(GetSession), userId);
            try
            {
                //AssertIsEndUser(userId);
                await Assert(SecurityPolicy.IsAuthorized, userId).ConfigureAwait(false);

                //using var ctx = QuiltContextFactory.Create();

                var participantReference = CreateParticipantReference.FromUserId(userId);
                var participantId = await CommunicationMicroService.AllocateParticipantAsync(participantReference).ConfigureAwait(false);
                var svcSummary = await CommunicationMicroService.GetSummaryAsync(participantId).ConfigureAwait(false);

                var result = new Session_Data
                {
                    HasNotifications = svcSummary.HasNotifications,
                    HasMessages = svcSummary.HasMessages
                };

                //var ordererReference = CreateOrdererReference.FromUserId(userId);
                //var dbOrdererPendingOrder = ctx.OrdererPendingOrders.SingleOrDefault(r => r.Orderer.OrdererReference == ordererReference);

                // HACK: OrderStatus
                /*
                    result.CartItemCount = dbOrdererPendingOrder == null
                        ? 0
                        : dbOrdererPendingOrder.Order.OrderItems.Where(r => r.OrderItemStatusTypeCode == OrderItemStatusTypes.Pending).Count();
                        */

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<Session_ViewOptionsData> GetViewOptions(string userId)
        {
            using var log = BeginFunction(nameof(SessionUserService), nameof(GetViewOptions), userId);
            try
            {
                if (userId != null)
                {
                    await Assert(SecurityPolicy.IsAuthorized, userId).ConfigureAwait(false);
                }

                if (m_cachedViewOptions == null)
                {
                    m_cachedViewOptions = LoadViewOptions();
                }

                var result = m_cachedViewOptions;

                log.Result(result);
                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        private Session_ViewOptionsData LoadViewOptions()
        {
            var ecommerceValue = EcommerceWebsitePropertyValues.Default;
            var publicRegistrationValue = PublicRegistrationWebsitePropertyValues.Default;

            foreach (var svcWebsiteValue in DomainMicroService.GetWebsiteValues())
            {
                if (svcWebsiteValue.PropertyName == WebsitePropertyNames.Ecommerce)
                {
                    ecommerceValue = svcWebsiteValue.PropertyValue;
                }
                else if (svcWebsiteValue.PropertyName == WebsitePropertyNames.PublicRegistration)
                {
                    publicRegistrationValue = svcWebsiteValue.PropertyValue;
                }
            }

            var result = new Session_ViewOptionsData()
            {
                EcommerceEnabled = ecommerceValue == EcommerceWebsitePropertyValues.Enabled,
                PublicRegistrationEnabled = publicRegistrationValue == PublicRegistrationWebsitePropertyValues.Enabled
            };

            return result;
        }
    }
}
