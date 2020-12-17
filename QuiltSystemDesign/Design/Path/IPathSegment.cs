//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using Newtonsoft.Json.Linq;

using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Path
{
    public interface IPathSegment
    {
        IPathSegment Clone();
        JToken JsonSave();

        PathPoint Origin { get; }

        void Copy(IPathSegment fromPathSegment);
        Dimension Distance(PathPoint destination);
        PathPoint Interpolate(PathPoint destination, double ratio);
        PathPoint Offset(PathPoint destination, double distance);
        bool Intersects(PathPoint destination, PathPoint lineFrom, PathPoint lineTo);
    }
}
