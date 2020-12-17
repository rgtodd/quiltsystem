//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using RichTodd.QuiltSystem.Service.Base;

namespace RichTodd.QuiltSystem.Test.Service.AdHoc
{
    [TestClass]
    public class SquareTest : BaseTest
    {
        [TestInitialize]
        public void TestInitialize()
        {
            OnTestInitialize(mockEvents: false);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            OnTestCleanup();
        }

        [TestMethod]
        public async Task CreateCustomer()
        {
            var originalSquareCustomerReference = CreateSquareCustomerReference.FromTimestamp(GetUniqueNow());
            var originalSquareCustomerId = await SquareMicroService.AllocateSquareCustomerAsync(originalSquareCustomerReference);

            var squareCustomerId = await SquareMicroService.LookupSquareCustomerIdAsync(originalSquareCustomerReference);
            Assert.AreEqual(originalSquareCustomerId, squareCustomerId);

            var squareCustomer = await SquareMicroService.GetSquareCustomerAsync(originalSquareCustomerId);
            Assert.IsNotNull(squareCustomer);
            Assert.AreEqual(originalSquareCustomerReference, squareCustomer.SquareCustomerReference);
        }

        [TestMethod]
        public async Task CreatePayment()
        {
            var logger = ServiceScope.ServiceProvider.GetService<ILogger<SquareTest>>();

            var squareCustomerReference = CreateSquareCustomerReference.FromTimestamp(GetUniqueNow());
            var squareCustomerId = await SquareMicroService.AllocateSquareCustomerAsync(squareCustomerReference);
            logger.LogInformation($"Square customer ID = {squareCustomerId}");

            var squarePaymentReference = CreateSquarePaymentReference.FromTimestamp(GetUniqueNow());
            var squarePaymentId = await SquareMicroService.AllocateSquarePaymentAsync(squarePaymentReference, squareCustomerId);
            logger.LogInformation($"Square payment ID = {squarePaymentId}");

            var squareWebPaymentTransactionId = await SquareMicroService.CreateSquareWebPaymentRequestAsync(squarePaymentId, 100m, "cnon:card-nonce-ok");
            var paymentResponse = await SquareMicroService.ProcessWebPaymentRequestAsync(squareWebPaymentTransactionId);
            logger.LogInformation($"Square payment response = {paymentResponse}");

            //var squareRefundTransactionId = await SquareMicroService.CreateSquareRefundTransactionAsync(squarePaymentId, 25m);
            //var refundResponse = await SquareMicroService.ProcessSquarePaymentTransactionAsync(squareRefundTransactionId);
            //logger.LogInformation($"Square refund response = {refundResponse}");

            _ = await SquareMicroService.ProcessEventsAsync();
        }
    }
}
