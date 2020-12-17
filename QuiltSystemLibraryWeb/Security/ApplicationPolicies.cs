//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Security
{
    public static class ApplicationPolicies
    {
        public const string IsAdministrator = "IsAdministrator";
        public const string IsService = "IsService";
        public const string IsEndUser = "IsUser";
        public const string IsPriviledged = "IsPriviledged";

        public const string AllowViewFinancial = "AllowViewFinancial";
        public const string AllowEditFinancial = "AllowEditFinancial";
        public const string AllowViewFulfillment = "AllowViewFulfillment";
        public const string AllowEditFulfillment = "AllowEditFulfillment";
        public const string AllowViewUser = "AllowViewUser";
        public const string AllowEditUser = "AllowEditUser";
    }
}
