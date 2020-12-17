//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

namespace RichTodd.QuiltSystem.Business.Report
{
    public abstract class StandardReport<TRecord> : Report<TRecord> where TRecord : class
    {

        private const int LEVEL_NONE = 99;

        private readonly IList<Column> m_aggregateColumns;
        private readonly Aggregate[,] m_aggregates;
        private readonly IList<Column> m_dataColumns;
        private readonly IList<Column> m_groupColumns;

        protected StandardReport()
        {
            m_groupColumns = new List<Column>();
            m_dataColumns = new List<Column>();
            m_aggregateColumns = new List<Column>();

            OnDefineColumns();

            m_aggregates = new Aggregate[GroupColumns.Count + 1, AggregateColumns.Count];

            OnDefineAggregates();
        }

        protected IList<Column> AggregateColumns
        {
            get { return m_aggregateColumns; }
        }

#pragma warning disable CA1819 // Properties should not return arrays
        protected Aggregate[,] Aggregates
#pragma warning restore CA1819 // Properties should not return arrays
        {
            get { return m_aggregates; }
        }

        protected override int BreakLevelMax
        {
            get { return GroupColumns.Count; }
        }

        protected override int BreakLevelMin
        {
            get { return 0; }
        }

        protected IList<Column> DataColumns
        {
            get { return m_dataColumns; }
        }

        protected IList<Column> GroupColumns
        {
            get { return m_groupColumns; }
        }

        protected override void AccumulateTotals(TRecord record)
        {
            for (int idxAggregate = 0; idxAggregate < AggregateColumns.Count; ++idxAggregate)
            {
                var amount = AggregateColumns[idxAggregate].GetValue(record);

                for (int breakLevel = 0; breakLevel < GroupColumns.Count + 1; ++breakLevel)
                {
                    Aggregates[breakLevel, idxAggregate].Apply(amount);
                }
            }
        }

        protected override int GetBreakLevel(TRecord record, TRecord previousRecord)
        {
            if (previousRecord == null)
            {
                return LEVEL_NONE;
            }

            for (int idxGroup = 0; idxGroup < GroupColumns.Count; ++idxGroup)
            {
                var column = GroupColumns[idxGroup];
                IComparable currentValue = column.GetValue(record);
                IComparable previousValue = column.GetValue(previousRecord);

                if (currentValue.CompareTo(previousValue) != 0)
                {
                    return idxGroup + 1;
                }
            }

            return LEVEL_NONE;
        }

        protected abstract void OnDefineAggregates();

        protected abstract void OnDefineColumns();

        protected override void WriteHeading(IReportWriter wtr)
        {
            foreach (var column in GroupColumns)
            {
                wtr.DefineColumn(column.Name);
            }

            foreach (var column in DataColumns)
            {
                wtr.DefineColumn(column.Name);
            }

            foreach (var column in AggregateColumns)
            {
                wtr.DefineColumn(column.Name);
            }
        }

        protected override void WriteRecord(IReportWriter wtr, TRecord record, TRecord previousRecord, int breakLevel)
        {
            for (int idxGroup = 0; idxGroup < GroupColumns.Count; ++idxGroup)
            {
                var column = GroupColumns[idxGroup];

                if (previousRecord == null || idxGroup >= breakLevel - 1)
                {
                    wtr.WriteCellTotal(column.GetFormattedValue(record), 1);
                }
                else
                {
                    wtr.WriteEmptyCell(1);
                }
            }

            foreach (var column in DataColumns)
            {
                wtr.WriteCell(column.GetFormattedValue(record), 1);
            }

            foreach (var column in AggregateColumns)
            {
                wtr.WriteCell(column.GetFormattedValue(record), 1);
            }
        }

        protected override void WriteTotals(IReportWriter wtr, int breakLevel)
        {
            if (breakLevel > 0)
            {
                wtr.WriteEmptyCell(breakLevel);
            }
            var colSpan = GroupColumns.Count + DataColumns.Count - breakLevel;
            if (colSpan > 0)
            {
                wtr.WriteCellTotal("Total", colSpan);
            }

            for (int idxAggregate = 0; idxAggregate < AggregateColumns.Count; ++idxAggregate)
            {
                wtr.WriteCellTotal(Aggregates[breakLevel, idxAggregate].GetFormattedValue(), 1);
            }

            ClearSubtotals(breakLevel);
        }

        private void ClearSubtotals(int breakLevel)
        {
            for (int idxAggregate = 0; idxAggregate < AggregateColumns.Count; ++idxAggregate)
            {
                Aggregates[breakLevel, idxAggregate].Reset();
            }
        }

        #region Protected Classes

        protected class Column
        {

            private Func<TRecord, IComparable> m_fieldAccessor;
            private string m_format;
            private string m_name;

            public Func<TRecord, IComparable> FieldAccessor
            {
                get { return m_fieldAccessor; }
                set { m_fieldAccessor = value; }
            }

            public string Format
            {
                get { return m_format; }
                set { m_format = value; }
            }

            public string Name
            {
                get { return m_name; }
                set { m_name = value; }
            }

            public string GetFormattedValue(TRecord record)
            {
                if (m_format != null)
                {
                    return string.Format(m_format, GetValue(record));
                }
                else
                {
                    return string.Format("{0}", GetValue(record));
                }
            }

            public IComparable GetValue(TRecord record)
            {
                return FieldAccessor(record);
            }

        }

        #endregion Protected Classes
    }
}