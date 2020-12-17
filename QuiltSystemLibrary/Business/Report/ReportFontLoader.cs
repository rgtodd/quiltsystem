//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Business.Report
{
    internal static class ReportFontLoader
    {

        public static byte[] CarroisGothicRegular
        {
            get { return LoadFontData("RichTodd.QuiltSystem.Resources.CarroisGothic-Regular.ttf"); }
        }

        public static byte[] SegoeWP
        {
            get { return LoadFontData("RichTodd.QuiltSystem.Resources.SegoeWP.ttf"); }
        }

        public static byte[] SegoeWPBlack
        {
            get { return LoadFontData("RichTodd.QuiltSystem.Resources.SegoeWP-Black.ttf"); }
        }

        public static byte[] SegoeWPBold
        {
            get { return LoadFontData("RichTodd.QuiltSystem.Resources.SegoeWP-Bold.ttf"); }
        }

        public static byte[] SegoeWPLight
        {
            get { return LoadFontData("RichTodd.QuiltSystem.Resources.SegoeWP-Light.ttf"); }
        }

        public static byte[] SegoeWPSemibold
        {
            get { return LoadFontData("RichTodd.QuiltSystem.Resources.SegoeWP-Semibold.ttf"); }
        }

        public static byte[] SegoeWPSemilight
        {
            get { return LoadFontData("RichTodd.QuiltSystem.Resources.SegoeWP-Semilight.ttf"); }
        }

        private static byte[] LoadFontData(string name)
        {
            var assembly = typeof(ReportFontLoader).Assembly;

            using var stream = assembly.GetManifestResourceStream(name);
            if (stream == null)
                throw new ArgumentException("No resource with name " + name, nameof(name));

            var count = (int)stream.Length;
            var data = new byte[count];
            stream.Read(data, 0, count);
            return data;
        }

    }
}