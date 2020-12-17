//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using RichTodd.QuiltSystem.Service.Base;

namespace RichTodd.QuiltSystem.Test.Service.Regression
{
    [TestClass]
    public class FundingTest : BaseTest
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
        public async Task CreateFunder()
        {
            Console.WriteLine("CreateFunder");

            var logger = ServiceScope.ServiceProvider.GetService<ILogger<FundingTest>>();

            var funderReference = CreateFunderReference.FromTimestamp(GetUniqueNow());
            logger.LogInformation($"Funder reference = {funderReference}");

            var funderId = await FundingMicroService.AllocateFunderAsync(funderReference);
            logger.LogInformation($"Funder ID = {funderId}");

            var funder = await FundingMicroService.GetFunderAsync(funderId);
            Assert.IsNotNull(funder);
            logger.LogInformation($"Funder retrieved.  Funder reference = {funder.FunderReference}");
            Assert.AreEqual(funderReference, funder.FunderReference);

            var funders = await FundingMicroService.GetFunderSummariesAsync(null, null, null, null);
            Assert.IsNotNull(funders);
            Assert.IsNotNull(funders.Summaries);
            Assert.IsTrue(funders.Summaries.Any(r => r.FunderReference == funderReference));
        }

        [TestMethod]
        public async Task CreateFundable()
        {
            Console.WriteLine("CreateFundable");

            var logger = ServiceScope.ServiceProvider.GetService<ILogger<FundingTest>>();

            var fundableReference = CreateFundableReference.FromTimestamp(GetUniqueNow());
            logger.LogInformation($"Fundable reference = {fundableReference}");

            var fundableId = await FundingMicroService.AllocateFundableAsync(fundableReference);
            logger.LogInformation($"Fundable ID = {fundableId}");

            var fundable = await FundingMicroService.GetFundableAsync(fundableId);
            Assert.IsNotNull(fundable);
            logger.LogInformation($"Fundable retrieved.  Fundable reference = {fundable.FundableReference}");
            Assert.AreEqual(fundableReference, fundable.FundableReference);

            var fundables = await FundingMicroService.GetFundableSummariesAsync(null, null, null);
            Assert.IsNotNull(fundables);
            Assert.IsNotNull(fundables.Summaries);
            Assert.IsTrue(fundables.Summaries.Any(r => r.FundableReference == fundableReference));
        }

        [TestMethod]
        public async Task TransferPrepayment()
        {
            Console.WriteLine("TransferPrepayment");

            var logger = ServiceScope.ServiceProvider.GetService<ILogger<FundingTest>>();
            var unitOfWork = CreateUnitOfWork.Timestamp(GetUniqueNow());

            var incomeAmount = 100m;
            var salesTaxAmount = 5m;
            var totalAmount = incomeAmount + salesTaxAmount;
            var salesTaxJurisdiction = "XX";

            // Create references.
            //
            var funderReference = CreateFunderReference.FromTimestamp(GetUniqueNow());
            logger.LogInformation($"Funder reference = {funderReference}");

            var fundableReference = CreateFundableReference.FromTimestamp(GetUniqueNow());
            logger.LogInformation($"Fundable reference = {fundableReference}");

            // Create funder and set funds received.
            //
            var funderId = await FundingMicroService.AllocateFunderAsync(funderReference);
            logger.LogInformation($"Funder ID = {funderId}");

            await FundingMicroService.SetFundsReceivedAsync(funderId, fundableReference, totalAmount, unitOfWork.Next());
            logger.LogInformation($"Set {totalAmount:c} funds received.");

            var eventCount = await EventProcessorMicroService.ProcessPendingEvents();
            logger.LogInformation($"{eventCount} events processed.");

            // Create fundable and set funds required.
            //
            var fundableId = await FundingMicroService.AllocateFundableAsync(fundableReference);
            logger.LogInformation($"Fundable ID = {fundableId}");

            await FundingMicroService.SetFundsRequiredAsync(fundableId, incomeAmount, salesTaxAmount, salesTaxJurisdiction, unitOfWork.Next());
            logger.LogInformation($"Set {incomeAmount:c} + {salesTaxAmount:c} funds required.");

            eventCount = await EventProcessorMicroService.ProcessPendingEvents();
            logger.LogInformation($"{eventCount} events processed.");

            // Ensure funds transferred.
            //
            var fundable = await FundingMicroService.GetFundableAsync(fundableId);
            Assert.IsNotNull(fundable);
            logger.LogInformation($"Fundable retrieved.  Fundable reference = {fundable.FundableReference}");
            Assert.AreEqual(fundableReference, fundable.FundableReference);
            Assert.AreEqual(fundable.FundsRequiredTotal, totalAmount);
            Assert.AreEqual(fundable.FundsReceived, totalAmount);
        }

