using System;
using System.Collections;
using System.Collections.Generic;
using Qsi.Data;
using Qsi.Parsing.Common;
using Qsi.Parsing.Common.Rules;
using Qsi.Shared.Extensions;

namespace Qsi.Oracle;

public sealed class OracleScriptParser : CommonScriptParser
{
    private const string Create = "CREATE";
    private const string Or = "OR";
    private const string Replace = "REPLACE";

    private const string Procedure = "PROCEDURE";
    private const string Function = "FUNCTION";
    private const string Package = "PACKAGE";
    private const string Trigger = "TRIGGER";
    private const string Type = "TYPE";

    private const string Declare = "DECLARE";
    private const string Begin = "BEGIN";
    private const string End = "END";

    private const string Loop = "LOOP";
    private const string Close = "CLOSE";

    private const string Case = "CASE";
    private const string If = "IF";
    private const string Null = "NULL";

    private const string Exec = "EXEC";
    private const string Execute = "EXECUTE";
    private const string SemiColon = ";";

    private const string BlockKey = "Oracle::Type";

    private readonly ITokenRule _oracleKeyword = new OracleLookbehindUnknownKeywordRule();

    public OracleScriptParser()
    {
        EnablePoundComment = false;
    }

    protected override QsiScriptType GetSuitableType(CommonScriptCursor cursor, IEnumerable<Token> tokens, Token[] leadingTokens)
    {
        if (leadingTokens.Length >= 1 && Exec.EqualsIgnoreCase(cursor.Value[leadingTokens[0].Span]))
        {
            return QsiScriptType.Execute;
        }

        return base.GetSuitableType(cursor, tokens, leadingTokens);
    }

    protected override bool TryParseToken(CommonScriptCursor cursor, out Token token)
    {
        int offset;
        TokenType tokenType;
        ITokenRule rule;

        switch (cursor.Current)
        {
            case ';':
                token = new Token(TokenType.Fragment, new Range(cursor.Index, cursor.Index + 1));
                return true;

            case var c when OracleLookbehindUnknownKeywordRule.IsOracleIdentifier(c):
                offset = 1;
                rule = _oracleKeyword;
                tokenType = TokenType.Keyword;
                break;

            default:
                return base.TryParseToken(cursor, out token);
        }

        int start = cursor.Index;
        cursor.Index += offset;

        if (rule.Run(cursor))
        {
            token = new Token(tokenType, start..(cursor.Index + 1));

            return true;
        }

        return base.TryParseToken(cursor, out token);
    }

    protected override bool IsEndOfScript(ParseContext context)
    {
        var block = context.GetUserData<Block>(BlockKey);

        if (block is { EndOfBlock: false })
        {
            block.EndOfBlock = IsEndOfBlock(context, block);

            if (!block.EndOfBlock)
                return false;

            context.SetUserData<Block>(BlockKey, null);
            return true;
        }

        if (!base.IsEndOfScript(context))
            return false;

        if (block == null && IsBlockStatement(context, out var type, out var index))
        {
            block = new Block(type)
            {
                LastTokenCount = context.Tokens.Count
            };

            int transition = type is BlockType.CreatePackage or BlockType.Begin ?
                index :
                IndexOfToken(context, index + 1, Begin);

            if (transition >= 0)
            {
                block.LastTokenCount = transition + 1;
                block.ExpectedToken.Push(SemiColon);
                block.ExpectedToken.Push(End);
                block.BodyOpened = true;

                if (IsEndOfBlock(context, block))
                {
                    return true;
                }
            }
            else if (type == BlockType.CreateType)
            {
                return true;
            }

            context.SetUserData(BlockKey, block);
            return false;
        }

        context.SetUserData<Block>(BlockKey, null);

        return true;
    }

    private static int IndexOfToken(ParseContext context, int startIndex, string keyword)
    {
        for (int i = startIndex; i < context.Tokens.Count; i++)
        {
            var value = context.Cursor.Value[context.Tokens[i].Span];

            if (keyword.EqualsIgnoreCase(value))
                return i;
        }

        return -1;
    }

