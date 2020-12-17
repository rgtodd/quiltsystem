//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace RichTodd.QuiltSystem.Design.Core
{
    public class KitBuildStepList : List<KitBuildStep>
    {
        public KitBuildStepList()
        { }

        public KitBuildStepList(JToken json)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));

            foreach (var jsonKitBuildStep in json)
            {
                Add(new KitBuildStep(jsonKitBuildStep));
            }
        }

        protected KitBuildStepList(IList<KitBuildStep> prototype)
        {
            if (prototype == null) throw new ArgumentNullException(nameof(prototype));

            foreach (var kitBuildStep in prototype)
            {
                Add(kitBuildStep.Clone());
            }
        }

        public KitBuildStepList Clone()
        {
            return new KitBuildStepList(this);
        }

        public JToken JsonSave()
        {
            var jsonKitBuildSteps = new JArray();
            foreach (var kitBuildStep in this)
            {
                jsonKitBuildSteps.Add(kitBuildStep.JsonSave());
            }

            return jsonKitBuildSteps;
        }
    }
}