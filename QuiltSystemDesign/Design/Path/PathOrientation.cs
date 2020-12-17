//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

using Newtonsoft.Json.Linq;

namespace RichTodd.QuiltSystem.Design.Path
{
    public class PathOrientation
    {
        private int m_pointOffset;
        private PathDirections m_pathDirection;

        public PathOrientation(int pointOffset, PathDirections pathDirection)
        {
            m_pointOffset = pointOffset;
            m_pathDirection = pathDirection;
        }

        public PathOrientation(JToken json)
        {
            var pointOffset = (int)json[JsonNames.PointOffset];
            var pathDirection = (PathDirections)Enum.Parse(typeof(PathDirections), (string)json[JsonNames.PathDirection]);

            m_pointOffset = pointOffset;
            m_pathDirection = pathDirection;
        }

        public static PathOrientation CreateDefault()
        {
            return new PathOrientation(0, PathDirections.Clockwise);
        }

        public PathOrientation Clone()
        {
            return new PathOrientation(PointOffset, PathDirection);
        }

        public JToken JsonSave()
        {
            var result = new JObject()
            {
                new JProperty(JsonNames.PointOffset, m_pointOffset),
                new JProperty(JsonNames.PathDirection, m_pathDirection.ToString())
            };

            return result;
        }

        public int PointOffset
        {
            get
            {
                return m_pointOffset;
            }

            set
            {
                m_pointOffset = value;
            }
        }

        public PathDirections PathDirection
        {
            get
            {
                return m_pathDirection;
            }

            set
            {
                m_pathDirection = value;
            }
        }

        public void RotateRight(int pointCount)
        {
            int pointOffset = (PointOffset + 1) % pointCount;

            PointOffset = pointOffset;
        }

        public void RotateLeft(int pointCount)
        {
            int pointOffset = (PointOffset - 1) % pointCount;
            if (pointOffset < 0)
            {
                pointOffset += pointCount;
            }

            PointOffset = pointOffset;
        }

        public void Flip()
        {
            if (PathDirection == PathDirections.Clockwise)
            {
                PathDirection = PathDirections.Counterclockwise;
            }
            else
            {
                PathDirection = PathDirections.Clockwise;
            }
        }
    }
}
