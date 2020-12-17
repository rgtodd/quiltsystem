//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Service.Core.Abstractions
{
    public interface IApplicationLocale
    {
        DateTime GetUtcNow();

        DateTime GetLocalNow();

        DateTime GetLocalTimeFromUtc(DateTime dateTime);

        DateTime? GetLocalTimeFromUtc(DateTime? dateTime);

        DateTime GetUtcFromLocalTime(DateTime dateTime);

        DateTime? GetUtcFromLocalTime(DateTime? dateTime);

        TimeZoneInfo GetLocalTimeZoneInfo();
    }
}
