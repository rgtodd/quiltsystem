//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

using Newtonsoft.Json.Linq;

namespace RichTodd.QuiltSystem.Design.Path
{
    static class PathSegmentFactory
    {
        public static IPathSegment Create(JToken json)
        {
            var typeName = (string)json[JsonNames.TypeName];

            return typeName switch
            {
                "LinePathSegment" => new LinePathSegment(json),
                _ => throw new InvalidOperationException(string.Format("Unknown type name {0}.", typeName)),
            };
        }
    }
}
