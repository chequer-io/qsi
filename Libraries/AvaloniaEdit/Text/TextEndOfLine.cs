namespace AvaloniaEdit.Text
{
    public class TextEndOfLine : TextRun
    {
        public TextEndOfLine(int length) : this(length, null)
        {
        }

        public TextEndOfLine(int length, TextRunProperties textRunProperties)
        {
            Length = length;
            Properties = textRunProperties;
        }

        public sealed override StringRange StringRange => default;

        public sealed override int Length { get; }

        public sealed override TextRunProperties Properties { get; }
    }
}
