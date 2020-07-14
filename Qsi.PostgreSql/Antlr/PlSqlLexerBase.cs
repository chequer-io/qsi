using System.IO;
using Antlr4.Runtime;

namespace Qsi.PostgreSql.Antlr
{
    internal abstract class PlSqlLexerBase : Lexer
    {
        protected PlSqlLexerBase(ICharStream input) : base(input)
        {
        }

        protected PlSqlLexerBase(ICharStream input, TextWriter output, TextWriter errorOutput) : base(input, output, errorOutput)
        {
        }

        protected bool IsNewlineAtPos(int pos)
        {
            int la = InputStream.LA(pos);
            return la == -1 || la == '\n';
        }
    }
}
