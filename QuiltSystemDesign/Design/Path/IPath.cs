//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using Newtonsoft.Json.Linq;

using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Path
{
    public interface IPath
    {
        IPath Clone();
        JToken JsonSave();

        IPathGeometry PathGeometry { get; }
        int SegmentCount { get; }

        bool Contains(PathPoint point);
        void Copy(IPath fromPath);
        void Copy(IPath fromPath, PathOrientation pathOrientation);
        PathBounds GetBounds();
        string GetPathDescription();
        IPathSegment GetSegment(int index);
        Dimension GetLength(int index);
        PathPoint Interpolate(int index, double ratio);
        PathPoint Offset(int index, double distance);
    }
}
