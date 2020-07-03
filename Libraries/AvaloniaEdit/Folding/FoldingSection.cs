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
using System.Text;
using AvaloniaEdit.Document;
using AvaloniaEdit.Rendering;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Folding
{
    /// <summary>
    ///     A section that can be folded.
    /// </summary>
    public sealed class FoldingSection : TextSegment
    {
        private readonly FoldingManager _manager;
        private bool _isFolded;
        private string _title;
        internal CollapsedLineSection[] CollapsedSections;

        internal FoldingSection(FoldingManager manager, int startOffset, int endOffset)
        {
            Debug.Assert(manager != null);
            _manager = manager;
            StartOffset = startOffset;
            Length = endOffset - startOffset;
        }

        /// <summary>
        ///     Gets/sets if the section is folded.
        /// </summary>
        public bool IsFolded
        {
            get => _isFolded;
            set
            {
                if (_isFolded != value)
                {
                    _isFolded = value;
                    ValidateCollapsedLineSections(); // create/destroy CollapsedLineSection
                    _manager.Redraw(this);
                }
            }
        }

        /// <summary>
        ///     Gets/Sets the text used to display the collapsed version of the folding section.
        /// </summary>
        public string Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;

                    if (IsFolded)
                        _manager.Redraw(this);
                }
            }
        }

        /// <summary>
        ///     Gets the content of the collapsed lines as text.
        /// </summary>
        public string TextContent => _manager.Document.GetText(StartOffset, EndOffset - StartOffset);

        /// <summary>
        ///     Gets the content of the collapsed lines as tooltip text.
        /// </summary>
        public string TooltipText
        {
            get
            {
                // This fixes SD-1394:
                // Each line is checked for leading indentation whitespaces. If
                // a line has the same or more indentation than the first line,
                // it is reduced. If a line is less indented than the first line
                // the indentation is removed completely.
                //
                // See the following example:
                // 	line 1
                // 		line 2
                // 			line 3
                //  line 4
                //
                // is reduced to:
                // line 1
                // 	line 2
                // 		line 3
                // line 4

                var startLine = _manager.Document.GetLineByOffset(StartOffset);
                var endLine = _manager.Document.GetLineByOffset(EndOffset);
                var builder = new StringBuilder();

                var current = startLine;
                var startIndent = TextUtilities.GetLeadingWhitespace(_manager.Document, startLine);

                while (current != endLine.NextLine)
                {
                    var currentIndent = TextUtilities.GetLeadingWhitespace(_manager.Document, current);

                    if (current == startLine && current == endLine)
                    {
                        builder.Append(_manager.Document.GetText(StartOffset, EndOffset - StartOffset));
                    }
                    else if (current == startLine)
                    {
                        if (current.EndOffset - StartOffset > 0)
                            builder.AppendLine(_manager.Document.GetText(StartOffset, current.EndOffset - StartOffset).TrimStart());
                    }
                    else if (current == endLine)
                    {
                        builder.Append(startIndent.Length <= currentIndent.Length
                            ? _manager.Document.GetText(current.Offset + startIndent.Length,
                                EndOffset - current.Offset - startIndent.Length)
                            : _manager.Document.GetText(current.Offset + currentIndent.Length,
                                EndOffset - current.Offset - currentIndent.Length));
                    }
                    else
                    {
                        builder.AppendLine(startIndent.Length <= currentIndent.Length
                            ? _manager.Document.GetText(current.Offset + startIndent.Length,
                                current.Length - startIndent.Length)
                            : _manager.Document.GetText(current.Offset + currentIndent.Length,
                                current.Length - currentIndent.Length));
                    }

                    current = current.NextLine;
                }

                return builder.ToString();
            }
        }

        /// <summary>
        ///     Gets/Sets an additional object associated with this folding section.
        /// </summary>
        public object Tag { get; set; }

        internal void ValidateCollapsedLineSections()
        {
            if (!_isFolded)
            {
                RemoveCollapsedLineSection();
                return;
            }

            // It is possible that StartOffset/EndOffset get set to invalid values via the property setters in TextSegment,
            // so we coerce those values into the valid range.
            var startLine = _manager.Document.GetLineByOffset(StartOffset.CoerceValue(0, _manager.Document.TextLength));
            var endLine = _manager.Document.GetLineByOffset(EndOffset.CoerceValue(0, _manager.Document.TextLength));

            if (startLine == endLine)
            {
                RemoveCollapsedLineSection();
            }
            else
            {
                if (CollapsedSections == null)
                    CollapsedSections = new CollapsedLineSection[_manager.TextViews.Count];

                // Validate collapsed line sections
                var startLinePlusOne = startLine.NextLine;

                for (var i = 0; i < CollapsedSections.Length; i++)
                {
                    var collapsedSection = CollapsedSections[i];

                    if (collapsedSection == null || collapsedSection.Start != startLinePlusOne || collapsedSection.End != endLine)
                    {
                        // recreate this collapsed section
                        if (collapsedSection != null)
                        {
                            Debug.WriteLine("CollapsedLineSection validation - recreate collapsed section from " + startLinePlusOne + " to " + endLine);
                            collapsedSection.Uncollapse();
                        }

                        CollapsedSections[i] = _manager.TextViews[i].CollapseLines(startLinePlusOne, endLine);
                    }
                }
            }
        }

        /// <inheritdoc />
        protected override void OnSegmentChanged()
        {
            ValidateCollapsedLineSections();
            base.OnSegmentChanged();

            // don't redraw if the FoldingSection wasn't added to the FoldingManager's collection yet
            if (IsConnectedToCollection)
                _manager.Redraw(this);
        }

        private void RemoveCollapsedLineSection()
        {
            if (CollapsedSections != null)
            {
                foreach (var collapsedSection in CollapsedSections)
                    if (collapsedSection?.Start != null)
                        collapsedSection.Uncollapse();

                CollapsedSections = null;
            }
        }
    }
}
