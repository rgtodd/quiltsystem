//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Text;

namespace RichTodd.QuiltSystem.Database.Builders
{
    public class TransactionDescriptionBuilder
    {
        private StringBuilder m_sbDescription;

        public void Append(string text)
        {
            if (m_sbDescription == null)
            {
                m_sbDescription = new StringBuilder();
            }
            else
            {
                _ = m_sbDescription.Append("|"); // HACK
            }
            _ = m_sbDescription.Append(text);
        }

        public override string ToString()
        {
            return m_sbDescription != null ? m_sbDescription.ToString() : string.Empty;
        }
    }
}
