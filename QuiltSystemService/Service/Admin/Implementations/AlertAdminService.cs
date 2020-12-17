//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
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
    internal class AlertAdminService : BaseService, IAlertAdminService
    {
        private ICommunicationMicroService CommunicationMicroService { get; }
        private IUserMicroService UserMicroService { get; }

        public AlertAdminService(
            IApplicationRequestServices requestServices,
            ILogger<AlertAdminService> logger,
            ICommunicationMicroService communicationMicroService,
            IUserMicroService userMicroService)
            : base(requestServices, logger)
        {
            CommunicationMicroService = communicationMicroService ?? throw new ArgumentNullException(nameof(communicationMicroService));
            UserMicroService = userMicroService ?? throw new ArgumentNullException(nameof(userMicroService));
        }

        public async Task<AAlert_AlertList> GetAlertsAsync(bool? acknowledged, int recordCount)
        {
            using var log = BeginFunction(nameof(AlertAdminService), nameof(GetAlertsAsync), acknowledged, recordCount);
            try
            {
                await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

                var alerts = new List<AAlert_Alert>();
                var mAlerts = await CommunicationMicroService.GetAlertsAsync(recordCount, acknowledged);
                foreach (var mAlert in mAlerts.Alerts)
                {
                    var mUser = mAlert.ParticipantReference != null && TryParseUserId.FromParticipantReference(mAlert.ParticipantReference, out string userId)
                        ? await UserMicroService.GetUserAsync(userId)
                        : null;

                    var alert = Create.AAlert_Alert(mAlert, mUser);
                    alerts.Add(alert);
                }

                var result = new AAlert_AlertList()
                {
                    Alerts = alerts
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

        public async Task<AAlert_Alert> GetAlertAsync(long alertId)
        {
            using var log = BeginFunction(nameof(AlertAdminService), nameof(GetAlertAsync), alertId);
            try
            {
                await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

                var mAlert = await CommunicationMicroService.GetAlertAsync(alertId).ConfigureAwait(false);

                var mUser = mAlert.ParticipantReference != null && TryParseUserId.FromParticipantReference(mAlert.ParticipantReference, out string userId)
                    ? await UserMicroService.GetUserAsync(userId)
                    : null;

                var result = Create.AAlert_Alert(mAlert, mUser);

                log.Result(result);
                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task AcknowledgeAlertsAsync()
        {
            using var log = BeginFunction(nameof(AlertAdminService), nameof(AcknowledgeAlertsAsync));
            try
            {
                await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

                await CommunicationMicroService.AcknowledgeAlertsAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task AcknowledgeAlertAsync(long alertId)
        {
            using var log = BeginFunction(nameof(AlertAdminService), nameof(AcknowledgeAlertAsync), alertId);
            try
            {
                await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

                await CommunicationMicroService.AcknowledgeAlertAsync(alertId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        private static class Create
        {
            public static AAlert_Alert AAlert_Alert(MCommunication_Alert mAlert, MUser_User mUser)
            {
                return new AAlert_Alert()
                {
                    MAlert = mAlert,
                    MUser = mUser
                };
            }
        }
    }
}