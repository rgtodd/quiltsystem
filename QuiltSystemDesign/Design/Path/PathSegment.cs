//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using Newtonsoft.Json.Linq;

using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Path
{
    abstract class PathSegment : IPathSegment
    {
        private PathPoint m_origin;

        public PathSegment()
        { }

        public PathSegment(PathPoint origin)
        {
            m_origin = origin;
        }

        public PathSegment(JToken json)
        {
            m_origin = new PathPoint(json[JsonNames.Origin]);
        }

        protected PathSegment(PathSegment prototype)
        {
            m_origin = prototype.m_origin;
        }

        public abstract IPathSegment Clone();

        public virtual JToken JsonSave()
        {
            var result = new JObject()
            {
                new JProperty(JsonNames.TypeName, GetType().Name),
                new JProperty(JsonNames.Origin, m_origin.JsonSave())
            };

            return result;
        }

        public PathPoint Origin
        {
            get
            {
                return m_origin;
            }

            set
            {
                m_origin = value;
            }
        }

        public abstract void Copy(IPathSegment fromPathSegment);

        public abstract Dimension Distance(PathPoint destination);

        public abstract PathPoint Interpolate(PathPoint destination, double ratio);

        public abstract PathPoint Offset(PathPoint destination, double distance);

        public abstract bool Intersects(PathPoint destination, PathPoint lineFrom, PathPoint lineTo);
    }
}
