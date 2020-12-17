//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace RichTodd.QuiltSystem.Business.Report
{
    public class HtmlReportWriter : IReportWriter
    {
        //private const string TableClass = "report";
        //private const string TableDataTotalClass = "report__total";

        // Report definition fields.

        private int m_columnCount = 0;
        private readonly IList<Column> m_columns = new List<Column>();

        // Report data fields.

        private IList<Cell> m_effectiveCells;
        private IList<Cell> m_previousEffectiveCells;
        //private Row m_previousRow;
        private Row m_row;
        private readonly IList<Row> m_rows = new List<Row>();

        public void DefineColumn(string headingText)
        {
            var column = new Column()
            {
                HeadingText = headingText
            };

            m_columns.Add(column);
            m_columnCount += 1;
        }

        public void RenderHtmlTable(TextWriter writer)
        {
            // HACK: Migrate code.
            //using (var htmlWriter = new HtmlTextWriter(writer))
            //{
            //    htmlWriter.BeginRender();

            //    htmlWriter.AddAttribute(HtmlTextWriterAttribute.Border, "1");
            //    htmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, TableClass);
            //    htmlWriter.RenderBeginTag(HtmlTextWriterTag.Table);
            //    {
            //        htmlWriter.RenderBeginTag(HtmlTextWriterTag.Thead);
            //        {
            //            htmlWriter.RenderBeginTag(HtmlTextWriterTag.Tr);
            //            {
            //                foreach (var column in m_columns)
            //                {
            //                    htmlWriter.RenderBeginTag(HtmlTextWriterTag.Th);
            //                    htmlWriter.Write(column.HeadingText);
            //                    htmlWriter.RenderEndTag(); // Th
            //                }
            //            }
            //            htmlWriter.RenderEndTag(); // Tr
            //        }
            //        htmlWriter.RenderEndTag(); // Thead

            //        htmlWriter.RenderBeginTag(HtmlTextWriterTag.Tbody);
            //        {
            //            foreach (var row in m_rows)
            //            {
            //                htmlWriter.RenderBeginTag(HtmlTextWriterTag.Tr);
            //                foreach (var cell in row.Cells.Where(c => c.IsPlaceholder == false))
            //                {
            //                    if (cell.ColSpan > 1)
            //                    {
            //                        htmlWriter.AddAttribute(HtmlTextWriterAttribute.Colspan, cell.ColSpan.ToString());
            //                    }
            //                    if (cell.RowSpan > 1)
            //                    {
            //                        htmlWriter.AddAttribute(HtmlTextWriterAttribute.Rowspan, cell.RowSpan.ToString());
            //                    }
            //                    if (cell.IsTotal)
            //                    {
            //                        htmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, TableDataTotalClass);
            //                    }
            //                    htmlWriter.RenderBeginTag(HtmlTextWriterTag.Td);
            //                    htmlWriter.Write(cell.Value);
            //                    htmlWriter.RenderEndTag(); // Td
            //                }

            //                htmlWriter.RenderEndTag(); // Tr
            //            }
            //        }
            //        htmlWriter.RenderEndTag(); // Tbody
            //    }
            //    htmlWriter.RenderEndTag(); // Table

            //    htmlWriter.EndRender();
            //}
        }

        public void WriteCell(string value, int colSpan)
        {
            OnBeginWriteCell();

            var cell = m_row.AddCell(colSpan, value);

            m_effectiveCells.Add(cell);

            OnEndWriteCell();
        }

        public void WriteCellTotal(string value, int colSpan)
        {
            OnBeginWriteCell();

            var cell = m_row.AddCell(colSpan, value, isTotal: true);

            m_effectiveCells.Add(cell);

            OnEndWriteCell();
        }

        public void WriteEmptyCell(int colSpan)
        {
            OnBeginWriteCell();

            if (m_previousEffectiveCells == null)
            {
                var cell = m_row.AddCell(colSpan, null);

                m_effectiveCells.Add(cell);
            }
            else
            {
                var nextFromColumnIndex = m_row.NextColumnIndex;
                var nextToColumnIndex = nextFromColumnIndex + colSpan - 1;

                var previousCells = m_previousEffectiveCells.Where(c => c.FromColumnIndex >= nextFromColumnIndex && c.ToColumnIndex <= nextToColumnIndex);

                if (previousCells.Count() == 0)
                {
                    var cell = m_row.AddCell(colSpan, "");

                    m_effectiveCells.Add(cell);
                }
                else
                {
                    var minColumnIndex = previousCells.Min(c => c.FromColumnIndex);
                    var maxColumnIndex = previousCells.Max(c => c.ToColumnIndex);

                    if (minColumnIndex > nextFromColumnIndex)
                    {
                        var cell = m_row.AddCell(minColumnIndex - nextFromColumnIndex, "");

                        m_effectiveCells.Add(cell);
                    }

                    {
                        var cell = m_row.AddCell(maxColumnIndex - minColumnIndex + 1, null, isPlaceHolder: true);

                        foreach (var previousCell in previousCells)
                        {
                            Debug.Assert(previousCell.IsPlaceholder == false);

                            previousCell.RowSpan += 1;

                            m_effectiveCells.Add(previousCell);
                        }
                    }

                    if (maxColumnIndex < nextToColumnIndex)
                    {
                        var cell = m_row.AddCell(nextToColumnIndex - maxColumnIndex, "");

                        m_effectiveCells.Add(cell);
                    }
                }
            }

            OnEndWriteCell();
        }

        private void OnBeginWriteCell()
        {
            // Start new row.
            //
            if (m_row == null)
            {
                Debug.Assert(m_effectiveCells == null);

                m_row = new Row(m_columnCount);
                m_effectiveCells = new List<Cell>();

                m_rows.Add(m_row);
            }
        }

        private void OnEndWriteCell()
        {
            // Complete row.
            //
            if (m_row.IsFull)
            {
                //m_previousRow = m_row;
                m_previousEffectiveCells = m_effectiveCells;

                m_row = null;
                m_effectiveCells = null;
            }
        }

        private class Cell
        {

            private readonly int m_fromColumnIndex;
            private readonly int m_toColumnIndex;

            public Cell(int fromColumnIndex, int toColumnIndex)
            {
                Debug.Assert(fromColumnIndex >= 0);
                Debug.Assert(toColumnIndex >= fromColumnIndex);

                m_fromColumnIndex = fromColumnIndex;
                m_toColumnIndex = toColumnIndex;
            }

            public int ColSpan
            {
                get { return m_toColumnIndex - m_fromColumnIndex + 1; }
            }

            public int FromColumnIndex
            {
                get { return m_fromColumnIndex; }
            }

            public bool IsPlaceholder { get; set; }

            public bool IsTotal { get; set; }

            public int RowSpan { get; set; }

            public int ToColumnIndex
            {
                get { return m_toColumnIndex; }
            }

            public string Value { get; set; }

        }

        private class Column
        {

            public string HeadingText { get; set; }

        }

        private class Row
        {

            private readonly int m_maxColumnIndex;
            private readonly List<Cell> m_cells = new List<Cell>();

            public Row(int columnCount)
            {
                Debug.Assert(columnCount > 0);

                m_maxColumnIndex = columnCount - 1;
            }

            public IReadOnlyList<Cell> Cells
            {
                get { return m_cells; }
            }

            public bool IsFull
            {
                get
                {
                    return m_cells.Count != 0
                        ? m_cells.Last().ToColumnIndex == m_maxColumnIndex
                        : false;
                }
            }

            public int NextColumnIndex
            {
                get
                {
                    if (m_cells.Count == 0)
                    {
                        return 0;
                    }

                    var nextColumnIndex = m_cells.Last().ToColumnIndex + 1;

                    Debug.Assert(nextColumnIndex <= m_maxColumnIndex);

                    return nextColumnIndex;
                }
            }

            public Cell AddCell(int colSpan, string value, bool isPlaceHolder = false, bool isTotal = false)
            {
                var lastColumnIndex = m_cells.Count > 0
                    ? m_cells.Last().ToColumnIndex
                    : -1;

                Debug.Assert(lastColumnIndex + colSpan <= m_maxColumnIndex);

                var fromColumnIndex = lastColumnIndex + 1;
                var toColumnIndex = fromColumnIndex + colSpan - 1;

                var cell = new Cell(fromColumnIndex, toColumnIndex)
                {
                    Value = value,
                    RowSpan = 1,
                    IsPlaceholder = isPlaceHolder,
                    IsTotal = isTotal
                };
                m_cells.Add(cell);

                return cell;
            }

            public Cell CellAtColumnIndex(int columnIndex)
            {
                return m_cells.Where(c => c.FromColumnIndex >= columnIndex && c.ToColumnIndex <= columnIndex).SingleOrDefault();
            }

        }
    }
}