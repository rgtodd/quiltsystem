//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Service.Base
{
    internal static class CreateOwnerReference
    {
        public static string FromUserId(string userId)
        {
            var reference = $"{ReferencePrefixes.User}{userId}";

            return reference;
        }
    }
}
