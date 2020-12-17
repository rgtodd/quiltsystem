//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Design.Primitives
{
    public static class Themes
    {
        private static Theme s_rainbow;

        public static Theme Rainbow
        {
            get
            {
                if (s_rainbow == null)
                {
                    s_rainbow = new Theme("Rainbow")
                    {
                        Entries = {
                            new ThemeEntry(Palettes.Rainbow)
                        }
                    };
                }

                return s_rainbow;
            }
        }
    }
}
