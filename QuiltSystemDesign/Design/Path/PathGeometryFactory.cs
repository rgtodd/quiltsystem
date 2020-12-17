//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using Newtonsoft.Json.Linq;

namespace RichTodd.QuiltSystem.Design.Path
{
    internal static class PathGeometryFactory
    {
        #region Methods

        public static IPathGeometry Create(JToken json)
        {
            var name = (string)json[JsonNames.Name];

            return PathGeometries.Lookup(name);
        }

        #endregion Methods
    }
}