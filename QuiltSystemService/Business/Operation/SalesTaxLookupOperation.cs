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
    public class SalesTaxLookupOperation : BusinessOperation
    {

        public SalesTaxLookupOperation(
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

        public async Task<Result> ExecuteAsync(string addressLine, string city, string stateCode, string postalCode, DateTime paymentDate)
        {
            using var log = BeginFunction(nameof(SalesTaxLookupOperation), nameof(ExecuteAsync), addressLine, city, stateCode, postalCode, paymentDate);
            try
            {
                if (string.IsNullOrEmpty(addressLine)) throw new BusinessOperationException("Invalid addressLine.");
                if (string.IsNullOrEmpty(city)) throw new BusinessOperationException("Invalid city.");
                if (string.IsNullOrEmpty(stateCode)) throw new BusinessOperationException("Invalid stateCode.");
                if (string.IsNullOrEmpty(postalCode)) throw new BusinessOperationException("Invalid postalCode");
                if (postalCode.Length != 5 && postalCode.Length != 9) throw new BusinessOperationException("Invalid postalCode.");

                if (stateCode != "KS")
                {
                    var result = new Result()
                    {
                        SalesTaxRate = 0m,
                        SalesTaxJurisdiction = stateCode
                    };

                    log.Result(result);
                    return result;
                }

                // Attempt to retrieve sales tax rate via web service.
                //
                {
                    var op = new KansasSalesTaxWebServiceOperation(Logger, Locale, QuiltContextFactory, CommunicationMicroService);
                    try
                    {
                        var opResult = await op.ExecuteAsync(addressLine, city, postalCode, paymentDate).ConfigureAwait(false);

                        var result = new Result()
                        {
                            SalesTaxRate = opResult.SalesTaxRate,
                            SalesTaxJurisdiction = stateCode + "/" + opResult.SalesTaxJurisdiction + "/" + "WEB"
                        };

                        log.Result(result);
                        return result;
                    }
                    catch (Exception ex)
                    {
                        log.Exception(ex);
                    }
                }

                // Lookup sales tax via rate table.
                //
                {
                    var op = new KansasSalesTaxTableLookupOperation(Logger, Locale, QuiltContextFactory, CommunicationMicroService);
                    var opResult = await op.ExecuteAsync(city, postalCode, paymentDate).ConfigureAwait(false);

                    var result = new Result()
                    {
                        SalesTaxRate = opResult.SalesTaxRate,
                        SalesTaxJurisdiction = stateCode + "/" + opResult.SalesTaxJurisdiction + "/" + "TABLE" + "/" + (opResult.CityRate ? "CITY" : "COUNTY")
                    };

                    log.Result(result);
                    return result;
                }

                throw new InvalidOperationException("Salex tax record not found.");
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