        [TestMethod]
        public async Task TransferPostpayment()
        {
            Console.WriteLine("TransferPostpayment");

            var logger = ServiceScope.ServiceProvider.GetService<ILogger<FundingTest>>();
            var unitOfWork = CreateUnitOfWork.Timestamp(GetUniqueNow());

            var incomeAmount = 100m;
            var salesTaxAmount = 5m;
            var totalAmount = incomeAmount + salesTaxAmount;
            var salesTaxJurisdiction = "XX";

            // Create references.
            //
            var funderReference = CreateFunderReference.FromTimestamp(GetUniqueNow());
            logger.LogInformation($"Funder reference = {funderReference}");

            var fundableReference = CreateFundableReference.FromTimestamp(GetUniqueNow());
            logger.LogInformation($"Fundable reference = {fundableReference}");

            // Create fundable and set funds required.
            //
            var fundableId = await FundingMicroService.AllocateFundableAsync(fundableReference);
            logger.LogInformation($"Fundable ID = {fundableId}");

            await FundingMicroService.SetFundsRequiredAsync(fundableId, incomeAmount, salesTaxAmount, salesTaxJurisdiction, unitOfWork.Next());
            logger.LogInformation($"Set {incomeAmount:c} + {salesTaxAmount:c} funds required.");

            var eventCount = await EventProcessorMicroService.ProcessPendingEvents();
            logger.LogInformation($"{eventCount} events processed.");

            // Create funder and set funds received.
            //
            var funderId = await FundingMicroService.AllocateFunderAsync(funderReference);
            logger.LogInformation($"Funder ID = {funderId}");

            await FundingMicroService.SetFundsReceivedAsync(funderId, fundableReference, totalAmount, unitOfWork.Next());
            logger.LogInformation($"Set {totalAmount:c} funds received.");

            eventCount = await EventProcessorMicroService.ProcessPendingEvents();
            logger.LogInformation($"{eventCount} events processed.");

            // Ensure funds transferred.
            //
            var fundable = await FundingMicroService.GetFundableAsync(fundableId);
            Assert.IsNotNull(fundable);
            logger.LogInformation($"Fundable retrieved.  Fundable reference = {fundable.FundableReference}");
            Assert.AreEqual(fundableReference, fundable.FundableReference);
            Assert.AreEqual(fundable.FundsRequiredTotal, totalAmount);
            Assert.AreEqual(fundable.FundsReceived, totalAmount);
        }

        [TestMethod]
        public async Task TransferRefund()
        {
            Console.WriteLine("TransferRefund");

            var logger = ServiceScope.ServiceProvider.GetService<ILogger<FundingTest>>();
            var unitOfWork = CreateUnitOfWork.Timestamp(GetUniqueNow());

            var incomeAmount = 100m;
            var salesTaxAmount = 5m;
            var totalAmount = incomeAmount + salesTaxAmount;
            var incomeRefundAmount = 10;
            var salesTaxRefundAmount = 1;
            var salesTaxJurisdiction = "XX";

            // Create references.
            //
            var funderReference = CreateFunderReference.FromTimestamp(GetUniqueNow());
            logger.LogInformation($"Funder reference = {funderReference}");

            var fundableReference = CreateFundableReference.FromTimestamp(GetUniqueNow());
            logger.LogInformation($"Fundable reference = {fundableReference}");

            // Create funder and set funds received.
            //
            var funderId = await FundingMicroService.AllocateFunderAsync(funderReference);
            logger.LogInformation($"Funder ID = {funderId}");

            await FundingMicroService.SetFundsReceivedAsync(funderId, fundableReference, totalAmount, unitOfWork.Next());
            logger.LogInformation($"Set {totalAmount:c} funds received.");

            var eventCount = await EventProcessorMicroService.ProcessPendingEvents();
            logger.LogInformation($"{eventCount} events processed.");

            // Create fundable and set funds required.
            //
            var fundableId = await FundingMicroService.AllocateFundableAsync(fundableReference);
            logger.LogInformation($"Fundable ID = {fundableId}");

            await FundingMicroService.SetFundsRequiredAsync(fundableId, incomeAmount, salesTaxAmount, salesTaxJurisdiction, unitOfWork.Next());
            logger.LogInformation($"Set {incomeAmount:c} + {salesTaxAmount:c} funds required.");

            eventCount = await EventProcessorMicroService.ProcessPendingEvents();
            logger.LogInformation($"{eventCount} events processed.");

            // Ensure funds transferred.
            //
            var fundable = await FundingMicroService.GetFundableAsync(fundableId);
            Assert.IsNotNull(fundable);
            logger.LogInformation($"Fundable retrieved.  Fundable reference = {fundable.FundableReference}");
            Assert.AreEqual(fundableReference, fundable.FundableReference);
            Assert.AreEqual(fundable.FundsRequiredTotal, totalAmount);
            Assert.AreEqual(fundable.FundsReceived, totalAmount);

            // Reduce funds required.
            //
            await FundingMicroService.SetFundsRequiredAsync(fundableId, incomeAmount - incomeRefundAmount, salesTaxAmount - salesTaxRefundAmount, salesTaxJurisdiction, unitOfWork.Next());
            logger.LogInformation($"Set {incomeAmount - incomeRefundAmount:c} + {salesTaxAmount - salesTaxRefundAmount:c} funds required.");

            // Ensure funds refunded.
            //
            var funder = await FundingMicroService.GetFunderAsync(funderId);
            Assert.AreEqual(funder.TotalFundsRefundable, incomeRefundAmount + salesTaxRefundAmount);
        }
    }
}
