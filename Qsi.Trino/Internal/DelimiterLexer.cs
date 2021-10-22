using System;
using System.Collections.Generic;
using Antlr4.Runtime;
using Qsi.Shared.Reflection;

namespace Qsi.Trino.Internal
{
    internal class DelimiterLexer : SqlBaseLexer
    {
        private readonly ISet<string> _delimiters;

        private readonly IMemberAccessor<Lexer, int> _tokenStartCharIndex;
        private readonly IMemberAccessor<Lexer, int> _tokenStartLine;
        private readonly IMemberAccessor<Lexer, int> _tokenStartColumn;

        protected DelimiterLexer(ICharStream input, ISet<string> delimiters) : base(input)
        {
            _delimiters = delimiters;
            _tokenStartCharIndex = ReflectionHelper.GetAccessor<Lexer, int>("_tokenStartCharIndex");
            _tokenStartLine = ReflectionHelper.GetAccessor<Lexer, int>("_tokenStartLine");
            _tokenStartColumn = ReflectionHelper.GetAccessor<Lexer, int>("_tokenStartColumn");
        }

        public override IToken NextToken()
        {
            if (InputStream == null)
                throw new InvalidOperationException("nextToken requires a non-null input stream.");

            // Mark start location in char stream so unbuffered streams are
            // guaranteed at least have text of current token
            int tokenStartMarker = InputStream.Mark();

            try
            {
                outer_continue:

                while (true)
                {
                    if (HitEOF)
                    {
                        EmitEOF();
                        return Token;
                    }

                    Token = null;
                    Channel = DefaultTokenChannel;
                    _tokenStartCharIndex.SetValue(this, InputStream.Index);
                    _tokenStartColumn.SetValue(this, Interpreter.Column);
                    _tokenStartLine.SetValue(this, Interpreter.Line);
                    Text = null;

                    do
                    {
                        Type = 0;
                        int ttype = -1;

                        // This entire method is copied from org.antlr.v4.runtime.Lexer, with the following bit
                        // added to match the delimiters before we attempt to match the token
                        bool found = false;

                        foreach (var terminator in _delimiters)
                        {
                            if (Match(terminator))
                            {
                                ttype = SqlBaseParser.DELIMITER;
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            try
                            {
                                ttype = Interpreter.Match((ICharStream)InputStream, CurrentMode);
                            }
                            catch (LexerNoViableAltException e)
                            {
                                NotifyListeners(e); // report error
                                Recover(e);
                                ttype = TokenTypes.Skip;
                            }
                        }

                        if (InputStream.LA(1) == IntStreamConstants.EOF)
                            HitEOF = true;

                        if (Type == TokenConstants.InvalidType)
                            Type = ttype;

                        if (Type == TokenTypes.Skip)
                            goto outer_continue;
                    } while (Type == TokenTypes.More);

                    if (Token == null)
                        Emit();

                    return Token;
                }
            }
            finally
            {
                // make sure we release marker after match or
                // unbuffered char stream will keep buffering
                InputStream.Release(tokenStartMarker);
            }
        }

        private bool Match(string delimiter)
        {
            for (int i = 0; i < delimiter.Length; i++)
            {
                if (InputStream.LA(i + 1) != delimiter[i])
                    return false;
            }

            InputStream.Seek(InputStream.Index + delimiter.Length);
            return true;
        }
    }
}
