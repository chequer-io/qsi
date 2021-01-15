using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Skia;
using Qsi.Debugger.Utilities;
using SkiaSharp;

namespace Qsi.Debugger
{
    public class NanumFontManager : IFontManagerImpl
    {
        private readonly string[] _bcp47 = { CultureInfo.CurrentCulture.ThreeLetterISOLanguageName, CultureInfo.CurrentCulture.TwoLetterISOLanguageName };

        private readonly SKTypeface[] _customTypefaces;
        private readonly string _defaultFamilyName;

        private readonly SKTypeface _defaultTypeface = SKTypeface.FromStream(AssetManager.FindResource("Fonts/NanumBarunGothic.ttf"));

        public NanumFontManager()
        {
            _customTypefaces = new[] { _defaultTypeface };
            _defaultFamilyName = _defaultTypeface.FamilyName;
        }

        public string GetDefaultFontFamilyName()
        {
            return _defaultFamilyName;
        }

        public IEnumerable<string> GetInstalledFontFamilyNames(bool checkForUpdates = false)
        {
            return _customTypefaces.Select(x => x.FamilyName);
        }

        public bool TryMatchCharacter(int codepoint, FontStyle fontStyle, FontWeight fontWeight, FontFamily fontFamily, CultureInfo culture, out Typeface typeface)
        {
            foreach (var customTypeface in _customTypefaces)
            {
                if (customTypeface.GetGlyph(codepoint) == 0)
                {
                    continue;
                }

                typeface = new Typeface(customTypeface.FamilyName, fontStyle, fontWeight);

                return true;
            }

            var fallback = SKFontManager.Default.MatchCharacter(fontFamily?.Name, (SKFontStyleWeight)fontWeight,
                SKFontStyleWidth.Normal, (SKFontStyleSlant)fontStyle, _bcp47, codepoint);

            typeface = new Typeface(fallback?.FamilyName ?? _defaultFamilyName, fontStyle, fontWeight);

            return true;
        }

        public IGlyphTypefaceImpl CreateGlyphTypeface(Typeface typeface)
        {
            return new GlyphTypefaceImpl(_defaultTypeface);
        }
    }
}
