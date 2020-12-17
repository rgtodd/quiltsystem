//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Linq;

using RichTodd.QuiltSystem.Design.Core;

namespace RichTodd.QuiltSystem.Design.Build
{
    internal class Builder
    {
        private readonly Core.Design m_design;
        private readonly KitSpecification m_kitSpecification;
        private List<IBuildComponent> m_allComponents;
        private List<IBuildStep> m_allSteps;
        private List<IBuildComponent> m_pendingComponents;
        private List<IBuildStep> m_pendingSteps;

        public Builder(KitSpecification kitSpecification, Core.Design design)
        {
            m_kitSpecification = kitSpecification ?? throw new ArgumentNullException(nameof(kitSpecification));
            m_design = design ?? throw new ArgumentNullException(nameof(design));
        }

        public IBuildPlan Create()
        {
            m_allComponents = new List<IBuildComponent>();
            m_pendingComponents = new List<IBuildComponent>();

            m_allSteps = new List<IBuildStep>();
            m_pendingSteps = new List<IBuildStep>();

            CreateBuildPlan();

            SortBuildSteps();

            var buildPlan = new BuildPlan();
            int stepNumber = 0;
            foreach (BuildStep buildStep in m_allSteps)
            {
                buildStep.StepNumber = ++stepNumber;
                buildPlan.AddBuildStep(buildStep);
            }

            return buildPlan;
        }

        private void CreateBuildPlan()
        {
            var factory = new BuildComponentFactory();

            var quilt = factory.CreateBuildComponentQuilt(m_kitSpecification, m_design);
            PushPendingComponent(quilt);

            while (true)
            {
                // Go through all outstanding components and create build steps.
                //
                {
                    var component = PullPendingComponent();
                    while (component != null)
                    {
                        ProcessComponent(component, factory);

                        component = PullPendingComponent();
                    }
                }

                // Pull a build step and add inputs.
                //
                {
                    var step = PullPendingStep();
                    if (step == null)
                    {
                        // No more steps.  Processing complete.
                        return;
                    }

                    step.ComputeInputs(factory);
                    foreach (BuildComponent component in step.Consumes)
                    {
                        PushPendingComponent(component);
                    }
                }
            }
        }

        private IBuildStep FindCompletedBuildStep(List<IBuildStep> buildSteps, List<BuildComponent> completedBuildComponents)
        {
            foreach (var buildStep in buildSteps)
            {
                if (buildStep.Consumes.Count == 0)
                {
                    return buildStep;
                }
            }

            foreach (var buildStep in buildSteps)
            {
                bool isComplete = true; // assume success
                foreach (var buildComponent in buildStep.Consumes)
                {
                    if (!completedBuildComponents.Contains(buildComponent) && !(buildComponent is BuildComponentYardage))
                    {
                        isComplete = false;
                        break;
                    }
                }

                if (isComplete)
                {
                    return buildStep;
                }
            }

            return null;
        }

        private IBuildStep FindOrCreateStep(IBuildComponent component)
        {
            var styleKey = component.StyleKey;

            var result = m_pendingSteps.Where(r => r.CanProduceQuantity(styleKey) >= component.Quantity).FirstOrDefault();

            if (result == null)
            {
                result = m_pendingSteps.Where(r => r.CanProduceQuantity(styleKey) > 0).FirstOrDefault();
            }

            if (result == null)
            {
                var styleType = styleKey.Substring(0, styleKey.IndexOf(BuildComponent.StyleKeyDelimiter));

                switch (styleType)
                {
                    case nameof(BuildComponentQuilt):
                        result = new BuildStepAssembleQuilt(styleKey);
                        break;

                    case nameof(BuildComponentLayout):
                        result = new BuildStepAssembleLayout(styleKey);
                        break;

                    case nameof(BuildComponentFlyingGoose):
                        result = new BuildStepFlyingGoose(styleKey);
                        break;

                    case nameof(BuildComponentHalfSquareTriangle):
                        result = new BuildStepHalfSquareTriangle(styleKey);
                        break;

                    case nameof(BuildComponentRectangle):
                        result = new BuildStepCut(styleKey);
                        break;

                    case nameof(BuildComponentYardage):

                        // Source component - no build step required.
                        //
                        return null;

                    default:
                        throw new InvalidOperationException(string.Format("No build step for style type {0}.", styleType));
                }

                m_allSteps.Add(result);
                m_pendingSteps.Add(result);
            }

            return result;
        }

