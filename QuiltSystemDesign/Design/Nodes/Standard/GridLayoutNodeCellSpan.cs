//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

using Newtonsoft.Json.Linq;

namespace RichTodd.QuiltSystem.Design.Nodes.Standard
{
    public class GridLayoutNodeCellSpan
    {
        private int m_columnSpan;
        private int m_rowSpan;

        public GridLayoutNodeCellSpan(int rowSpan, int columnSpan)
        {
            if (rowSpan <= 0) throw new ArgumentOutOfRangeException(nameof(rowSpan));
            if (columnSpan <= 0) throw new ArgumentOutOfRangeException(nameof(columnSpan));

            m_rowSpan = rowSpan;
            m_columnSpan = columnSpan;
        }

        public GridLayoutNodeCellSpan(JToken json)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));

            m_rowSpan = json.Value<int>(JsonNames.RowSpan);
            m_columnSpan = json.Value<int>(JsonNames.ColumnSpan);
        }

        protected GridLayoutNodeCellSpan(GridLayoutNodeCellSpan prototype)
        {
            if (prototype == null) throw new ArgumentNullException(nameof(prototype));

            m_rowSpan = prototype.m_rowSpan;
            m_columnSpan = prototype.m_columnSpan;
        }

        public int ColumnSpan
        {
            get
            {
                return m_columnSpan;
            }

            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value));

                m_columnSpan = value;
            }
        }

        public int RowSpan
        {
            get
            {
                return m_rowSpan;
            }

            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value));

                m_rowSpan = value;
            }
        }

        public GridLayoutNodeCellSpan Clone()
        {
            return new GridLayoutNodeCellSpan(this);
        }

        public JToken JsonSave()
        {
            var result = new JObject()
            {
                new JProperty(JsonNames.RowSpan, m_rowSpan),
                new JProperty(JsonNames.ColumnSpan, m_columnSpan)
            };

            return result;
        }
    }
}