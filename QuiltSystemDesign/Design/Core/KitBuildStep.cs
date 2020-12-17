//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

using Newtonsoft.Json.Linq;

using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Core
{
    public class KitBuildStep
    {
        private readonly KitBuildItemList m_consumes;
        private readonly string m_description;
        private readonly KitBuildItemList m_produces;
        private readonly int m_stepNumber;

        public KitBuildStep(int stepNumber, string description)
        {
            if (string.IsNullOrEmpty(description)) throw new ArgumentNullException(nameof(description));

            m_stepNumber = stepNumber;
            m_description = description;

            m_consumes = new KitBuildItemList();
            m_produces = new KitBuildItemList();
        }

        public KitBuildStep(JToken json)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));

            m_stepNumber = json.Value<int?>(JsonNames.StepNumber) ?? 0;
            m_description = (string)json[JsonNames.Description];

            var jsonConsumes = json[JsonNames.Consumes];
            if (jsonConsumes != null)
            {
                m_consumes = new KitBuildItemList(jsonConsumes);
            }
            else
            {
                m_consumes = new KitBuildItemList();
            }

            var jsonProduces = json[JsonNames.Produces];
            if (jsonProduces != null)
            {
                m_produces = new KitBuildItemList(jsonProduces);
            }
            else
            {
                m_produces = new KitBuildItemList();
            }
        }

        protected KitBuildStep(KitBuildStep prototype)
        {
            if (prototype == null) throw new ArgumentNullException(nameof(prototype));

            m_stepNumber = prototype.m_stepNumber;
            m_description = prototype.m_description;

            m_consumes = prototype.m_consumes.Clone();
            m_produces = prototype.m_produces.Clone();
        }

        public KitBuildItemList Consumes
        {
            get
            {
                return m_consumes;
            }
        }

        public string Description
        {
            get
            {
                return m_description;
            }
        }

        public KitBuildItemList Produces
        {
            get
            {
                return m_produces;
            }
        }

        public int StepNumber
        {
            get
            {
                return m_stepNumber;
            }
        }

        public KitBuildStep Clone()
        {
            return new KitBuildStep(this);
        }

        public JToken JsonSave()
        {
            var result = new JObject()
            {
                new JProperty(JsonNames.StepNumber, m_stepNumber),
                new JProperty(JsonNames.Description, m_description),
                new JProperty(JsonNames.Consumes, m_consumes.JsonSave()),
                new JProperty(JsonNames.Produces, m_produces.JsonSave())
            };

            return result;
        }

        public Dimension GetMaximumBuildItemWidth()
        {
            var result = new Dimension(0, DimensionUnits.Inch);

            foreach (var kitBuildItem in Consumes)
            {
                if (result.Value == 0)
                {
                    result = kitBuildItem.Area.Width;
                }
                else
                {
                    if (kitBuildItem.Area.Width.Unit != result.Unit)
                    {
                        throw new InvalidOperationException("Unit mismatch.");
                    }
                    result = new Dimension(Math.Max(kitBuildItem.Area.Width.Value, result.Value), result.Unit);
                }
            }

            foreach (var kitBuildItem in Produces)
            {
                if (result.Value == 0)
                {
                    result = kitBuildItem.Area.Width;
                }
                else
                {
                    if (kitBuildItem.Area.Width.Unit != result.Unit)
                    {
                        throw new InvalidOperationException("Unit mismatch.");
                    }
                    result = new Dimension(Math.Max(kitBuildItem.Area.Width.Value, result.Value), result.Unit);
                }
            }

            return result;
        }
    }
}