﻿//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MPayPal_ButtonRequestData
    {
        public string ApiUrl { get; set; }

        public string ApiUserName { get; set; }
        public string ApiPassword { get; set; }
        public string ApiSignature { get; set; }

        public decimal Amount { get; set; } // amount
        public string ItemName { get; set; } // item_name
        public string ItemNumber { get; set; } // item_number
        public int Quantity { get; set; } // quantity
        public decimal Shipping { get; set; } // shipping
        public decimal Tax { get; set; } // tax

        public string Return { get; set; } // return
        public string CancelReturn { get; set; } // cancel_return

        public string AddressLine1 { get; set; } // address1
        public string City { get; set; } // city
        public string CountryCode { get; set; } // country
        public string Email { get; set; } // email
        public string State { get; set; } // state
        public string Zip { get; set; } // zip

        public static MPayPal_ButtonRequestData CreateDefault()
        {
            return new MPayPal_ButtonRequestData()
            {
                ApiUrl = "https://api-3t.sandbox.paypal.com/nvp",
                ApiUserName = "*SECRET*",
                ApiPassword = "*SECRET*",
                ApiSignature = "*SECRET*",
                CountryCode = "US",
                Return = "https://quiltagogo.azurewebsites.net/Cart/PayComplete",
                CancelReturn = "https://quiltagogo.azurewebsites.net/Cart/PayCancel"
            };
        }
    }
}
