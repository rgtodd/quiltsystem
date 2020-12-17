//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Database.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;

namespace RichTodd.QuiltSystem.Business.Operation
{
    internal class KansasSalesTaxTableLookupOperation : BusinessOperation
    {

        public KansasSalesTaxTableLookupOperation(
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

        public async Task<Result> ExecuteAsync(string city, string postalCode, DateTime paymentDate)
        {
            using var log = BeginFunction(nameof(KansasSalesTaxTableLookupOperation), nameof(ExecuteAsync), city, postalCode, paymentDate);
            try
            {
                if (string.IsNullOrEmpty(city)) throw new BusinessOperationException("Invalid city.");
                if (string.IsNullOrEmpty(postalCode)) throw new BusinessOperationException("Invalid postalCode");
                if (postalCode.Length != 5 && postalCode.Length != 9) throw new BusinessOperationException("Invalid postalCode.");

                // Table lookup ignores plus 4
                //
                if (postalCode.Length == 9) postalCode = postalCode.Substring(0, 5);

                using (var ctx = QuiltContextFactory.Create())
                {
                    var taxTable = await ctx.KansasTaxTables.Where(r => paymentDate >= r.EffectiveDate && paymentDate < r.ExpirationDate).SingleOrDefaultAsync().ConfigureAwait(false);
                    if (taxTable == null)
                    {
                        throw new InvalidOperationException("Tax table not found for payment date.");
                    }

                    var taxTableEntry = taxTable.KansasTaxTableEntries.Where(r => r.PostalCode == postalCode && r.City == city).OrderByDescending(r => r.InsideCityTaxRate).FirstOrDefault();
                    if (taxTableEntry != null)
                    {
                        var result = new Result()
                        {
                            SalesTaxRate = taxTableEntry.InsideCityTaxRate / 100m,
                            SalesTaxJurisdiction = taxTableEntry.InsideCityJurisdictionCode,
                            CityRate = true
                        };

                        log.Result(result);
                        return result;
                    }

                    taxTableEntry = taxTable.KansasTaxTableEntries.Where(r => r.PostalCode == postalCode).OrderByDescending(r => r.OutsideCityTaxRate).FirstOrDefault();
                    if (taxTableEntry != null)
                    {
                        var result = new Result()
                        {
                            SalesTaxRate = taxTableEntry.OutsideCityTaxRate / 100m,
                            SalesTaxJurisdiction = taxTableEntry.OutsideCityJurisdictionCode,
                            CityRate = false
                        };

                        log.Result(result);
                        return result;
                    }
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

            public bool CityRate { get; set; }
            public string SalesTaxJurisdiction { get; set; }
            public decimal SalesTaxRate { get; set; }

        }

        #endregion Public Classes
    }
}