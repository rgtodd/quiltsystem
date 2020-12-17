//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Micro.Implementations
{
    internal class CacheMicroService : ICacheMicroService
    {
        private IList<MInventory_LibraryEntry> m_cachedEntries;

        public IList<MInventory_LibraryEntry> GetCachedEntries()
        {
            return m_cachedEntries;
        }

        public void SetCachedEntries(IList<MInventory_LibraryEntry> entries)
        {
            m_cachedEntries = entries;
        }
    }
}
