//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

namespace RichTodd.QuiltSystem.Design.Nodes.Generator
{
    internal class PatchMapper<T> where T : Enum
    {
        private static PatchMapper<T> s_identityMapper;

        private readonly IDictionary<T, T> m_mappings;

        public PatchMapper(IDictionary<T, T> mappings)
        {
            m_mappings = mappings;
        }

        public static PatchMapper<T> IdentityMapper
        {
            get
            {
                if (s_identityMapper == null)
                {
                    var mappings = new Dictionary<T, T>();
                    foreach (T item in Enum.GetValues(typeof(T)))
                    {
                        mappings.Add(item, item);
                    }

                    s_identityMapper = new PatchMapper<T>(mappings);
                }

                return s_identityMapper;
            }
        }

        public T Map(T patch)
        {
            return m_mappings.ContainsKey(patch)
                ? m_mappings[patch]
                : patch;
        }

        public PatchMapper<T> Then(PatchMapper<T> then)
        {
            var mappings = new Dictionary<T, T>();
            foreach (T item in Enum.GetValues(typeof(T)))
            {
                mappings.Add(item, then.Map(Map(item)));
            }

            return new PatchMapper<T>(mappings);
        }
    }
}