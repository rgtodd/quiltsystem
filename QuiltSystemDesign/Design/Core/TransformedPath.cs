//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

using Newtonsoft.Json.Linq;

using RichTodd.QuiltSystem.Design.Path;
using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Core
{
    class TransformedPath : IPath
    {
        private readonly IPath m_path;
        private readonly PathOrientation m_pathOrientation;

        public TransformedPath(IPath path, PathOrientation pathOrientation)
        {
            m_path = path ?? throw new ArgumentNullException(nameof(path));
            m_pathOrientation = pathOrientation ?? throw new ArgumentNullException(nameof(pathOrientation));
        }

        protected TransformedPath(TransformedPath prototype)
        {
            if (prototype == null) throw new ArgumentNullException(nameof(prototype));

            m_path = prototype.m_path.Clone();
            m_pathOrientation = prototype.m_pathOrientation.Clone();
        }

        public IPathGeometry PathGeometry
        {
            get
            {
                return m_path.PathGeometry;
            }
        }

        public int SegmentCount
        {
            get
            {
                return m_path.SegmentCount;
            }
        }

        public bool Contains(PathPoint point)
        {
            throw new NotImplementedException();
        }

        public void Copy(IPath fromPath)
        {
            throw new NotImplementedException();
        }

        public void Copy(IPath fromPath, PathOrientation pathOrientation)
        {
            throw new NotImplementedException();
        }

        public PathBounds GetBounds()
        {
            throw new NotImplementedException();
        }

        public string GetPathDescription()
        {
            throw new NotImplementedException();
        }

        public IPathSegment GetSegment(int index)
        {
            throw new NotImplementedException();
        }

        public Dimension GetLength(int index)
        {
            throw new NotImplementedException();
        }

        public PathPoint Interpolate(int index, double ratio)
        {
            throw new NotImplementedException();
        }

        public PathPoint Offset(int index, double distance)
        {
            throw new NotImplementedException();
        }

        public JToken JsonSave()
        {
            throw new NotImplementedException();
        }

        public IPath Clone()
        {
            return new TransformedPath(this);
        }
    }
}
