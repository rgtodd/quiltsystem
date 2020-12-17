//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Build
{
    internal class UnlimitedCutStockFactory : ICutStockFactory
    {
        public CutStock GetCutStock(Area area)
        {
            var standardArea = Area.GetSmallestContainingStandardArea(area);

            var cutStock = new CutStock(standardArea);

            return cutStock;
        }
    }
}