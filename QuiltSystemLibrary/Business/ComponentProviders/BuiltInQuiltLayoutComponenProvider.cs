//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

using RichTodd.QuiltSystem.Design.Component;
using RichTodd.QuiltSystem.Design.Component.Standard;

namespace RichTodd.QuiltSystem.Business.ComponentProviders
{
    public class BuiltInQuiltLayoutComponenProvider : IComponentProvider
    {
        public const string ComponentName_Checkerboard = "Quilt Layout";
        public const string ComponentName_HorizontalStripes1 = "Stripes";
        public const string ComponentName_HorizontalStripes2 = "Stripes2";
        public const string ComponentName_HorizontalStripes3 = "Stripes3";
        public const string ComponentName_HorizontalStripes4 = "Stripes4";
        public const string ComponentName_HorizontalStripes5 = "Stripes5";
        public const string ComponentName_Radial1 = "Radial1";
        public const string ComponentName_Radial2 = "Radial2";
        public const string ComponentName_VerticalStripes = "Vertical Stripes";

        public ComponentProviderEntry GetComponent(string type, string category, string name)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (category == null) throw new ArgumentNullException(nameof(category));
            if (name == null) throw new ArgumentNullException(nameof(name));

            if (type == LayoutComponent.TypeName)
            {
                if (category == Constants.DefaultComponentCategory)
                {
                    switch (name)
                    {
                        case ComponentName_Checkerboard: return CreateCheckerboard();
                        case ComponentName_HorizontalStripes1: return CreateHorizontalStripes1();
                        case ComponentName_HorizontalStripes2: return CreateHorizontalStripes2();
                        case ComponentName_HorizontalStripes3: return CreateHorizontalStripes3();
                        case ComponentName_HorizontalStripes4: return CreateHorizontalStripes4();
                        case ComponentName_HorizontalStripes5: return CreateHorizontalStripes5();
                        case ComponentName_VerticalStripes: return CreateVerticalStripes();
                        case ComponentName_Radial1: return CreateRadial1();
                        case ComponentName_Radial2: return CreateRadial2();
                    }
                }
            }

            return null;
        }

        public List<ComponentProviderEntry> GetComponents(string type, string category)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (category == null) throw new ArgumentNullException(nameof(category));

            var result = new List<ComponentProviderEntry>();

            if (type == LayoutComponent.TypeName)
            {
                if (category == Constants.DefaultComponentCategory)
                {
                    result.Add(CreateCheckerboard());
                    result.Add(CreateHorizontalStripes1());
                    result.Add(CreateHorizontalStripes2());
                    result.Add(CreateHorizontalStripes3());
                    result.Add(CreateHorizontalStripes4());
                    result.Add(CreateHorizontalStripes5());
                    result.Add(CreateVerticalStripes());
                    result.Add(CreateRadial1());
                    result.Add(CreateRadial2());
                }
            }

            return result;
        }

        private static ComponentProviderEntry CreateCheckerboard()
        {
            return new ComponentProviderEntry()
            {
                Type = LayoutComponent.TypeName,
                Category = Constants.DefaultComponentCategory,
                Name = ComponentName_Checkerboard,
                BlockCount = 2
            };
        }

        private static ComponentProviderEntry CreateHorizontalStripes1()
        {
            return new ComponentProviderEntry()
            {
                Type = LayoutComponent.TypeName,
                Category = Constants.DefaultComponentCategory,
                Name = ComponentName_HorizontalStripes1,
                BlockCount = 2
            };
        }

        private static ComponentProviderEntry CreateHorizontalStripes2()
        {
            return new ComponentProviderEntry()
            {
                Type = LayoutComponent.TypeName,
                Category = Constants.DefaultComponentCategory,
                Name = ComponentName_HorizontalStripes2,
                BlockCount = 3
            };
        }

        private static ComponentProviderEntry CreateHorizontalStripes3()
        {
            return new ComponentProviderEntry()
            {
                Type = LayoutComponent.TypeName,
                Category = Constants.DefaultComponentCategory,
                Name = ComponentName_HorizontalStripes3,
                BlockCount = 3
            };
        }

        private static ComponentProviderEntry CreateHorizontalStripes4()
        {
            return new ComponentProviderEntry()
            {
                Type = LayoutComponent.TypeName,
                Category = Constants.DefaultComponentCategory,
                Name = ComponentName_HorizontalStripes4,
                BlockCount = 3
            };
        }

        private static ComponentProviderEntry CreateHorizontalStripes5()
        {
            return new ComponentProviderEntry()
            {
                Type = LayoutComponent.TypeName,
                Category = Constants.DefaultComponentCategory,
                Name = ComponentName_HorizontalStripes5,
                BlockCount = 3
            };
        }

        private static ComponentProviderEntry CreateRadial1()
        {
            return new ComponentProviderEntry()
            {
                Type = LayoutComponent.TypeName,
                Category = Constants.DefaultComponentCategory,
                Name = ComponentName_Radial1,
                BlockCount = 2
            };
        }

        private static ComponentProviderEntry CreateRadial2()
        {
            return new ComponentProviderEntry()
            {
                Type = LayoutComponent.TypeName,
                Category = Constants.DefaultComponentCategory,
                Name = ComponentName_Radial2,
                BlockCount = 2
            };
        }

        private static ComponentProviderEntry CreateVerticalStripes()
        {
            return new ComponentProviderEntry()
            {
                Type = LayoutComponent.TypeName,
                Category = Constants.DefaultComponentCategory,
                Name = ComponentName_VerticalStripes,
                BlockCount = 2
            };
        }
    }
}