        private void ProcessComponent(IBuildComponent component, BuildComponentFactory factory)
        {
            var step = FindOrCreateStep(component);
            if (step != null)
            {
                int quantity = step.CanProduceQuantity(component.StyleKey);
                if (quantity < component.Quantity)
                {
                    var splitComponent = component.Split(factory, component.Quantity - quantity);
                    PushPendingComponent(splitComponent);
                }

                step.AddOutput(component);
            }
        }

        private IBuildComponent PullPendingComponent()
        {
            IBuildComponent result;

            if (m_pendingComponents.Count == 0)
            {
                return null;
            }

            result = PullPendingComponent(typeof(BuildComponentQuilt));
            if (result != null)
            {
                return result;
            }

            result = PullPendingComponent(typeof(BuildComponentLayout));
            if (result != null)
            {
                return result;
            }

            result = PullPendingComponent(typeof(BuildComponentFlyingGoose));
            if (result != null)
            {
                return result;
            }

            result = PullPendingComponent(typeof(BuildComponentHalfSquareTriangle));
            if (result != null)
            {
                return result;
            }

            result = PullPendingComponent(typeof(BuildComponentRectangle));
            if (result != null)
            {
                return result;
            }

            result = PullPendingComponent(typeof(BuildComponentYardage));

            return result ?? throw new InvalidOperationException("Pending components collection is not empty.");
        }

        private IBuildComponent PullPendingComponent(Type componentType)
        {
            var component = m_pendingComponents.Where(r => r.GetType() == componentType).FirstOrDefault();
            if (component == null)
            {
                return null;
            }

            _ = m_pendingComponents.Remove(component);

            return component;
        }

        private IBuildStep PullPendingStep(Type stepType)
        {
            var step = m_pendingSteps.Where(r => r.GetType() == stepType).FirstOrDefault();
            if (step == null)
            {
                return null;
            }

            _ = m_pendingSteps.Remove(step);

            return step;
        }

        private IBuildStep PullPendingStep()
        {
            IBuildStep result;

            if (m_pendingSteps.Count == 0)
            {
                return null;
            }

            result = PullPendingStep(typeof(BuildStepAssembleQuilt));
            if (result != null)
            {
                return result;
            }

            result = PullPendingStep(typeof(BuildStepAssembleLayout));
            if (result != null)
            {
                return result;
            }

            result = PullPendingStep(typeof(BuildStepFlyingGoose));
            if (result != null)
            {
                return result;
            }

            result = PullPendingStep(typeof(BuildStepHalfSquareTriangle));
            if (result != null)
            {
                return result;
            }

            result = PullPendingStep(typeof(BuildStepCut));
            
            return result ?? throw new InvalidOperationException("Pending steps collection is not empty.");
        }

        private void PushPendingComponent(IBuildComponent component)
        {
            m_allComponents.Add(component);
            m_pendingComponents.Add(component);
        }

        private void SortBuildSteps()
        {
            var pendingBuildSteps = new List<IBuildStep>(m_allSteps);
            var completedBuildComponents = new List<BuildComponent>();
            var sortedBuildSteps = new List<IBuildStep>();

            while (pendingBuildSteps.Count > 0)
            {
                var buildStep = FindCompletedBuildStep(pendingBuildSteps, completedBuildComponents);
                if (buildStep == null)
                {
                    throw new InvalidOperationException("Cannot sort build steps.");
                }

                _ = pendingBuildSteps.Remove(buildStep);

                sortedBuildSteps.Add(buildStep);

                foreach (var buildComponent in buildStep.Produces)
                {
                    completedBuildComponents.Add((BuildComponent)buildComponent);
                }
            }

            m_allSteps = sortedBuildSteps;
        }

#pragma warning disable IDE0051 // Remove unused private members
        private IBuildComponent Split(BuildComponentFactory factory, IBuildComponent component, int quantity)
#pragma warning restore IDE0051 // Remove unused private members
        {
            return component.Split(factory, quantity);
        }
    }
}