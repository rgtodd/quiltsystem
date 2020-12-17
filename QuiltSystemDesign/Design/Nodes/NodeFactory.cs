//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace RichTodd.QuiltSystem.Design.Nodes
{
    public class NodeFactory
    {
        private static readonly NodeFactory s_singleton = new NodeFactory();

        private readonly IList<NodeConstructor> m_constructors;

        private NodeFactory()
        {
            m_constructors = new List<NodeConstructor>();
            foreach (var type in GetTypesWith<NodeAttribute>(false))
            {
                m_constructors.Add(new NodeConstructor(type));
            }
        }

        public static NodeFactory Singleton
        {
            get
            {
                return s_singleton;
            }
        }

        public IList<NodeConstructor> Constructors
        {
            get
            {
                return m_constructors;
            }
        }

        public Node Create(JToken json)
        {
            var typeName = (string)json[JsonNames.TypeName];
            return Lookup(typeName).Create(json);
        }

        private static IEnumerable<Type> GetTypesWith<TAttribute>(bool inherit)
            where TAttribute : Attribute
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] types;
                try
                {
                    types = assembly.GetTypes();
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception)
                {
                    types = null;
                }
#pragma warning restore CA1031 // Do not catch general exception types

                if (types != null)
                {
                    foreach (var type in types)
                    {
                        if (type.IsDefined(typeof(TAttribute), inherit))
                        {
                            yield return type;
                        }
                    }
                }
            }
        }

        private NodeConstructor Lookup(string typeName)
        {
            foreach (var factory in m_constructors)
            {
                if (factory.TypeName == typeName)
                {
                    return factory;
                }
            }

            return null;
        }
    }
}