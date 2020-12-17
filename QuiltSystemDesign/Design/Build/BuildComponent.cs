//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

using RichTodd.QuiltSystem.Design.Nodes;
using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Build
{
    internal abstract class BuildComponent : IBuildComponent
    {
        public static char StyleKeyDelimiter = '|';

        protected static FabricStyle[] s_emptyFabricStyleList = Array.Empty<FabricStyle>();

        private readonly string m_id;
        private IBuildStep m_consumedBy;
        private IBuildStep m_producedBy;
        private int m_quantity = 1;

        public BuildComponent(string id)
        {
            m_id = id;
        }

        public abstract Area Area { get; }

        public abstract string ComponentSubtype { get; }

        public abstract string ComponentType { get; }

        public IBuildStep ConsumedBy
        {
            get
            {
                return m_consumedBy;
            }
            set
            {
                if (m_consumedBy != null)
                {
                    throw new InvalidOperationException("ConsumedBy already set.");
                }

                m_consumedBy = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        public abstract IReadOnlyList<FabricStyle> FabricStyles { get; }

        public string Id
        {
            get { return m_id; }
        }

        public abstract Node Node { get; }

        public IBuildStep ProducedBy
        {
            get
            {
                return m_producedBy;
            }
            set
            {
                if (m_producedBy != null)
                {
                    throw new InvalidOperationException("ProducedBy already set.");
                }

                m_producedBy = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        public int Quantity
        {
            get
            {
                return m_quantity;
            }
            set
            {
                m_quantity = value;
            }
        }

        public abstract string StyleKey { get; }

        public IBuildComponent Split(BuildComponentFactory factory, int quantity)
        {
            if (quantity >= Quantity)
            {
                throw new ArgumentException(string.Format("Value exceeds component quantity of {0}.", Quantity), nameof(quantity));
            }
            if (ProducedBy != null)
            {
                throw new InvalidOperationException("Component is already produced by a build step.");
            }

            Quantity -= quantity;

            var component = Clone(factory);
            component.Quantity = quantity;

            if (ConsumedBy != null)
            {
                ConsumedBy.AddInput(component);
            }

            return component;
        }

        protected abstract IBuildComponent Clone(BuildComponentFactory factory);
    }
}