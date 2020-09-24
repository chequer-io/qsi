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
        public KnownLayer Layer => KnownLayer.Text;

        private IPen _pen;

        private QsiScript[] _scripts;

        public QsiScriptRenderer()
        {
            _pen = new Pen(Brushes.Red, 1);
        }

        public void Update(QsiScript[] scripts)
        {
            _scripts = scripts;
        }

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            if (_scripts == null)
                return;

            var offsetY = textView.ScrollOffset.Y;
            var top = textView.GetDocumentLineByVisualTop(offsetY);
            var bottom = textView.GetDocumentLineByVisualTop(offsetY + textView.Bounds.Height);

            BackgroundGeometryBuilder builder = null;

            IEnumerable<QsiScript> scripts = _scripts
                .Where(s => top.LineNumber <= s.Start.Line || s.End.Line <= bottom.LineNumber);

            foreach (var script in scripts)
            {
                builder ??= new BackgroundGeometryBuilder();

                var minTop = double.MaxValue;
                var minLeft = double.MaxValue;
                var maxRight = double.MinValue;
                var maxBottom = double.MinValue;

                for (int i = script.Start.Line; i <= script.End.Line; i++)
                {
                    var line = textView.GetVisualLine(i);

                    if (line == null)
                        continue;

                    Rect bounds;

                    if (i == script.Start.Line)
                    {
                        var textLine = line.TextLines[^1];
                        bounds = textLine.GetTextBounds(script.Start.Column - 1, textLine.Length);
                    }
                    else if (i == script.End.Line)
                    {
                        bounds = line.TextLines[^1].GetTextBounds(0, script.End.Column);
                    }
                    else
                    {
                        if (line == null)
                            continue;

                        var textLine = line.TextLines[^1];
                        bounds = textLine.GetTextBounds(0, textLine.Length);
                    }

                    minTop = Math.Min(minTop, bounds.Y);
                    minLeft = Math.Min(minLeft, bounds.X);
                    maxRight = Math.Max(maxRight, bounds.Right);
                    maxBottom = Math.Max(maxBottom, bounds.Bottom);
                }

                var rect = new Rect(
                    minLeft, 
                    minTop, 
                    maxRight - minLeft,
                    maxBottom - minTop);

                builder.AddRectangle(textView, rect);
            }

            var geometry = builder?.CreateGeometry();

            if (geometry != null)
                drawingContext.DrawGeometry(null, _pen, geometry);
        }
    }
}
