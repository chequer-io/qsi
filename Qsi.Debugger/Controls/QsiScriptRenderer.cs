using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Media;
using AvaloniaEdit.Rendering;
using Qsi.Data;

namespace Qsi.Debugger.Controls
{
    public class QsiScriptRenderer : IBackgroundRenderer
    {
        public KnownLayer Layer => KnownLayer.Caret;

        private readonly IPen _pen;

        private QsiScript[] _scripts;

        public QsiScriptRenderer(IPen pen)
        {
            _pen = pen;
        }

        public void Update(QsiScript[] scripts)
        {
            _scripts = scripts;
        }

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            if (_scripts == null)
                return;

            var scrollOffset = textView.ScrollOffset;
            var top = textView.GetDocumentLineByVisualTop(scrollOffset.Y);
            var bottom = textView.GetDocumentLineByVisualTop(scrollOffset.Y + textView.Bounds.Height);

            IEnumerable<QsiScript> scripts = _scripts
                .Where(s => !(s.End.Line < top.LineNumber || bottom.LineNumber < s.Start.Line));

            foreach (var script in scripts)
            {
                var minTop = double.MaxValue;
                var minLeft = double.MaxValue;
                var maxRight = double.MinValue;
                var maxBottom = double.MinValue;

                var singleLine = script.Start.Line == script.End.Line;

                foreach (var line in textView.GetVisualLines(script.Start.Line, script.End.Line))
                {
                    Rect bounds;

                    if (singleLine)
                    {
                        var textLine = line.TextLines[^1];
                        bounds = textLine.GetTextBounds(script.Start.Column - 1, script.Script.Length);
                    }
                    else if (line.FirstDocumentLine.LineNumber == script.Start.Line)
                    {
                        var textLine = line.TextLines[^1];
                        bounds = textLine.GetTextBounds(script.Start.Column - 1, textLine.Length);
                    }
                    else if (line.LastDocumentLine.LineNumber == script.End.Line)
                    {
                        bounds = line.TextLines[^1].GetTextBounds(0, script.End.Column);
                    }
                    else
                    {
                        var textLine = line.TextLines[^1];
                        bounds = textLine.GetTextBounds(0, textLine.Length);
                    }

                    minTop = Math.Min(minTop, bounds.Y + line.VisualTop);
                    minLeft = Math.Min(minLeft, bounds.X);
                    maxRight = Math.Max(maxRight, bounds.Right);
                    maxBottom = Math.Max(maxBottom, bounds.Bottom + line.VisualTop);
                }

                var rect = new Rect(
                    minLeft - scrollOffset.X,
                    minTop - scrollOffset.Y,
                    maxRight - minLeft,
                    maxBottom - minTop);

                drawingContext.DrawRectangle(_pen, rect);
            }
        }
    }
}
