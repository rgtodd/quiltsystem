//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

using RichTodd.QuiltSystem.Design.Nodes;
using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Build
{
    internal interface IBuildComponent
    {
        string Id { get; }
        string ComponentType { get; }
        string ComponentSubtype { get; }
        string StyleKey { get; }

        Area Area { get; }
        Node Node { get; }
        IReadOnlyList<FabricStyle> FabricStyles { get; }

        IBuildStep ProducedBy { get; set; }
        IBuildStep ConsumedBy { get; set; }
        int Quantity { get; set; }

        IBuildComponent Split(BuildComponentFactory factory, int quantity);
    }
}