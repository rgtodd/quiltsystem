//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using RichTodd.QuiltSystem.Database.Domain;
using RichTodd.QuiltSystem.Service.Base;

namespace RichTodd.QuiltSystem.Test.Service.Regression
{
    [TestClass]
    public class CommunicationTest : BaseTest
    {
        [TestInitialize]
        public void TestInitialize()
        {
            OnTestInitialize(mockEvents: true);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            OnTestCleanup();
        }

        [TestMethod]
        public async Task CreateAlert()
        {
            const int AlertCount = 3;

            var logger = ServiceScope.ServiceProvider.GetService<ILogger<CommunicationTest>>();

            await CommunicationMicroService.AcknowledgeAlertsAsync();

            var mAlerts = await CommunicationMicroService.GetAlertsAsync(int.MaxValue, true);
            Assert.IsNotNull(mAlerts);
            var existingAlertCount = mAlerts.Alerts.Count;

            mAlerts = await CommunicationMicroService.GetAlertsAsync(int.MaxValue, false);
            Assert.IsNotNull(mAlerts);
            Assert.AreEqual(0, mAlerts.Alerts.Count);

            var alertIds = new List<long>();
            for (int idx = 0; idx < AlertCount; ++idx)
            {
                var participantReference = CreateParticipantReference.FromTimestamp(GetUniqueNow());
                logger.LogInformation($"Participant reference = {participantReference}");

                var participantId = await CommunicationMicroService.AllocateParticipantAsync(participantReference);
                logger.LogInformation($"Participant ID = {participantId}");

                var topicReference = CreateTopicReference.FromTimestamp(GetUniqueNow());
                logger.LogInformation($"Topic reference = {topicReference}");

                var topicFields = new Dictionary<string, string>()
                {
                    {"Key1","Value1" },
                    {"Key2","Value2" }
                };
                var topicId = await CommunicationMicroService.AllocateTopicAsync(topicReference, topicFields);
                logger.LogInformation($"Topic ID = {topicId}");

                var exception = new Exception("Test Exception");

                var alertId = await CommunicationMicroService.CreateAlert(exception, participantId, topicId);
                logger.LogInformation($"Alert ID = {alertId}");
                alertIds.Add(alertId);

                var mAlert = await CommunicationMicroService.GetAlertAsync(alertId);
                Assert.IsNotNull(mAlert);
                Assert.AreEqual(alertId, mAlert.AlertId);
                Assert.AreEqual(participantId, mAlert.ParticipantId);
                Assert.AreEqual(participantReference, mAlert.ParticipantReference);
                Assert.AreEqual(topicId, mAlert.TopicId);
                Assert.AreEqual(topicReference, mAlert.TopicReference);
                Assert.IsNull(mAlert.AcknowledgementDateTimeUtc);
            }

            mAlerts = await CommunicationMicroService.GetAlertsAsync(int.MaxValue, false);
            Assert.IsNotNull(mAlerts);
            Assert.AreEqual(AlertCount, mAlerts.Alerts.Count);
            foreach (var alertId in alertIds)
            {
                var mAlert = mAlerts.Alerts.Where(r => r.AlertId == alertId).Single();
                Assert.IsNotNull(mAlert);
                Assert.AreEqual(alertId, mAlert.AlertId);
                Assert.IsNull(mAlert.AcknowledgementDateTimeUtc);
            }

            await CommunicationMicroService.AcknowledgeAlertAsync(alertIds[0]);

            mAlerts = await CommunicationMicroService.GetAlertsAsync(int.MaxValue, false);
            Assert.IsNotNull(mAlerts);
            Assert.AreEqual(AlertCount - 1, mAlerts.Alerts.Count);
            Assert.IsNull(mAlerts.Alerts.Where(r => r.AlertId == alertIds[0]).FirstOrDefault());
        }

        [TestMethod]
        public async Task CreateMessage()
        {
            var logger = ServiceScope.ServiceProvider.GetService<ILogger<CommunicationTest>>();

            var user = await GetRandomUserAsync();
            logger.LogInformation($"User = {user}");

            var participantReference = CreateParticipantReference.FromTimestamp(GetUniqueNow());
            logger.LogInformation($"Participant reference = {participantReference}");

            var participantId = await CommunicationMicroService.AllocateParticipantAsync(participantReference);
            logger.LogInformation($"Participant ID = {participantId}");

            var topicReference = CreateTopicReference.FromUserId(user.Id);
            logger.LogInformation($"Topic reference = {topicReference}");

            var topicFields = new Dictionary<string, string>()
                {
                    {"Key1","Value1" },
                    {"Key2","Value2" }
                };
            var topicId = await CommunicationMicroService.AllocateTopicAsync(topicReference, topicFields).ConfigureAwait(false);
            logger.LogInformation($"Topic ID = {topicId}");

            var messageId = await CommunicationMicroService.SendMessageToParticipantAsync(participantId, "Test Message", "This is a test message.", null, topicId);
            logger.LogInformation($"Message ID = {messageId}");
        }

        [TestMethod]
        public async Task CreateNotification()
        {
            var logger = ServiceScope.ServiceProvider.GetService<ILogger<CommunicationTest>>();

            var user = await GetRandomUserAsync();
            logger.LogInformation($"User = {user}");

            var participantReference = CreateParticipantReference.FromTimestamp(GetUniqueNow());
            logger.LogInformation($"Participant reference = {participantReference}");

            var participantId = await CommunicationMicroService.AllocateParticipantAsync(participantReference);
            logger.LogInformation($"Participant ID = {participantId}");

            var topicReference = CreateTopicReference.FromUserId(user.Id);
            logger.LogInformation($"Topic reference = {topicReference}");

            var topicFields = new Dictionary<string, string>()
                {
                    {"Key1","Value1" },
                    {"Key2","Value2" }
                };
            var topicId = await CommunicationMicroService.AllocateTopicAsync(topicReference, topicFields).ConfigureAwait(false);
            logger.LogInformation($"Topic ID = {topicId}");

            await CommunicationMicroService.SendNotification(participantId, NotificationTypeCodes.OrderShipped, topicId);
        }
    }
}
