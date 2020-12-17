//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Web.Paging
{
    public class PagingState
    {
        public const int PageSize = 10;
        public const int PageSizeLarge = 100;
        public const int PageSizeHuge = 1000;

        private readonly bool m_descending;
        private readonly string m_filter;
        private readonly int? m_page;
        private readonly string m_sort;

        public PagingState(string sort, bool descending, int? page, string filter)
        {
            m_sort = sort;
            m_descending = descending;
            m_page = page;
            m_filter = filter;
        }

        public bool Descending
        {
            get { return m_descending; }
        }

        public string Filter
        {
            get { return m_filter; }
        }

        public int? Page
        {
            get { return m_page; }
        }

        public string Sort
        {
            get { return m_sort; }
        }
    }
}