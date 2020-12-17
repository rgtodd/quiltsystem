//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Design.Path
{
    struct PathLocation
    {
        private int m_pathSegmentIndex;
        private double m_offset;

        public int PathSegmentIndex
        {
            get
            {
                return m_pathSegmentIndex;
            }

            set
            {
                m_pathSegmentIndex = value;
            }
        }

        public double Offset
        {
            get
            {
                return m_offset;
            }

            set
            {
                m_offset = value;
            }
        }
    }
}
