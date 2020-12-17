//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Database.Builders
{
    public class UnitOfWork
    {
        private readonly string m_root;
        private int m_index;

        public UnitOfWork(string root)
        {
            m_root = root;
        }

        public string Next()
        {
            m_index += 1;

            return $"{m_root}.{m_index:00}";
        }

        public static string GetRoot(string value)
        {
            var idx = value.IndexOf('.');
            return idx != -1
                ? value.Substring(0, value.IndexOf('.') + 1)
                : value + ".";
        }
    }
}
