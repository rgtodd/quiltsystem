//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Diagnostics;

using PdfSharpCore.Fonts;

namespace RichTodd.QuiltSystem.Business.Report
{
    public class ReportFontResolver : IFontResolver
    {
        public string DefaultFontName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public byte[] GetFont(string faceName)
        {
            return faceName switch
            {
                FaceNames.SegoeWPLight => ReportFontLoader.SegoeWPLight,
                FaceNames.SegoeWPSemilight => ReportFontLoader.SegoeWPSemilight,
                FaceNames.SegoeWP => ReportFontLoader.SegoeWP,
                FaceNames.SegoeWPSemibold => ReportFontLoader.SegoeWPSemibold,
                FaceNames.SegoeWPBold => ReportFontLoader.SegoeWPBold,
                FaceNames.SegoeWPBlack => ReportFontLoader.SegoeWPBlack,
                FaceNames.CarroisGothicRegular => ReportFontLoader.CarroisGothicRegular,
                _ => throw new ArgumentException(string.Format("Invalid face name '{0}'", faceName), nameof(faceName)),
            };
        }

        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            var lowerFamilyName = familyName.ToLower();

            if (lowerFamilyName.StartsWith("segoe wp"))
            {
                var simulateBold = false;
                var simulateItalic = isItalic;

                string faceName;
                switch (lowerFamilyName)
                {
                    case FamilyNames.SegoeWPLight:
                        faceName = isBold ? FamilyNames.SegoeWPSemilight : FaceNames.SegoeWPLight;
                        break;

                    case FamilyNames.SegoeWPSemilight:
                        faceName = FaceNames.SegoeWPSemilight;
                        break;

                    case FamilyNames.SegoeWP:
                        faceName = isBold ? FaceNames.SegoeWPBold : FaceNames.SegoeWP;
                        break;

                    case FamilyNames.SegoeWPSemibold:
                        faceName = FaceNames.SegoeWPSemibold;
                        break;

                    case FamilyNames.SegoeWPBold:
                        faceName = isBold ? FaceNames.SegoeWPBlack : FaceNames.SegoeWPBold;
                        break;

                    case FamilyNames.SegoeWPBlack:
                        faceName = FaceNames.SegoeWPBlack;
                        break;

                    default:
                        Debug.Assert(false, "Unknown Segoe WP font: " + lowerFamilyName);
                        faceName = FaceNames.SegoeWPBlack;
                        break;
                }

                return new FontResolverInfo(faceName, simulateBold, simulateItalic);
            }
            else if (lowerFamilyName.StartsWith("carrois gothic"))
            {
                var simulateBold = false;
                var simulateItalic = isItalic;

                return new FontResolverInfo("CarroisGothicRegular", simulateBold, simulateItalic);
            }

            return null;
        }

        #region Private Classes

        private static class FaceNames
        {

            /// Used in the first parameter of the FontResolverInfo constructor.
            public const string CarroisGothicRegular = "CarroisGothicRegular";

            public const string SegoeWP = "SegoeWP";
            public const string SegoeWPBlack = "SegoeWPBlack";
            public const string SegoeWPBold = "SegoeWPBold";
            public const string SegoeWPLight = "SegoeWPLight";
            public const string SegoeWPSemibold = "SegoeWPSemibold";
            public const string SegoeWPSemilight = "SegoeWPSemilight";

        }

        private static class FamilyNames
        {

            public const string CarroisGothicRegular = "carrois gothic regular";
            public const string SegoeWP = "segoe wp";
            public const string SegoeWPBlack = "segoe wp black";
            public const string SegoeWPBold = "segoe wp bold";
            public const string SegoeWPLight = "segoe wp light";
            public const string SegoeWPSemibold = "segoe wp semibold";
            public const string SegoeWPSemilight = "segoe wp semilight";

        }

        #endregion Private Classes
    }
}