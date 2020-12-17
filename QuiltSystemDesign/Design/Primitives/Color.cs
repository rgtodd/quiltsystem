//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Globalization;

namespace RichTodd.QuiltSystem.Design.Primitives
{
    // Note: class is immutable.
    //
    public sealed class Color : IEquatable<Color>
    {
        private static readonly Color s_black = new Color(0, 0, 0);
        private static readonly Color s_blue = new Color(0, 0, 255);
        private static readonly Color s_green = new Color(0, 255, 0);
        private static readonly Color s_indigo = new Color(75, 0, 130);
        private static readonly Color s_orange = new Color(255, 165, 0);
        private static readonly Color s_red = new Color(255, 0, 0);
        private static readonly Color s_violet = new Color(127, 0, 255);
        private static readonly Color s_white = new Color(255, 255, 255);
        private static readonly Color s_yellow = new Color(255, 255, 0);

        private readonly byte m_b;
        private readonly byte m_g;
        private readonly byte m_r;

        // Derived Values

        private double m_brightness;
        private double m_cieA;
        private double m_cieB;
        private double m_cieL;
        private double m_hue;
        private double m_saturation;
        private double m_x;
        private double m_y;
        private double m_z;

        public Color(byte r, byte g, byte b)
        {
            m_r = r;
            m_g = g;
            m_b = b;

            ComputeHSL();
            ComputeXYZ();
            ComputeCieLAB();
        }

        public Color(int rgb)
        {
            if ((rgb & 0xFF000000) != 0) throw new ArgumentOutOfRangeException(nameof(rgb));

            m_r = (byte)((rgb & 0x00FF0000) >> 16);
            m_g = (byte)((rgb & 0x0000FF00) >> 8);
            m_b = (byte)(rgb & 0x000000FF);

            ComputeHSL();
            ComputeXYZ();
            ComputeCieLAB();
        }

        public static Color Black
        {
            get { return s_black; }
        }

        public static Color Blue
        {
            get { return s_blue; }
        }

        public static Color Green
        {
            get { return s_green; }
        }

        public static Color Indigo
        {
            get { return s_indigo; }
        }

        public static Color Orange
        {
            get { return s_orange; }
        }

        public static Color Red
        {
            get { return s_red; }
        }

        public static Color Violet
        {
            get { return s_violet; }
        }

        public static Color White
        {
            get { return s_white; }
        }

        public static Color Yellow
        {
            get { return s_yellow; }
        }

        public byte B
        {
            get { return m_b; }
        }

        public double Brightness
        {
            get { return m_brightness; }
        }

        public double CieA
        {
            get { return m_cieA; }
        }

        public double CieB
        {
            get { return m_cieB; }
        }

        public double CieL
        {
            get { return m_cieL; }
        }

        public byte G
        {
            get { return m_g; }
        }

        public double Hue
        {
            get { return m_hue; }
        }

        public byte R
        {
            get { return m_r; }
        }

        public double Saturation
        {
            get { return m_saturation; }
        }

        public string WebColor
        {
            get
            {
                return string.Format("#{0:X2}{1:X2}{2:X2}", R, G, B); ;
            }
        }

        public double X
        {
            get { return m_x; }
        }

        public double Y
        {
            get { return m_y; }
        }

        public double Z
        {
            get { return m_z; }
        }

        public static double AdjustedHueToHue(double adjustedHue)
        {
            return adjustedHue >= 0 && adjustedHue < 120
                ? adjustedHue * (60.0 / 120.0)
                : adjustedHue >= 120 && adjustedHue < 240 ? ((adjustedHue - 120) * (180.0 / 120.0)) + 60 : adjustedHue;
        }

        public static Color[,] CreateRectangleColorPalette(double hue, int rowCount, int columnCount)
        {
            var result = new Color[rowCount, columnCount];

            for (var row = 0; row < rowCount; ++row)
            {
                for (var column = 0; column < columnCount; ++column)
                {
                    var value = 1.0 * (columnCount - 1.0 - column) / (columnCount - 1.0);
                    var saturation = 1.0 * (rowCount - 1.0 - row) / (rowCount - 1.0);

                    var color = FromAhsb(255, hue, saturation, value);

                    result[row, column] = color;
                }
            }

            return result;
        }

