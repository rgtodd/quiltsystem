//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Database.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;

namespace RichTodd.QuiltSystem.Business.Operation
{
    internal class KansasSalesTaxWebServiceOperation : BusinessOperation
    {

        public KansasSalesTaxWebServiceOperation(
            ILogger applicationLogger,
            IApplicationLocale applicationLocale,
            IQuiltContextFactory quiltContextFactory,
            ICommunicationMicroService communicationMicroService)
            : base(
                  applicationLogger,
                  applicationLocale,
                  quiltContextFactory,
                  communicationMicroService)
        { }

        public async Task<Result> ExecuteAsync(string addressLine, string city, string postalCode, DateTime paymentDate)
        {
            using var log = BeginFunction(nameof(KansasSalesTaxWebServiceOperation), nameof(ExecuteAsync), addressLine, city, postalCode, paymentDate);
            try
            {
                await Task.CompletedTask.ConfigureAwait(false);
                //if (string.IsNullOrEmpty(addressLine)) throw new BusinessOperationException("Invalid addressLine.");
                //if (string.IsNullOrEmpty(city)) throw new BusinessOperationException("Invalid city.");
                //if (string.IsNullOrEmpty(postalCode)) throw new BusinessOperationException("Invalid postalCode");
                //if (postalCode.Length != 5 && postalCode.Length != 9) throw new BusinessOperationException("Invalid postalCode.");

                //int zipCode;
                //int zipPlus;
                //if (postalCode.Length == 5)
                //{
                //    zipCode = int.Parse(postalCode);
                //    zipPlus = 0;
                //}
                //else // Length == 9
                //{
                //    zipCode = int.Parse(postalCode.Substring(0, 5));
                //    zipPlus = int.Parse(postalCode.Substring(5, 4));
                //}

                //var client = new JurisdictionLookupPortClient();
                //var clientResponse = await client.GetFIPSByAddress2Async(addressLine, city, zipCode, zipPlus, paymentDate);
                //clientResponse = await client.GetFIPSByAddress2Async(addressLine, city, zipCode, zipPlus, paymentDate);

                //if (clientResponse == null) throw new InvalidOperationException("clientResponse null");
                //if (clientResponse.FIPSRecordList == null) throw new InvalidOperationException("clientResponse.FIPSRecordList null");

                //string jurisdictionCode = null;
                //decimal salesTax = 0;
                //foreach (var fipsRecord in clientResponse.FIPSRecordList)
                //{
                //    if (jurisdictionCode == null)
                //    {
                //        jurisdictionCode = fipsRecord.strCompositeSER;
                //    }
                //    else
                //    {
                //        if (jurisdictionCode != fipsRecord.strCompositeSER)
                //        {
                //            throw new InvalidOperationException(string.Format("Jurisdiction mismatch {0} {1}", jurisdictionCode, fipsRecord.strCompositeSER));
                //        }
                //    }

                //    salesTax += decimal.Parse(fipsRecord.strGeneralTaxRateIntrastate);
                //}

                //var result = new Result()
                //{
                //    SalesTaxRate = salesTax / 100m,
                //    SalesTaxJurisdiction = jurisdictionCode
                //};

                // HACK
                var result = new Result()
                {
                    SalesTaxRate = 5m / 100m,
                    SalesTaxJurisdiction = "12345"
                };

                log.Result(result);
                return result;
            }
            catch (BusinessOperationException ex)
            {
                log.Exception(ex);
                throw;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        #region Public Classes

        public class Result
        {

            public string SalesTaxJurisdiction { get; set; }
            public decimal SalesTaxRate { get; set; }

        }

        #endregion Public Classes
    }
}