//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

namespace RichTodd.QuiltSystem.Design.Build
{
    internal class CutPlan
    {
        private readonly List<CutRegion> m_cutRegions = new List<CutRegion>();
        private readonly List<CutStock> m_cutStocks = new List<CutStock>();

        public List<CutRegion> CutRegions
        {
            get
            {
                return m_cutRegions;
            }
        }

        public List<CutStock> CutStocks
        {
            get
            {
                return m_cutStocks;
            }
        }
    }
}