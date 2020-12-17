//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Linq;

namespace RichTodd.QuiltSystem.Business.Report
{
    public static class DbSetExtensions
    {

        public static ReportBuilder<TSource> CreateReport<TSource>(this IQueryable<TSource> queryable)
        {
            return new ReportBuilder<TSource>(queryable);
        }

    }

    public class ReportBuilder<TSource>
    {

        //private readonly IQueryable<TSource> m_queryable;

        public ReportBuilder(IQueryable<TSource> queryable)
        {
            //m_queryable = queryable;
        }

        public ReportBuilder<TSource> OrderBy<TKey>(Func<TSource, TKey> expression)
        {
            //m_expression = expression;

            //var del = m_expression.Compile();

            //var comparer = Comparer<TKey>.Default;

            return this;
        }

        public ReportBuilder<TSource> Sum(Func<TSource, int?> expression)
        {
            //var x = expression;
            return this;
        }

        public ReportBuilder<TSource> Sum(Func<TSource, decimal?> expression)
        {
            return this;
        }

    }
}