        public static Color[,] CreateTriangleColorPalette(double hue, int rowCount, int columnCount)
        {
            var result = new Color[rowCount, columnCount];

            for (var row = 0; row < rowCount; ++row)
            {
                for (var column = 0; column < columnCount; ++column)
                {
                    var columnHalfCount = columnCount / 2;
                    var columnOffset = Math.Abs(column - columnHalfCount);
                    var maxRow = (int)Math.Round(rowCount - ((rowCount - 1) * columnOffset / (double)columnHalfCount));

                    Color color;
                    if (row >= rowCount - maxRow)
                    {
                        var value = 1.0 * column / (columnCount - 1.0);
                        var saturation = 1.0 * (rowCount - 1.0 - row) / Math.Max(maxRow - 1.0, 1.0);

                        color = FromAhsb(255, hue, saturation, value);
                    }
                    else
                    {
                        color = null;
                    }

                    result[row, column] = color;
                }
            }

            return result;
        }

        public static double DistanceSquared(Color lhs, Color rhs)
        {
            return DistanceE1994Squared(lhs, rhs);
            //return DistanceCmcSquared(lhs, rhs);
        }

        public static Color FromAhsb(int alpha, double hue, double saturation, double brightness)
        {
            if (alpha < 0 || alpha > 255) throw new ArgumentOutOfRangeException(nameof(alpha));
            if (hue < 0 || hue > 360) throw new ArgumentOutOfRangeException(nameof(hue));
            if (saturation < 0 || saturation > 1) throw new ArgumentOutOfRangeException(nameof(saturation));
            if (brightness < 0 || brightness > 1) throw new ArgumentOutOfRangeException(nameof(brightness));

            if (saturation == 0)
            {
                return FromArgb(
                    alpha,
                    Convert.ToInt32(brightness * 255),
                    Convert.ToInt32(brightness * 255),
                    Convert.ToInt32(brightness * 255));
            }

            double fMax, fMin;
            if (0.5 < brightness)
            {
                fMax = brightness - (brightness * saturation) + saturation;
                fMin = brightness + (brightness * saturation) - saturation;
            }
            else
            {
                fMax = brightness + (brightness * saturation);
                fMin = brightness - (brightness * saturation);
            }

            int iSextant;
            iSextant = (int)Math.Floor(hue / 60.0);

            if (300.0 <= hue)
            {
                hue -= 360.0;
            }
            hue /= 60.0;
            hue -= 2.0 * Math.Floor((iSextant + 1.0) % 6.0 / 2.0);

            var fMid = 0 == iSextant % 2
                ? (hue * (fMax - fMin)) + fMin
                : fMin - (hue * (fMax - fMin));

            var iMax = Convert.ToInt32(fMax * 255);
            var iMid = Convert.ToInt32(fMid * 255);
            var iMin = Convert.ToInt32(fMin * 255);

            return iSextant switch
            {
                1 => FromArgb(alpha, iMid, iMax, iMin),
                2 => FromArgb(alpha, iMin, iMax, iMid),
                3 => FromArgb(alpha, iMin, iMid, iMax),
                4 => FromArgb(alpha, iMid, iMin, iMax),
                5 => FromArgb(alpha, iMax, iMin, iMid),
                _ => FromArgb(alpha, iMax, iMid, iMin),
            };
        }

#pragma warning disable IDE0060 // Remove unused parameter
        public static Color FromArgb(int a, int r, int g, int b)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            return new Color((byte)r, (byte)g, (byte)b);
        }

        public static Color FromArgb(int r, int g, int b)
        {
            return new Color((byte)r, (byte)g, (byte)b);
        }

        public static Color FromArgb(int argb)
        {
            return new Color(argb & 0x00FFFFFF);
        }

        public static Color FromWebColor(string webColor)
        {
            var red = int.Parse(webColor.Substring(1, 2), NumberStyles.HexNumber);
            var green = int.Parse(webColor.Substring(3, 2), NumberStyles.HexNumber);
            var blue = int.Parse(webColor.Substring(5, 2), NumberStyles.HexNumber);

            return FromArgb(red, green, blue);
        }

        //   0 - Red                   -->   0 - Red
        //  60 - Yellow (Red + Green)  --> 120 - Yellow
        // 120 - Green
        // 180 - Cyan   (Green + Blue)
        // 240 - Blue                  --> 240 - Blue
        // 300 - Magenta (Blue + Red)
        //
        public static double HueToAdjustedHue(double hue)
        {
            return hue >= 0 && hue < 60
                ? hue * (120.0 / 60.0)
                : hue >= 60 && hue < 240
                    ? ((hue - 60) * (120.0 / 180.0)) + 120
                    : hue;
        }

