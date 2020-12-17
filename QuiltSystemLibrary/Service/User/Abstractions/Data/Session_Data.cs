//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Service.User.Abstractions.Data
{
    public class Session_Data
    {
        public bool HasMessages { get; set; }
        public bool HasNotifications { get; set; }
        public int CartItemCount { get; set; }
    }
}
