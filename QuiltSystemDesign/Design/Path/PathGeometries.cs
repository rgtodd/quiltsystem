//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Design.Path
{
    internal static class PathGeometries
    {
        private static readonly IPathGeometry s_rectangle = new RectanglePathGeometry();
        private static readonly IPathGeometry s_triangle = new TrianglePathGeometry();

        public static IPathGeometry Rectangle
        {
            get { return s_rectangle; }
        }

        public static IPathGeometry Triangle
        {
            get { return s_triangle; }
        }

        public static IPathGeometry Lookup(string name)
        {
            if (name == PathGeometryNames.TRIANGLE) return Triangle;
            if (name == PathGeometryNames.RECTANGLE) return Rectangle;

            throw new ArgumentOutOfRangeException(nameof(name));
        }
    }
}