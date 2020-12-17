//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

using Newtonsoft.Json.Linq;

using RichTodd.QuiltSystem.Design.Path;
using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Core
{
    public class PatternElement
    {
        private readonly string m_id;
        private PathPoint m_source;
        private PathPoint m_target;
        private PatternElementTypes m_type;

        public PatternElement(PatternElementTypes type, PathPoint source, PathPoint target, string id)
        {
            m_type = type;
            m_source = source;
            m_target = target;
            m_id = id;
        }

        public PatternElement(JToken json)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));

            m_type = (PatternElementTypes)Enum.Parse(typeof(PatternElementTypes), (string)json[JsonNames.PatternElementType]);
            m_source = new PathPoint(json[JsonNames.Source]);
            m_target = new PathPoint(json[JsonNames.Target]);
            m_id = json.Value<string>(JsonNames.Id);
        }

        protected PatternElement(PatternElement prototype)
        {
            if (prototype == null) throw new ArgumentNullException(nameof(prototype));

            m_type = prototype.m_type;
            m_source = prototype.m_source;
            m_target = prototype.m_target;
            m_id = prototype.m_id;
        }

        public Dimension Height
        {
            get
            {
                return Target.Y - Source.Y;
            }
        }

        public string Id
        {
            get
            {
                return m_id;
            }
        }

        public PathPoint Source
        {
            get
            {
                return m_source;
            }
            set
            {
                m_source = value;
            }
        }

        public PathPoint Target
        {
            get
            {
                return m_target;
            }
            set
            {
                m_target = value;
            }
        }

        public PatternElementTypes Type
        {
            get
            {
                return m_type;
            }
            set
            {
                m_type = value;
            }
        }

        public Dimension Width
        {
            get
            {
                return Target.X - Source.X;
            }
        }

        public PatternElement Clone()
        {
            return new PatternElement(this);
        }

        public JToken JsonSave()
        {
            var result = new JObject()
            {
                new JProperty(JsonNames.PatternElementType, m_type.ToString()),
                new JProperty(JsonNames.Source, m_source.JsonSave()),
                new JProperty(JsonNames.Target, m_target.JsonSave()),
                new JProperty(JsonNames.Id, m_id)
            };

            return result;
        }
    }
}