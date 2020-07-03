﻿// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Media;

namespace AvaloniaEdit.Rendering
{
	/// <summary>
	///     Base class for known layers.
	/// </summary>
	internal class Layer : Control
    {
        protected readonly KnownLayer KnownLayer;
        protected readonly TextView TextView;

        public Layer(TextView textView, KnownLayer knownLayer)
        {
            Debug.Assert(textView != null);
            TextView = textView;
            KnownLayer = knownLayer;
            Focusable = false;
            IsHitTestVisible = false;
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);
            TextView.RenderBackground(context, KnownLayer);
        }
    }
}
