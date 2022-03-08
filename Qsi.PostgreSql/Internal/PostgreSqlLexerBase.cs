namespace Qsi.PostgreSql.Internal
{
    using System;
    using Antlr4.Runtime;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;

    internal class PostgreSqlLexerBase : Lexer
    {
        protected static Queue<string> tags = new Queue<string>();

        public PostgreSqlLexerBase(ICharStream input) : base(input)
        {
        }
        public PostgreSqlLexerBase(ICharStream input, TextWriter output, TextWriter errorOutput)
            : base(input, output, errorOutput)
        {
        }

        private IIntStream getInputStream() { return InputStream; }
        public override string[] RuleNames => throw new NotImplementedException();

        public override IVocabulary Vocabulary => throw new NotImplementedException();

        public override string GrammarFileName => throw new NotImplementedException();

        public void pushTag() { tags.Enqueue(this.Text); }

        public bool isTag() { return this.Text.Equals(tags.Peek()); }

        public void popTag()
        {
            tags.Dequeue();
        }

        public void UnterminatedBlockCommentDebugAssert()
        {
            Debug.Assert(InputStream.LA(1) == -1 /*EOF*/);
        }

        public bool checkLA(int c)
        {
            return getInputStream().LA(1) != c;
        }

        public bool charIsLetter()
        {
            return Char.IsLetter((char)InputStream.LA(-1));
        }

        public void HandleNumericFail()
        {
            InputStream.Seek(getInputStream().Index - 2);
            Type = PostgreSqlLexerInternal.Integral;
        }

        public void HandleLessLessGreaterGreater()
        {
            if (Text == "<<") Type = PostgreSqlLexerInternal.LESS_LESS;
            if (Text == ">>") Type = PostgreSqlLexerInternal.GREATER_GREATER;
        }

        public bool CheckIfUtf32Letter()
        {
            return Char.IsLetter(Char.ConvertFromUtf32(Char.ConvertToUtf32((char)InputStream.LA(-2), (char)InputStream.LA(-1))).Substring(0)[0]);
        }

    }
}