        public static bool operator !=(Color lhs, Color rhs)
        {
            return lhs is null && rhs is null
                ? false
                : lhs is null || rhs is null
                    ? true
                    : !(lhs.R == rhs.R && lhs.G == rhs.G && lhs.B == rhs.B);
        }

        public static bool operator ==(Color lhs, Color rhs)
        {
            return lhs is null && rhs is null
                ? true :
                lhs is null || rhs is null
                    ? false
                    : lhs.R == rhs.R && lhs.G == rhs.G && lhs.B == rhs.B;
        }

        public Color Closest(IEnumerable<Color> colors)
        {
            Color result = null;
            var resultDistance = 0.0;

            foreach (var color in colors)
            {
                var distance = DistanceSquared(this, color);

                if (result == null || distance < resultDistance)
                {
                    result = color;
                    resultDistance = distance;
                }
            }

            return result;
        }

        public T Closest<T>(IEnumerable<T> items, Func<T, Color> func) where T : class
        {
            T result = null;
            var resultDistance = 0.0;

            foreach (var item in items)
            {
                var thisColor = func(item);

                var distance = DistanceSquared(this, thisColor);

                if (result == null || distance < resultDistance)
                {
                    result = item;
                    resultDistance = distance;
                }
            }

            return result;
        }

        public bool Equals(Color other)
        {
            return other is null
                ? false
                : R == other.R && G == other.G && B == other.B;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (!(obj is Color)) return false;

            var other = (Color)obj;

            return R == other.R && G == other.G && B == other.B;
        }

        public override int GetHashCode()
        {
            return R ^ B ^ G;
        }

        public int ToArgb(byte alpha)
        {
            return (int)(((uint)alpha << 24) | ((uint)R << 16) | ((uint)G << 8) | (uint)B);
        }

        public int ToRgb()
        {
            return (int)(((uint)R << 16) | ((uint)G << 8) | (uint)B);
        }

        private static double CieLab2Hue(double var_a, double var_b)
        {
            if (var_a >= 0 && var_b == 0) return 0;
            if (var_a < 0 && var_b == 0) return 180;
            if (var_a == 0 && var_b > 0) return 90;
            if (var_a == 0 && var_b < 0) return 270;

            double var_bias = 0;
            if (var_a > 0 && var_b > 0) var_bias = 0;
            if (var_a < 0) var_bias = 180;
            if (var_a > 0 && var_b < 0) var_bias = 360;

            return RadiansToDegrees(Math.Atan(var_b / var_a)) + var_bias;
        }

        private static double DegreesToRadians(double degrees)
        {
            return Math.PI * 2 * degrees / 360.0;
        }

#pragma warning disable IDE0051 // Remove unused private members
        private static double DistanceCmcSquared(Color lhs, Color rhs)
#pragma warning restore IDE0051 // Remove unused private members
        {
            var xC1 = Math.Sqrt(Squared(lhs.CieA) + Squared(lhs.CieB));
            var xC2 = Math.Sqrt(Squared(rhs.CieA) + Squared(rhs.CieB));
            var xff = Math.Sqrt(Math.Pow(xC1, 4) / (Math.Pow(xC1, 4) + 1900));
            var xH1 = CieLab2Hue(lhs.CieA, lhs.CieB);

            var xTT = xH1 < 164 || xH1 > 345
                ? 0.36 + Math.Abs(0.4 * Math.Cos(DegreesToRadians(35 + xH1)))
                : 0.56 + Math.Abs(0.2 * Math.Cos(DegreesToRadians(168 + xH1)));

            var xSL = lhs.CieL < 16
                ? 0.511
                : 0.040975 * lhs.CieL / (1 + (0.01765 * lhs.CieL));

            var xSC = (0.0638 * xC1 / (1 + (0.0131 * xC1))) + 0.638;
            var xSH = ((xff * xTT) + 1 - xff) * xSC;
            var xDH = Math.Sqrt(Squared(rhs.CieA - lhs.CieA) + Squared(rhs.CieB - lhs.CieB) - Squared(xC2 - xC1));

            xSL = (rhs.CieL - lhs.CieL) / xSL;
            xSC = (xC2 - xC1) / xSC;
            xSH = xDH / xSH;

            var result = Math.Sqrt(Squared(xSL) + Squared(xSC) + Squared(xSH));

            return result;
        }

