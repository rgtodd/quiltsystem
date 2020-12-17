//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Design.Primitives
{
    public static class Palettes
    {
        private const int COUNT = 7;

        private static Palette s_blue;
        private static Palette s_green;
        private static Palette s_indigo;
        private static Palette s_orange;
        private static Palette s_rainbow;
        private static Palette s_red;
        private static Palette s_violet;
        private static Palette s_yellow;

        public static Palette Blue
        {
            get
            {
                if (s_blue == null)
                {
                    s_blue = Palette.Create("Blue", Color.Blue.Hue, COUNT);
                }

                return s_blue;
            }
        }

        public static Palette Green
        {
            get
            {
                if (s_green == null)
                {
                    s_green = Palette.Create("Green", Color.Green.Hue, COUNT);
                }

                return s_green;
            }
        }

        public static Palette Indigo
        {
            get
            {
                if (s_indigo == null)
                {
                    s_indigo = Palette.Create("Indigo", Color.Indigo.Hue, COUNT);
                }

                return s_indigo;
            }
        }

        public static Palette Orange
        {
            get
            {
                if (s_orange == null)
                {
                    s_orange = Palette.Create("Orange", Color.Orange.Hue, COUNT);
                }

                return s_orange;
            }
        }

        public static Palette Rainbow
        {
            get
            {
                if (s_rainbow == null)
                {
                    s_rainbow = new Palette("Rainbow");
                    s_rainbow.Entries.Add(new PaletteEntry(new FabricStyle(Color.Red)));
                    s_rainbow.Entries.Add(new PaletteEntry(new FabricStyle(Color.Orange)));
                    s_rainbow.Entries.Add(new PaletteEntry(new FabricStyle(Color.Yellow)));
                    s_rainbow.Entries.Add(new PaletteEntry(new FabricStyle(Color.Green)));
                    s_rainbow.Entries.Add(new PaletteEntry(new FabricStyle(Color.Blue)));
                    s_rainbow.Entries.Add(new PaletteEntry(new FabricStyle(Color.Indigo)));
                    s_rainbow.Entries.Add(new PaletteEntry(new FabricStyle(Color.Violet)));
                }

                return s_rainbow;
            }
        }

        public static Palette Red
        {
            get
            {
                if (s_red == null)
                {
                    s_red = Palette.Create("Red", Color.Red.Hue, COUNT);
                }

                return s_red;
            }
        }

        public static Palette Violet
        {
            get
            {
                if (s_violet == null)
                {
                    s_violet = Palette.Create("Violet", Color.Violet.Hue, COUNT);
                }

                return s_violet;
            }
        }

        public static Palette Yellow
        {
            get
            {
                if (s_yellow == null)
                {
                    s_yellow = Palette.Create("Yellow", Color.Yellow.Hue, COUNT);
                }

                return s_yellow;
            }
        }
    }
}