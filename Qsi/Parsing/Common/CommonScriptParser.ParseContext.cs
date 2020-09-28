using System.Collections.Generic;
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
