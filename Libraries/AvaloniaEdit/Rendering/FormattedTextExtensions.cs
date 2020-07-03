using System.Collections.Generic;
using Avalonia.Media;

namespace AvaloniaEdit.Rendering
{
    public static class FormattedTextExtensions
    {
        public static void SetTextStyle(this FormattedText text, int startIndex, int length, IBrush foreground = null)
        {
            var spans = new List<FormattedTextStyleSpan>();

            if (text.Spans != null)
                spans.AddRange(text.Spans);

            spans.Add(new FormattedTextStyleSpan(startIndex, length, foreground));

            text.Spans = spans;
        }
    }
}
