using System;
using System.IO;
using Antlr4.Runtime;
using Qsi.Impala.Utilities;

namespace Qsi.Impala.Internal;

internal abstract class ImpalaBaseLexer : Lexer
{
    public ImpalaDialect Dialect { get; set; }

    protected LexHint Hint { get; set; }

    protected ImpalaBaseLexer(ICharStream input) : base(input)
    {
    }

    protected ImpalaBaseLexer(ICharStream input, TextWriter output, TextWriter errorOutput) : base(input, output, errorOutput)
    {
    }

    protected void CategorizeIdentifier()
    {
        if (Text.StartsWith("."))
        {
            // If we see an identifier that starts with a dot, we push back the identifier
            // minus the dot back into the input stream.
            var source = new Tuple<ITokenSource, ICharStream>(this, (ICharStream)InputStream);
            var length = Text.Length;

            Token = TokenFactory.Create(
                source,
                ImpalaLexerInternal.DOT,
                ".",
                Channel,
                TokenStartCharIndex,
                TokenStartCharIndex,
                TokenStartLine,
                TokenStartColumn);

            InputStream.Seek(TokenStartCharIndex + 1);
            Interpreter.Column -= length - 1;
            return;
        }

        if (Dialect.KeywordMap.TryGetValue(Text.ToLower(), out var keywordId))
        {
            Type = keywordId;
        }
        else if (Dialect.IsReserved(Text))
        {
            Type = ImpalaLexerInternal.UNUSED_RESERVED_WORD;
        }
        else
        {
            Type = ImpalaLexerInternal.IDENT;
        }
    }

    protected bool IsCommentPlanHint()
    {
        return ImpalaUtility.IsCommentPlanHint(Text);
    }

    protected enum LexHint
    {
        Default,
        SingleLineComment,
        MultiLineComment
    }
}