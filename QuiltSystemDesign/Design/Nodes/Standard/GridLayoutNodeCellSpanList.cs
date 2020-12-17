//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace RichTodd.QuiltSystem.Design.Nodes.Standard
{
    internal class GridLayoutNodeCellSpanList : List<GridLayoutNodeCellSpan>
    {
        public GridLayoutNodeCellSpanList()
        {
        }

        public GridLayoutNodeCellSpanList(JToken json)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));

            foreach (var jsonGridLayoutNodeCellSpan in json)
            {
                Add(new GridLayoutNodeCellSpan(jsonGridLayoutNodeCellSpan));
            }
        }

        protected GridLayoutNodeCellSpanList(IList<GridLayoutNodeCellSpan> prototype)
        {
            if (prototype == null) throw new ArgumentNullException(nameof(prototype));

            foreach (var layoutSite in prototype)
            {
                Add(layoutSite.Clone());
            }
        }

        public GridLayoutNodeCellSpanList Clone()
        {
            return new GridLayoutNodeCellSpanList(this);
        }

        public JToken JsonSave()
        {
            var jsonGridLayoutNodeCellSpanList = new JArray();
            foreach (var gridLayoutNodeCellSpan in this)
            {
                jsonGridLayoutNodeCellSpanList.Add(gridLayoutNodeCellSpan.JsonSave());
            }

            return jsonGridLayoutNodeCellSpanList;
        }
    }
}