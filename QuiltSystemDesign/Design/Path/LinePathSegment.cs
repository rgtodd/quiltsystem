//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

using Newtonsoft.Json.Linq;

using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Path
{
    internal class LinePathSegment : PathSegment
    {
        public LinePathSegment()
        { }

        public LinePathSegment(PathPoint origin)
            : base(origin)
        { }

        public LinePathSegment(JToken json)
            : base(json)
        { }

        protected LinePathSegment(LinePathSegment prototype) : base(prototype)
        { }

        public override IPathSegment Clone()
        {
            return new LinePathSegment(this);
        }

        public override void Copy(IPathSegment fromPathSegment)
        {
            if (fromPathSegment == null) throw new ArgumentNullException(nameof(fromPathSegment));

            if (!(fromPathSegment is LinePathSegment fromLinePathSegment))
                throw new ArgumentException("Path segment type mismatch.", nameof(fromPathSegment));

            Origin = fromLinePathSegment.Origin;
        }

        public override Dimension Distance(PathPoint destination)
        {
            var dx = destination.X - Origin.X;
            var dy = destination.Y - Origin.Y;

            var distanceSquared = (dx * dx) + (dy * dy);

            var distance = new Dimension(Math.Sqrt(distanceSquared.Value), distanceSquared.Unit);

            return distance;
        }

        public override PathPoint Interpolate(PathPoint destination, double ratio)
        {
            if (ratio < -1 || ratio > 1) throw new ArgumentOutOfRangeException(nameof(ratio));

            return ratio >= 0
                ? new PathPoint(
                    Origin.X + ((destination.X - Origin.X) * ratio),
                    Origin.Y + ((destination.Y - Origin.Y) * ratio))
                : new PathPoint(
                    destination.X + ((Origin.X - destination.X) * -ratio),
                    destination.Y + ((Origin.Y - destination.Y) * -ratio));
        }

        public override bool Intersects(PathPoint destination, PathPoint lineFrom, PathPoint lineTo)
        {
            var intersects = Geometry.Intersects(Origin, destination, lineFrom, lineTo);

            return intersects;
        }

        public override JToken JsonSave()
        {
            var result = base.JsonSave();

            return result;
        }

        public override PathPoint Offset(PathPoint destination, double distance)
        {
            var totalDistance = Distance(destination).Value;

            var ratio = distance / totalDistance;

            return Interpolate(destination, ratio);
        }

        public override string ToString()
        {
            return string.Format("({0},{1})", Origin.X, Origin.Y);
        }
    }
}