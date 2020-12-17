//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Admin.Abstractions.Data
{
    public class ANotification_Notification
    {
        public MCommunication_Notification MNotification { get; set; }
        public MUser_User MUser { get; set; }
    }
}
