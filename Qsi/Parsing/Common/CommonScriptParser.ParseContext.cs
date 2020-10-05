using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Qsi.Data;

namespace Qsi.Parsing.Common
{
    public partial class CommonScriptParser
    {
        protected class ParseContext
        {
            public readonly List<QsiScript> Scripts;

            public readonly CommonScriptCursor Cursor;

            public string Delimiter { get; set; }

            public readonly IReadOnlyList<Token> Tokens;

            internal int? FragmentStart;
            internal int FragmentEnd;

            internal int LastLine;
            internal int[] LineMap;

            private readonly List<Token> _tokens;
            private readonly CancellationToken _cancellationToken;

            private Dictionary<string, object> _userData;

            public ParseContext(string input, string delimiter, CancellationToken cancellationToken)
            {
                Scripts = new List<QsiScript>();
                Cursor = new CommonScriptCursor(input);
                Delimiter = delimiter;
                _tokens = new List<Token>();
                Tokens = _tokens;
                _cancellationToken = cancellationToken;
            }

            public string GetTokenText(Token token)
            {
                return Cursor.Value[token.Span];
            }

            public string JoinTokens(string delimiter, Token[] tokens)
            {
                return JoinTokens(delimiter, tokens, 0, tokens.Length);
            }

            public string JoinTokens(string delimiter, Token[] tokens, int startIndex, int count)
            {
                int bufferLength = 0;
                int end = startIndex + count;

                for (int i = startIndex; i < end; i++)
                {
                    bufferLength += tokens[i].Span.GetOffsetAndLength(Cursor.Length).Length;
                }

                bufferLength += (tokens.Length - 1) * delimiter.Length;

                var buffer = new char[bufferLength];
                int bufferIndex = 0;

                for (int i = startIndex; i < end; i++)
                {
                    if (i > startIndex)
                    {
                        delimiter.CopyTo(0, buffer, bufferIndex, delimiter.Length);
                        bufferIndex += delimiter.Length;
                    }

                    var (offset, length) = tokens[i].Span.GetOffsetAndLength(Cursor.Length);

                    Cursor.Value.CopyTo(offset, buffer, bufferIndex, length);
                    bufferIndex += length;
                }

                return new string(buffer);
            }

            public void AddToken(Token token)
            {
                _cancellationToken.ThrowIfCancellationRequested();
                _tokens.Add(token);
            }

            public void ClearTokens()
            {
                _tokens.Clear();
            }

            public T GetUserData<T>(string key)
            {
                if (_userData != null && _userData.TryGetValue(key, out var value))
                    return (T)value;

                return default;
            }

            public void SetUserData<T>(string key, T value)
            {
                _userData ??= new Dictionary<string, object>();
                _userData[key] = value;
            }
        }
    }
}
