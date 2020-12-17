//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace RichTodd.QuiltSystem.Design.Path
{
    class PathSegmentList : List<IPathSegment>
    {
        public PathSegmentList()
        { }

        public PathSegmentList(JToken json)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));

            foreach (var jsonPathSegment in json)
            {
                Add(PathSegmentFactory.Create(jsonPathSegment));
            }
        }

        protected PathSegmentList(IList<IPathSegment> prototype)
        {
            if (prototype == null) throw new ArgumentNullException(nameof(prototype));

            foreach (var pathSegement in prototype)
            {
                Add(pathSegement.Clone());
            }
        }

        public PathSegmentList Clone()
        {
            return new PathSegmentList(this);
        }

        public JToken JsonSave()
        {
            var jsonSegments = new JArray();
            foreach (var segment in this)
            {
                jsonSegments.Add(segment.JsonSave());
            }

            return jsonSegments;
        }
    }
}