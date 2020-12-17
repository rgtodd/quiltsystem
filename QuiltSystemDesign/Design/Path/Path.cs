//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Text;

using Newtonsoft.Json.Linq;

using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Path
{
    class Path : IPath
    {
        private readonly IPathGeometry m_pathGeometry;
        private readonly PathSegmentList m_segments;

        public Path(IPathGeometry pathGeometry, PathSegmentList segments)
        {
            m_pathGeometry = pathGeometry ?? throw new ArgumentNullException(nameof(pathGeometry));
            m_segments = segments ?? throw new ArgumentNullException(nameof(segments));
        }

        public Path(JToken json)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));

            m_pathGeometry = PathGeometryFactory.Create(json[JsonNames.PathGeometry]);
            m_segments = new PathSegmentList(json[JsonNames.Segments]);
        }

        protected Path(Path prototype)
        {
            if (prototype == null) throw new ArgumentNullException(nameof(prototype));

            m_pathGeometry = prototype.m_pathGeometry;
            m_segments = prototype.m_segments.Clone();
        }

        public IPath Clone()
        {
            return new Path(this);
        }

        public virtual JToken JsonSave()
        {
            var result = new JObject()
            {
                new JProperty(JsonNames.TypeName, GetType().Name),
                new JProperty(JsonNames.PathGeometry, m_pathGeometry.JsonSave()),
                new JProperty(JsonNames.Segments, m_segments.JsonSave())
            };

            return result;
        }

        public IPathGeometry PathGeometry
        {
            get
            {
                return m_pathGeometry;
            }
        }

        public int SegmentCount
        {
            get { return m_segments.Count; }
        }

        public bool Contains(PathPoint point)
        {
            var bounds = GetBounds();

            if (!bounds.Contains(point))
            {
                return false;
            }

            PathPoint exteriorPoint = new PathPoint(bounds.MaximumX + 1000, bounds.MaximumY + 1000);

            int intersectCount = IntersectionCount(point, exteriorPoint);

            return (intersectCount % 2) == 1;
        }

        public void Copy(IPath fromPath)
        {
            if (fromPath == null) throw new ArgumentNullException(nameof(fromPath));
            if (PathGeometry != fromPath.PathGeometry) throw new ArgumentException("PathGeometry mismatch.", nameof(fromPath));

            if (SegmentCount != fromPath.SegmentCount)
                throw new ArgumentException("Segment counts do not match", nameof(fromPath));

            for (int idx = 0; idx < SegmentCount; ++idx)
            {
                m_segments[idx].Copy(fromPath.GetSegment(idx));
            }
        }

        public void Copy(IPath fromPath, PathOrientation pathOrientation)
        {
            if (fromPath == null) throw new ArgumentNullException(nameof(fromPath));
            if (pathOrientation == null) throw new ArgumentNullException(nameof(pathOrientation));

            if (PathGeometry != fromPath.PathGeometry) throw new ArgumentException("PathGeometry mismatch.", nameof(fromPath));
            if (SegmentCount != fromPath.SegmentCount) throw new ArgumentException("Segment count mismatch.", nameof(fromPath));

            for (int idxSource = 0; idxSource < SegmentCount; ++idxSource)
            {
                int idxTarget;
                if (pathOrientation.PathDirection == PathDirections.Clockwise)
                {
                    idxTarget = idxSource;
                }
                else
                {
                    idxTarget = -idxSource;
                }
                idxTarget += pathOrientation.PointOffset;

                idxTarget %= SegmentCount;
                if (idxTarget < 0)
                {
                    idxTarget += SegmentCount;
                }

                m_segments[idxTarget].Copy(fromPath.GetSegment(idxSource));
            }
        }

        public PathBounds GetBounds()
        {
            if (m_segments.Count == 0)
            {
                throw new InvalidOperationException("Path is empty.");
            }

            var minimumX = m_segments[0].Origin.X;
            var maximumX = m_segments[0].Origin.X;
            var minimumY = m_segments[0].Origin.Y;
            var maximumY = m_segments[0].Origin.Y;

            foreach (var segment in m_segments)
            {
                if (segment.Origin.X < minimumX) minimumX = segment.Origin.X;
                if (segment.Origin.X > maximumX) maximumX = segment.Origin.X;
                if (segment.Origin.Y < minimumY) minimumY = segment.Origin.Y;
                if (segment.Origin.Y > maximumY) maximumY = segment.Origin.Y;
            }

            return new PathBounds(minimumX, maximumX, minimumY, maximumY);
        }

        public string GetPathDescription()
        {
            var sb = new StringBuilder();

            var prefix = string.Empty;
            for (int idx = 0; idx < m_segments.Count; ++idx)
            {
                sb.Append(prefix); prefix = ", ";

                int degrees = (int)(180.0 * GetAngle(idx) / Math.PI);

                sb.Append(degrees.ToString());
            }

            return sb.ToString();
        }

        public IPathSegment GetSegment(int index)
        {
            if (index < 0) return m_segments[index + m_segments.Count];
            if (index >= m_segments.Count) return m_segments[index - m_segments.Count];
            return m_segments[index];
        }

        public Dimension GetLength(int index)
        {
            return GetSegment(index).Distance(GetSegment(index + 1).Origin);
        }

        public PathPoint Interpolate(int index, double ratio)
        {
            if (index < 0 || index >= SegmentCount) throw new ArgumentOutOfRangeException(nameof(index));

            if (ratio < -1) ratio = -1;
            if (ratio > 1) ratio = 1;

            IPathSegment fromSegment = m_segments[index];
            IPathSegment toSegment = m_segments[(index + 1) % SegmentCount];

            return fromSegment.Interpolate(toSegment.Origin, ratio);
        }

        public PathPoint Offset(int index, double distance)
        {
            if (index < 0 || index >= SegmentCount) throw new ArgumentOutOfRangeException(nameof(index));

            IPathSegment fromSegment = m_segments[index];
            IPathSegment toSegment = m_segments[(index + 1) % SegmentCount];

            return fromSegment.Offset(toSegment.Origin, distance);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("[");

            string prefix = string.Empty;
            foreach (PathSegment pathSegment in m_segments)
            {
                sb.Append(prefix); prefix = ",";
                sb.Append(pathSegment.ToString());
            }
            sb.Append("]");

            return sb.ToString();
        }

        private double GetAngle(int index)
        {
            var from = GetSegment(index - 1).Origin;
            var midpoint = GetSegment(index).Origin;
            var to = GetSegment(index + 1).Origin;

            return Geometry.Angle(from, midpoint, to);
        }

        private int IntersectionCount(PathPoint lineFrom, PathPoint lineTo)
        {
            int count = 0;

            for (int idx = 0; idx < m_segments.Count; ++idx)
            {
                var segment = m_segments[idx];
                var nextSegment = m_segments[(idx + 1) % m_segments.Count];

                if (segment.Intersects(nextSegment.Origin, lineFrom, lineTo))
                {
                    count += 1;
                }
            }

            return count;
        }
    }
}
