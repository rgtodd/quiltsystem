//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;
using System.Linq;

using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Build
{
    internal class CutStockFactory : ICutStockFactory
    {
        private readonly List<CutStock> m_unusedCutStocks;
        private readonly List<CutStock> m_usedCutStocks;

        public CutStockFactory(IEnumerable<CutStock> cutStocks)
        {
            m_unusedCutStocks = new List<CutStock>(cutStocks);
            m_usedCutStocks = new List<CutStock>();
        }

        public IReadOnlyList<CutStock> UnusedCutStocks
        {
            get
            {
                return m_unusedCutStocks;
            }
        }

        public IReadOnlyList<CutStock> UsedCutStocks
        {
            get
            {
                return m_usedCutStocks;
            }
        }

        public CutStock GetCutStock(Area area)
        {
            var cutStock = m_unusedCutStocks.OrderByDescending(r => r.Area.Width * r.Area.Height).FirstOrDefault();

            if (cutStock != null)
            {
                m_unusedCutStocks.Remove(cutStock);
                m_usedCutStocks.Add(cutStock);
            }

            return cutStock;
        }
    }
}