    private bool IsEndOfBlock(ParseContext context, Block block)
    {
        if (context.Tokens.Count == block.LastTokenCount)
            return false;

        using var t = new TokenEnumerator(context, TokenType.Keyword | TokenType.Fragment, block.LastTokenCount);
        block.LastTokenCount = context.Tokens.Count;

        while (!block.BodyOpened && t.MoveNext())
        {
            if (!Begin.EqualsIgnoreCase(t.Current))
                continue;

            block.BodyOpened = true;
            block.ExpectedToken.Push(SemiColon);
            block.ExpectedToken.Push(End);
            break;
        }

        if (!block.BodyOpened)
            return false;

        while (t.MoveNext() && block.ExpectedToken.TryPeek(out var expected))
        {
            if (expected.EqualsIgnoreCase(t.Current))
            {
                block.ExpectedToken.Pop();
            }
            else if (If.EqualsIgnoreCase(t.Current))
            {
                block.ExpectedToken.Push(SemiColon);
                block.ExpectedToken.Push(If);
                block.ExpectedToken.Push(End);
            }
            else if (Case.EqualsIgnoreCase(t.Current))
            {
                block.ExpectedToken.Push(SemiColon);
                block.ExpectedToken.Push(Case);
                block.ExpectedToken.Push(End);
            }
            else if (Begin.EqualsIgnoreCase(t.Current))
            {
                block.ExpectedToken.Push(SemiColon);
                block.ExpectedToken.Push(End);
            }
            else if (Exec.EqualsIgnoreCase(t.Current) ||
                     Execute.EqualsIgnoreCase(t.Current))
            {
                block.ExpectedToken.Push(SemiColon);
            }
            else if (Loop.EqualsIgnoreCase(t.Current))
            {
                block.ExpectedToken.Push(SemiColon);
                block.ExpectedToken.Push(Loop);
                block.ExpectedToken.Push(End);
            }
            else if (Null.EqualsIgnoreCase(t.Current))
            {
                block.ExpectedToken.Push(SemiColon);
            }

            if (block.ExpectedToken.Count == 0)
                return true;
        }

        return false;
    }

    private bool IsBlockStatement(ParseContext context, out BlockType type, out int endIndex)
    {
        using var k = new TokenEnumerator(context, TokenType.Keyword);

        endIndex = -1;
        type = default;

        if (!k.MoveNext())
            return false;

        // BEGIN
        if (Begin.EqualsIgnoreCase(k.Current))
        {
            endIndex = k.Index;
            type = BlockType.Begin;
            return true;
        }

        // DECLARE
        if (Declare.EqualsIgnoreCase(k.Current))
        {
            endIndex = k.Index;
            type = BlockType.Declare;
            return true;
        }

        // EXEC | EXECUTE
        if (Exec.EqualsIgnoreCase(k.Current) ||
            Execute.EqualsIgnoreCase(k.Current))
        {
            endIndex = k.Index;
            type = BlockType.Execute;
            return true;
        }

        // CREATE
        if (!Create.EqualsIgnoreCase(k.Current) || !k.MoveNext())
            return false;

        // .. [OR REPLACE]
        if (Or.EqualsIgnoreCase(k.Current))
        {
            if (!k.MoveNext() || !Replace.EqualsIgnoreCase(k.Current) || !k.MoveNext())
            {
                return false;
            }
        }

        endIndex = k.Index;

        if (Procedure.EqualsIgnoreCase(k.Current))
        {
            type = BlockType.CreateProcedure;
            return true;
        }

        if (Function.EqualsIgnoreCase(k.Current))
        {
            type = BlockType.CreateFunction;
            return true;
        }

        if (Package.EqualsIgnoreCase(k.Current))
        {
            type = BlockType.CreatePackage;
            return true;
        }

        if (Trigger.EqualsIgnoreCase(k.Current))
        {
            type = BlockType.CreateTrigger;
            return true;
        }

        if (Type.EqualsIgnoreCase(k.Current))
        {
            type = BlockType.CreateType;
            return true;
        }

        return false;
    }

    private enum BlockType
    {
        CreateProcedure,
        CreateFunction,
        CreatePackage,
        CreateTrigger,
        CreateType,
        Begin,
        Declare,
        Execute
    }

    private sealed class Block
    {
        public BlockType BlockType { get; }

        public int LastTokenCount { get; set; }

        public Stack<string> ExpectedToken { get; }

        public bool EndOfBlock { get; set; }

        public bool BodyOpened { get; set; }

        public Block(BlockType blockType)
        {
            BlockType = blockType;
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
