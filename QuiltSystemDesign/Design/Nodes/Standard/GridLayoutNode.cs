//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using RichTodd.QuiltSystem.Design.Path;
using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Nodes.Standard
{
    [Node(PathGeometryNames.RECTANGLE)]
    public class GridLayoutNode : LayoutNode
    {
        private readonly LayoutSiteList m_layoutSites;
        private GridLayoutNodeCellSpanList m_cellSpans;
        private int m_columnCount;
        private int m_rowCount;

        public GridLayoutNode(int rowCount, int columnCount) : base(PathGeometries.Rectangle)
        {
            if (rowCount <= 0) throw new ArgumentOutOfRangeException(nameof(rowCount));
            if (columnCount <= 0) throw new ArgumentOutOfRangeException(nameof(columnCount));

            m_rowCount = rowCount;
            m_columnCount = columnCount;
            m_layoutSites = new LayoutSiteList(this);
            m_cellSpans = null;

            ResizeLists();
        }

        public GridLayoutNode(JToken json) : base(json)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));

            m_rowCount = json.Value<int>(JsonNames.RowCount);
            m_columnCount = json.Value<int>(JsonNames.ColumnCount);
            m_layoutSites = new LayoutSiteList(this, json[JsonNames.LayoutSites]);

            var jsonCellSpans = json[JsonNames.CellSpans];
            m_cellSpans = jsonCellSpans != null ? new GridLayoutNodeCellSpanList(jsonCellSpans) : null;
        }

        protected GridLayoutNode(GridLayoutNode prototype) : base(prototype)
        {
            if (prototype == null) throw new ArgumentNullException(nameof(prototype));

            m_rowCount = prototype.m_rowCount;
            m_columnCount = prototype.m_columnCount;
            m_layoutSites = prototype.m_layoutSites.Clone(this);

            m_cellSpans = prototype.m_cellSpans?.Clone();
        }

        public int ColumnCount
        {
            get
            {
                return m_columnCount;
            }

            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value));

                m_columnCount = value;

                ResizeLists();
            }
        }

        public override IReadOnlyList<LayoutSite> LayoutSites
        {
            get
            {
                return m_layoutSites;
            }
        }

        public int RowCount
        {
            get
            {
                return m_rowCount;
            }

            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value));

                m_rowCount = value;

                ResizeLists();
            }
        }

        public override Node Clone()
        {
            return new GridLayoutNode(this);
        }

        public int GetColumnSpan(int row, int column)
        {
            var cellSpan = GetCellSpan(row, column, false);
            return cellSpan == null ? 1 : cellSpan.ColumnSpan;
        }

        public LayoutSite GetLayoutSite(int row, int column)
        {
            return LayoutSites[GetListIndex(row, column)];
        }

        public int GetRowSpan(int row, int column)
        {
            var cellSpan = GetCellSpan(row, column, false);
            return cellSpan == null ? 1 : cellSpan.RowSpan;
        }

        public GridLayoutNode GetSimplified()
        {
            var simplifiedHorizontal = (GridLayoutNode)Clone();
            simplifiedHorizontal.SimplifyHorizontal();
            simplifiedHorizontal.SimplifyVertical();

            var simplifiedVertical = (GridLayoutNode)Clone();
            simplifiedVertical.SimplifyVertical();
            simplifiedVertical.SimplifyHorizontal();

            return simplifiedVertical.GetShapeNodes().Count < simplifiedHorizontal.GetShapeNodes().Count
                ? simplifiedVertical
                : simplifiedHorizontal;
        }

        public override JToken JsonSave()
        {
            var result = base.JsonSave();

            result[JsonNames.RowCount] = m_rowCount;
            result[JsonNames.ColumnCount] = m_columnCount;
            result[JsonNames.LayoutSites] = m_layoutSites.JsonSave();

            if (m_cellSpans != null)
            {
                result[JsonNames.CellSpans] = m_cellSpans.JsonSave();
            }

            return result;
        }

        public void SetColumnSpan(int row, int column, int columnSpan)
        {
            var cellSpan = GetCellSpan(row, column, true);
            cellSpan.ColumnSpan = columnSpan;
        }

        public void SetRowSpan(int row, int column, int rowSpan)
        {
            var cellSpan = GetCellSpan(row, column, true);
            cellSpan.RowSpan = rowSpan;
        }

        public override void UpdatePath(IPath path, PathOrientation pathOrientation, DimensionScale scale)
        {
            base.UpdatePath(path, pathOrientation, scale);

            if (path.GetBounds().Empty)
            {
                return;
            }

            //Trace.TraceInformation("GridLayout::ResizeChildren");

            var columnRatio = 1.0 / ColumnCount;
            //Trace.TraceInformation("columnRatio = {0}", columnRatio);

            var rowRatio = 1.0 / RowCount;
            //Trace.TraceInformation("rowRatio = {0}", rowRatio);

            for (var column = 0; column < ColumnCount; ++column)
            {
                for (var row = 0; row < RowCount; ++row)
                {
                    var columnSpan = GetColumnSpan(row, column);
                    var rowSpan = GetRowSpan(row, column);

                    var top1 = Path.Interpolate(0, column * columnRatio);
                    var top2 = Path.Interpolate(0, (column + columnSpan) * columnRatio);

                    var bottom1 = Path.Interpolate(2, 1.0 - (column * columnRatio));
                    var bottom2 = Path.Interpolate(2, 1.0 - ((column + columnSpan) * columnRatio));

                    var right1 = Path.Interpolate(1, row * rowRatio);
                    var right2 = Path.Interpolate(1, (row + rowSpan) * rowRatio);

                    var left1 = Path.Interpolate(3, 1.0 - (row * rowRatio));
                    var left2 = Path.Interpolate(3, 1.0 - ((row + rowSpan) * rowRatio));

                    var p1 = Geometry.Intersection(top1, bottom1, right1, left1);
                    var p2 = Geometry.Intersection(top2, bottom2, right1, left1);
                    var p3 = Geometry.Intersection(top2, bottom2, right2, left2);
                    var p4 = Geometry.Intersection(top1, bottom1, right2, left2);

                    if (p1.IsInvalid || p2.IsInvalid || p3.IsInvalid || p4.IsInvalid)
                    {
                        throw new InvalidOperationException();
                    }
                    else
                    {
                        var pathCell = PathGeometries.Rectangle.CreatePath(
                            new PathPoint[] { p1, p2, p3, p4 });

                        //Trace.TraceInformation("Layout {0},{1} = {2}", row, column, path.ToString());

                        var layoutSite = GetLayoutSite(row, column);
                        layoutSite.UpdatePath(pathCell, scale);
                    }
                }
            }
        }

        private GridLayoutNodeCellSpan GetCellSpan(int row, int column, bool ensureExists)
        {
            if (ensureExists)
            {
                if (m_cellSpans == null)
                {
                    m_cellSpans = new GridLayoutNodeCellSpanList();
                }

                var index = GetListIndex(row, column);
                while (index >= m_cellSpans.Count)
                {
                    m_cellSpans.Add(new GridLayoutNodeCellSpan(1, 1));
                }

                var cellSpan = m_cellSpans[index];
                return cellSpan;
            }
            else
            {
                if (m_cellSpans == null)
                {
                    return null;
                }

                var index = GetListIndex(row, column);
                if (index >= m_cellSpans.Count)
                {
                    return null;
                }

                var cellSpan = m_cellSpans[index];
                return cellSpan;
            }
        }

        private int GetListIndex(int row, int column)
        {
            return row < 0 || row > RowCount - 1
                ? throw new ArgumentOutOfRangeException(nameof(row))
                : column < 0 || column > ColumnCount - 1 
                    ? throw new ArgumentOutOfRangeException(nameof(column)) 
                    : (row * ColumnCount) + column;
        }

        private void ResizeLists()
        {
            var size = RowCount * ColumnCount;

            for (var idx = m_layoutSites.Count; idx < size; ++idx)
            {
                m_layoutSites.Add(new LayoutSite(this, PathGeometries.Rectangle));
            }

            for (var idx = m_layoutSites.Count; idx > size; --idx)
            {
                m_layoutSites.RemoveAt(idx - 1);
            }

            if (m_cellSpans != null)
            {
                for (var idx = m_cellSpans.Count; idx > size; --idx)
                {
                    m_cellSpans.RemoveAt(idx - 1);
                }
            }
        }

        private void SimplifyHorizontal()
        {
            for (var row = 0; row < RowCount; ++row)
            {
                for (var column = 0; column < ColumnCount; ++column)
                {
                    while (SimplifyRight(row, column))
                    {
                        // Keep expanding the cell to the right.
                    }
                }
            }
        }

        private bool SimplifyRight(int row, int column)
        {
            var sourceNode = GetRectangleShapeNode(row, column);
            if (sourceNode == null)
            {
                // We can only simplify rectangles.
                //
                return false;
            }

            var columnSpan = GetColumnSpan(row, column);
            if (column + columnSpan >= ColumnCount)
            {
                // No more columns in layout.
                //
                return false;
            }

            var targetColumn = column + columnSpan;
            var targetNode = GetRectangleShapeNode(row, targetColumn);
            if (targetNode == null)
            {
                // Adjacent shape is not a rectangle.
                //
                return false;
            }

            if (sourceNode.Style != targetNode.Style)
            {
                // The two rectangles are different styles.
                //
                return false;
            }

            if (GetRowSpan(row, column) != GetRowSpan(row, targetColumn))
            {
                // The two rectangles are different heights.
                //
                return false;
            }

            // All checks were successful.  We can merge the cells.

            // Add the target column span to the source.
            //
            SetColumnSpan(row, column, columnSpan + GetColumnSpan(row, targetColumn));

            // Delete target node.
            //
            GetLayoutSite(row, targetColumn).Node = null;

            return true;
        }

        private void SimplifyVertical()
        {
            for (var row = 0; row < RowCount; ++row)
            {
                for (var column = 0; column < ColumnCount; ++column)
                {
                    while (SimplifyDown(row, column))
                    {
                        // Keep expanding the cell to the right.
                    }
                }
            }
        }

        private bool SimplifyDown(int row, int column)
        {
            var sourceNode = GetRectangleShapeNode(row, column);
            if (sourceNode == null)
            {
                // We can only simplify rectangles.
                //
                return false;
            }

            var rowSpan = GetRowSpan(row, column);
            if (row + rowSpan >= RowCount)
            {
                // No more rows in layout.
                //
                return false;
            }

            var targetRow = row + rowSpan;
            var targetNode = GetRectangleShapeNode(targetRow, column);
            if (targetNode == null)
            {
                // Adjacent shape is not a rectangle.
                //
                return false;
            }

            if (sourceNode.Style != targetNode.Style)
            {
                // The two rectangles are different styles.
                //
                return false;
            }

            if (GetColumnSpan(row, column) != GetColumnSpan(targetRow, column))
            {
                // The two rectangles are different widths.
                //
                return false;
            }

            // All checks were successful.  We can merge the cells.

            // Add the target column span to the source.
            //
            SetRowSpan(row, column, rowSpan + GetRowSpan(targetRow, column));

            // Delete target node.
            //
            GetLayoutSite(targetRow, column).Node = null;

            return true;
        }

        private RectangleShapeNode GetRectangleShapeNode(int row, int column)
        {
            var layoutSite = GetLayoutSite(row, column);

            return layoutSite.Node != null
                ? layoutSite.Node as RectangleShapeNode 
                : null;
        }
    }
}