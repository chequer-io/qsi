using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

namespace Qsi.Shared.Antlr4;

internal sealed class CaseInsensitiveStream : ICharStream
{
    public int Index => _stream.Index;

    public int Size => _stream.Size;

    public string SourceName => _stream.SourceName;

    private readonly ICharStream _stream;

    public CaseInsensitiveStream(ICharStream stream)
    {
        _stream = stream;
    }

    public void Consume()
    {
        _stream.Consume();
    }

    public int LA(int i)
    {
        int result = _stream.LA(i);

        switch (result)
        {
            case 0:
            case IntStreamConstants.EOF:
                return result;

            default:
                return char.ToUpperInvariant((char)result);
        }
    }

    public int Mark()
    {
        return _stream.Mark();
    }

    public void Release(int marker)
    {
        _stream.Release(marker);
    }

    public void Seek(int index)
    {
        _stream.Seek(index);
    }

    public string GetText(Interval interval)
    {
        return _stream.GetText(interval);
    }
}