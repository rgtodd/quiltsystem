//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Web
{
    public class ActionData
    {
        private const string ActionPrefix = "action~";

        private readonly string[] m_fields;

        private ActionData(string[] fields)
        {
            m_fields = fields ?? throw new ArgumentNullException(nameof(fields));
        }

        public string ActionName
        {
            get
            {
                return m_fields.Length >= 2 ? m_fields[1] : null;
            }
        }

        public string ActionParameter
        {
            get
            {
                return m_fields.Length >= 3 ? m_fields[2] : null;
            }
        }

        public static bool HasActionData(string key)
        {
            return key.StartsWith(ActionPrefix);
        }

        public static ActionData Parse(string key)
        {
            var fields = key.Split(new char[] { '~' });
            return new ActionData(fields);
        }
    }
}