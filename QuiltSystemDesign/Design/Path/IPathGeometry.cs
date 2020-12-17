//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Path
{
    public interface IPathGeometry
    {
        #region Properties

        string Name { get; }

        IPath Prototype { get; }

        #endregion Properties

        #region Methods

        IPath CreatePath(IList<PathPoint> origins);

        IPath CreatePath(Dimension width, Dimension height);

        JToken JsonSave();

        #endregion Methods
    }
}