//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Web.Models.Cart
{
    public class CartSquareModel
    {
        public decimal OrderTotal { get; set; }
        public string Nonce { get; set; }
        public string Errors { get; set; }
        public string CardData { get; set; }
    }
}