        private static double DistanceE1994Squared(Color lhs, Color rhs)
        {
            var xC1 = Math.Sqrt(Squared(lhs.CieA) + Squared(lhs.CieB));
            var xC2 = Math.Sqrt(Squared(rhs.CieA) + Squared(rhs.CieB));

            var xDL = rhs.CieL - lhs.CieL;
            var xDC = xC2 - xC1;
            var xDE = Math.Sqrt(Squared(lhs.CieL - rhs.CieL) + Squared(lhs.CieA - rhs.CieA) + Squared(lhs.CieB - rhs.CieB));

            var xDH = Squared(xDE) - Squared(xDL) - Squared(xDC);
            xDH = xDH > 0 ? Math.Sqrt(xDH) : 0;

            var xSC = 1 + (0.045 * xC1);
            var xSH = 1 + (0.015 * xC1);

            xDL /= 1 /* WHT-L */;
            xDC /= 1 /* WHT-C */ * xSC;
            xDH /= 1 /* WHT-H  */ * xSH;

            var result = Squared(xDL) + Squared(xDC) + Squared(xDH);

            return result;
        }

#pragma warning disable IDE0051 // Remove unused private members
        private static double DistanceECieSquared(Color lhs, Color rhs)
#pragma warning restore IDE0051 // Remove unused private members
        {
            var result =
                Squared(lhs.CieL - rhs.CieL) +
                Squared(lhs.CieA - rhs.CieA) +
                Squared(lhs.CieB - rhs.CieB);

            return result;
        }

        private static double RadiansToDegrees(double radians)
        {
            return 360.0 * radians / (Math.PI * 2.0);
        }

        private static double Squared(double value)
        {
            return value * value;
        }

        private void ComputeCieLAB()
        {
            var var_X = X / 94.811;
            var var_Y = Y / 100.000;
            var var_Z = Z / 107.304;

            var_X = var_X > 0.008856
                ? Math.Pow(var_X, 1.0 / 3.0)
                : (7.787 * var_X) + (16.0 / 116.0);

            var_Y = var_Y > 0.008856
                ? Math.Pow(var_Y, 1.0 / 3.0)
                : (7.787 * var_Y) + (16.0 / 116.0);

            var_Z = var_Z > 0.008856
                ? Math.Pow(var_Z, 1.0 / 3.0)
                : (7.787 * var_Z) + (16.0 / 116.0);

            m_cieL = (116.0 * var_Y) - 16.0;
            m_cieA = 500.0 * (var_X - var_Y);
            m_cieB = 200.0 * (var_Y - var_Z);
        }

        private void ComputeHSL()
        {
            var red = (double)m_r;
            var green = (double)m_g;
            var blue = (double)m_b;

            red /= 255.0;
            green /= 255.0;
            blue /= 255.0;

            var min = Math.Min(Math.Min(red, green), blue);
            var max = Math.Max(Math.Max(red, green), blue);
            var delta = max - min;

            m_brightness = (min + max) / 2.0;

            if (delta == 0.0)
            {
                m_hue = 0.0;
                m_saturation = 0.0;
            }
            else
            {
                var hue = max == red
                    ? (green - blue) / delta
                    : max == green
                        ? 2d + ((blue - red) / delta)
                        : 4d + ((red - green) / delta);

                hue *= 60.0;
                if (hue < 0.0)
                {
                    hue += 360.0;
                }

                m_hue = hue;

                var chroma = min + max;

                m_saturation = chroma < 1.0
                    ? delta / chroma
                    : delta / (2.0 - chroma);
            }
        }

        private void ComputeXYZ()
        {
            var red = (double)m_r;
            var green = (double)m_g;
            var blue = (double)m_b;

            red /= 255.0;
            green /= 255.0;
            blue /= 255.0;

            if (red > 0.04045)
            {
                red = Math.Pow((red + 0.055) / 1.055, 2.4);
            }
            else
            {
                red /= 12.92;
            }

            if (green > 0.04045)
            {
                green = Math.Pow((green + 0.055) / 1.055, 2.4);
            }
            else
            {
                green /= 12.92;
            }

            if (blue > 0.04045)
            {
                blue = Math.Pow((blue + 0.055) / 1.055, 2.4);
            }
            else
            {
                blue /= 12.92;
            }

            red *= 100.0;
            green *= 100.0;
            blue *= 100.0;

            m_x = (red * 0.4124) + (green * 0.3576) + (blue * 0.1805);
            m_y = (red * 0.2126) + (green * 0.7152) + (blue * 0.0722);
            m_z = (red * 0.0193) + (green * 0.1192) + (blue * 0.9505);
        }
    }
}