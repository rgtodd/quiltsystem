//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Linq.Expressions;

namespace RichTodd.QuiltSystem.Business.Report
{
    public abstract class Aggregate
    {
        public abstract void Apply(object value);

        public abstract string GetFormattedValue();

        public abstract void Reset();
    }

    public class CountAggregate : Aggregate
    {
        private int m_count;

        public override void Apply(object value)
        {
            m_count += 1;
        }

        public override string GetFormattedValue()
        {
            return string.Format("{0}", m_count);
        }

        public override void Reset()
        {
            m_count = 0;
        }

        public override string ToString()
        {
            return m_count.ToString();
        }
    }

    public class DecimalSumAggregate : SumAggregate<decimal>
    {
        public override string GetFormattedValue()
        {
            return string.Format("{0:c}", m_sum);
        }
    }

    public class IntegerSumAggregate : SumAggregate<int>
    { }

    public class SumAggregate<T> : Aggregate
    {
        protected T m_sum;

        private static readonly Func<T, T, T> s_function = GetSumAggregateFunction();

        private static Func<T, T, T> GetSumAggregateFunction()
        {
            var lhs = Expression.Parameter(typeof(T));
            var rhs = Expression.Parameter(typeof(T));

            var function = (Func<T, T, T>)Expression
                .Lambda(Expression.Add(lhs, rhs), lhs, rhs)
                .Compile();

            return function;
        }

        public override void Apply(object value)
        {
            m_sum = s_function(m_sum, (T)value);
        }

        public override string GetFormattedValue()
        {
            return string.Format("{0}", m_sum);
        }

        public override void Reset()
        {
            m_sum = default;
        }

        public override string ToString()
        {
            return m_sum.ToString();
        }
    }
}