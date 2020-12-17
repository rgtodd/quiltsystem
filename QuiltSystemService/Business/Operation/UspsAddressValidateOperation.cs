//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Xml;

using Microsoft.Extensions.Logging;

using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Database.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;

namespace RichTodd.QuiltSystem.Business.Operation
{
    public class UspsAddressValidateOperation : BusinessOperation
    {

        public UspsAddressValidateOperation(
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

        public async Task<Result> ExecuteAsync(string addressLine1, string addressLine2, string city, string stateCode, string postalCode)
        {
            using var log = BeginFunction(nameof(UspsAddressValidateOperation), nameof(ExecuteAsync), addressLine1, addressLine2, city, stateCode, postalCode);
            try
            {
                if (string.IsNullOrEmpty(addressLine1)) throw new BusinessOperationException("Invalid addressLine1.");
                if (string.IsNullOrEmpty(city)) throw new BusinessOperationException("Invalid city.");
                if (string.IsNullOrEmpty(stateCode)) throw new BusinessOperationException("Invalid stateCode.");
                if (string.IsNullOrEmpty(postalCode)) throw new BusinessOperationException("Invalid postalCode");
                if (postalCode.Length != 5 && postalCode.Length != 9) throw new BusinessOperationException("Invalid postalCode.");

                string zip5;
                string zip4;
                if (postalCode.Length == 5)
                {
                    zip5 = postalCode;
                    zip4 = "";
                }
                else // Length == 9
                {
                    zip5 = postalCode.Substring(0, 5);
                    zip4 = postalCode.Substring(5, 4);
                }

                var request =
@"<AddressValidateRequest USERID=""871NA0001502"">
    <IncludeOptionalElements>false</IncludeOptionalElements>
    <ReturnCarrierRoute>false</ReturnCarrierRoute>
    <Address ID = ""0"">
        <FirmName />
        <Address1>" + addressLine1 + @"</Address1>
        <Address2>" + addressLine2 + @"</Address2>
        <City>" + city + @"</City>
        <State>" + stateCode + @"</State>
        <Zip5>" + zip5 + @"</Zip5>
        <Zip4>" + zip4 + @"</Zip4>
    </Address>
</AddressValidateRequest>";

                request = request.Replace("\r", "");
                request = request.Replace("\n", "");

                var url = @"http://production.shippingapis.com/ShippingAPI.dll?API=Verify&XML=" + request;

                var webRequest = WebRequest.Create(url);

                string responseData;
                using (var response = await webRequest.GetResponseAsync().ConfigureAwait(false))
                {
                    using var responseStream = response.GetResponseStream();
                    using var responseReader = new StreamReader(responseStream);
                    responseData = await responseReader.ReadToEndAsync().ConfigureAwait(false);
                }

                var result = new Result();

                using (var responseReader = new StringReader(responseData))
                {
                    using var xmlReader = XmlReader.Create(responseReader);
                    while (xmlReader.Read())
                    {
                        if (xmlReader.NodeType == XmlNodeType.Element)
                        {
                            switch (xmlReader.Name)
                            {
                                case "Address1": xmlReader.Read(); result.Address1 = xmlReader.Value; break;
                                case "Address2": xmlReader.Read(); result.Address2 = xmlReader.Value; break;
                                case "City": xmlReader.Read(); result.City = xmlReader.Value; break;
                                case "State": xmlReader.Read(); result.StateCode = xmlReader.Value; break;
                                case "Zip5": xmlReader.Read(); result.PostalCode = xmlReader.Value; break;
                                case "Zip4": xmlReader.Read(); result.PostalCode += xmlReader.Value; break; // Assumes Zip5 preceeds Zip4
                            }
                        }
                    }
                }

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

            public string Address1 { get; set; }
            public string Address2 { get; set; }
            public string City { get; set; }
            public string PostalCode { get; set; }
            public string ResponseData { get; set; }
            public string StateCode { get; set; }

        }

        #endregion Public Classes
    }
}