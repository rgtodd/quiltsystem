//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Service.Admin.Abstractions.Data
{
    public class AUser_GetUsers
    {
        public string UserName { get; set; }
        public string Role { get; set; }
        public int? RecordCount { get; set; }
    }
}
