//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using RichTodd.QuiltSystem.Design.Nodes;

namespace RichTodd.QuiltSystem.Design.Component
{
    public interface INodeFactory
    {
        Node Create(Component component, NodeList childNodes);
    }
}
