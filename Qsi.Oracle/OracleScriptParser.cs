using System;
using System.Collections;
using System.Collections.Generic;
using Qsi.Data;
using Qsi.JSql;
using Qsi.Parsing.Common;

namespace Qsi.Oracle
{
    public sealed class OracleScriptParser : JSqlScriptParser
    {
        private const string Create = "CREATE";
        private const string Or = "OR";
        private const string Replace = "REPLACE";

        private const string Procedure = "PROCEDURE";
        private const string Function = "FUNCTION";
        private const string Package = "PACKAGE";
        private const string Trigger = "TRIGGER";
        private const string Type = "TYPE";

        private const string Begin = "BEGIN";
        private const string End = "END";

        private const string Case = "CASE";
        private const string If = "IF";

        private const string Exec = "EXEC";

        private const string SectionKey = "Oracle::Type";

        protected override QsiScriptType GetSuitableType(CommonScriptCursor cursor, IReadOnlyList<Token> tokens, Token[] leadingTokens)
        {
            if (leadingTokens.Length >= 1 &&
                Exec.Equals(cursor.Value[leadingTokens[0].Span], StringComparison.OrdinalIgnoreCase))
            {
                return QsiScriptType.Execute;
            }

            return base.GetSuitableType(cursor, tokens, leadingTokens);
        }

        protected override bool IsEndOfScript(ParseContext context)
        {
            var section = context.GetUserData<Section>(SectionKey);

            if (section != null && !section.EndOfSection)
            {
                section.EndOfSection = IsEndOfCreateSection(context, section);

                if (!section.EndOfSection)
                    return false;
            }

            if (!base.IsEndOfScript(context))
                return false;

            if (section == null && IsCreateStatement(context, out var type, out var index))
            {
                section = new Section(type)
                {
                    LastTokenCount = context.Tokens.Count
                };

                int transition = type == SectionType.CreatePackage ? index : IndexOfToken(context, index + 1, Begin);

                if (transition >= 0)
                {
                    section.LastTokenCount = transition + 1;
                    section.ExpectedToken.Push(End);
                    section.BodyOpened = true;

                    if (IsEndOfCreateSection(context, section))
                    {
                        return true;
                    }
                }
                else if (type == SectionType.CreateType)
                {
                    return true;
                }

                context.SetUserData(SectionKey, section);
                return false;
            }

            context.SetUserData<Section>(SectionKey, null);

            return true;
        }

        private static int IndexOfToken(ParseContext context, int startIndex, string keyword)
        {
            for (int i = startIndex; i < context.Tokens.Count; i++)
            {
                var value = context.Cursor.Value[context.Tokens[i].Span];

                if (keyword.Equals(value, StringComparison.OrdinalIgnoreCase))
                    return i;
            }

            return -1;
        }

        private bool IsEndOfCreateSection(ParseContext context, Section section)
        {
            if (context.Tokens.Count == section.LastTokenCount)
                return false;

            using var t = new TokenEnumerator(context, TokenType.Keyword, section.LastTokenCount);
            section.LastTokenCount = context.Tokens.Count;

            while (!section.BodyOpened && t.MoveNext())
            {
                if (!Begin.Equals(t.Current, StringComparison.OrdinalIgnoreCase))
                    continue;

                section.BodyOpened = true;
                section.ExpectedToken.Push(End);
                break;
            }

            if (!section.BodyOpened)
                return false;

            while (t.MoveNext() && section.ExpectedToken.TryPeek(out var expected))
            {
                if (expected.Equals(t.Current, StringComparison.OrdinalIgnoreCase))
                {
                    section.ExpectedToken.Pop();
                }
                else if (If.Equals(t.Current, StringComparison.OrdinalIgnoreCase))
                {
                    section.ExpectedToken.Push(If);
                    section.ExpectedToken.Push(End);
                }
                else if (Case.Equals(t.Current, StringComparison.OrdinalIgnoreCase))
                {
                    section.ExpectedToken.Push(Case);
                    section.ExpectedToken.Push(End);
                }
                else if (Begin.Equals(t.Current, StringComparison.OrdinalIgnoreCase))
                {
                    section.ExpectedToken.Push(End);
                }

                if (section.ExpectedToken.Count == 0)
                    return true;
            }

            return false;
        }

        private bool IsCreateStatement(ParseContext context, out SectionType type, out int endIndex)
        {
            using var k = new TokenEnumerator(context, TokenType.Keyword);

            endIndex = -1;
            type = default;

            // CREATE
            if (!k.MoveNext() || !Create.Equals(k.Current, StringComparison.OrdinalIgnoreCase))
                return false;

            if (!k.MoveNext())
                return false;

            // .. [OR REPLACE]
            if (Or.Equals(k.Current, StringComparison.OrdinalIgnoreCase))
            {
                if (!k.MoveNext() ||
                    !Replace.Equals(k.Current, StringComparison.OrdinalIgnoreCase) ||
                    !k.MoveNext())
                {
                    return false;
                }
            }

            endIndex = k.Index;

            if (Procedure.Equals(k.Current, StringComparison.OrdinalIgnoreCase))
            {
                type = SectionType.CreateProcedure;
                return true;
            }

            if (Function.Equals(k.Current, StringComparison.OrdinalIgnoreCase))
            {
                type = SectionType.CreateFunction;
                return true;
            }

            if (Package.Equals(k.Current, StringComparison.OrdinalIgnoreCase))
            {
                type = SectionType.CreatePackage;
                return true;
            }

            if (Trigger.Equals(k.Current, StringComparison.OrdinalIgnoreCase))
            {
                type = SectionType.CreateTrigger;
                return true;
            }

            if (Type.Equals(k.Current, StringComparison.OrdinalIgnoreCase))
            {
                type = SectionType.CreateType;
                return true;
            }

            return false;
        }

        private enum SectionType
        {
            CreateProcedure,
            CreateFunction,
            CreatePackage,
            CreateTrigger,
            CreateType
        }

        private sealed class Section
        {
            public SectionType Type { get; }

            public int LastTokenCount { get; set; }

            public Stack<string> ExpectedToken { get; }

            public bool EndOfSection { get; set; }

            public bool BodyOpened { get; set; }

            public Section(SectionType type)
            {
                Type = type;
                ExpectedToken = new Stack<string>();
            }
        }

        private struct TokenEnumerator : IEnumerator<string>
        {
            public string Current { get; private set; }

            public int Index { get; private set; }

            object IEnumerator.Current => Current;

            private IReadOnlyList<Token> _tokens;
            private ParseContext _context;
            private readonly TokenType _type;

            public TokenEnumerator(ParseContext context, TokenType type, int offset = 0)
            {
                Index = offset - 1;
                _tokens = context.Tokens;
                _context = context;
                _type = type;
                Current = null;
            }

            public bool MoveNext()
            {
                while (++Index < _tokens.Count)
                {
                    if (_type.HasFlag(_tokens[Index].Type))
                    {
                        Current = _context.Cursor.Value[_tokens[Index].Span];
                        return true;
                    }
                }

                return false;
            }

            public void Reset()
            {
                Index = -1;
            }

            public void Dispose()
            {
                _tokens = null;
                _context = null;
            }
        }
    }
}
