//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Core
{
    public class DesignSize
    {
        #region Members

        private readonly string m_id;
        private readonly Dimension m_width;
        private readonly Dimension m_height;
        private readonly string m_description;
        private readonly bool m_preferred;

        #endregion

        public DesignSize(string id, Dimension width, Dimension height, string description, bool preferred)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(description));

            m_id = id;
            m_width = width;
            m_height = height;
            m_description = description;
            m_preferred = preferred;
        }

        public string Id
        {
            get
            {
                return m_id;
            }
        }

        public Dimension Width
        {
            get
            {
                return m_width;
            }
        }

        public Dimension Height
        {
            get
            {
                return m_height;
            }
        }

        public string Description
        {
            get
            {
                return m_description;
            }
        }

        public bool Preferred
        {
            get
            {
                return m_preferred;
            }
        }
    }
}
