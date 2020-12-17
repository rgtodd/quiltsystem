//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Path
{
    internal class RectanglePathGeometry : IPathGeometry
    {
        private IPath m_prototype;

        public string Name
        {
            get
            {
                return PathGeometryNames.RECTANGLE;
            }
        }

        public IPath Prototype
        {
            get
            {
                if (m_prototype == null)
                {
                    var segments = new PathSegmentList()
                    {
                        new LinePathSegment(),
                        new LinePathSegment(),
                        new LinePathSegment(),
                        new LinePathSegment()
                    };

                    m_prototype = new Path(this, segments);
                }

                return m_prototype;
            }
        }

        public IPath CreatePath(IList<PathPoint> origins)
        {
            if (origins == null) throw new ArgumentNullException(nameof(origins));
            if (origins.Count != 4) throw new ArgumentOutOfRangeException(nameof(origins));

            var segments = new PathSegmentList() {
                new LinePathSegment() { Origin = origins[0] },
                new LinePathSegment() { Origin = origins[1] },
                new LinePathSegment() { Origin = origins[2] },
                new LinePathSegment() { Origin = origins[3] } };

            var path = new Path(this, segments);

            return path;
        }

        public IPath CreatePath(Dimension width, Dimension height)
        {
            if (width.Unit != height.Unit) throw new ArgumentException("Width and height have different units.");

            var origin = new Dimension(0, width.Unit);

            var p1 = new PathPoint(origin, origin);
            var p2 = new PathPoint(width, origin);
            var p3 = new PathPoint(width, height);
            var p4 = new PathPoint(origin, height);

            return CreatePath(new PathPoint[] { p1, p2, p3, p4 });
        }

        public JToken JsonSave()
        {
            var result = new JObject()
            {
                new JProperty(JsonNames.Name, Name)
            };

            return result;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}