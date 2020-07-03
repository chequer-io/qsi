using System;
using Antlr4.Runtime;
using Qsi.Data;
using Qsi.Tree;

namespace Qsi.Parsing.Antlr
{
    public abstract class AntlrParserBase : IQsiTreeParser
    {
        public event EventHandler<QsiSyntaxErrorException> SyntaxError;

        private readonly AntlrErrorHandler _errorHandler;

        protected AntlrParserBase()
        {
            _errorHandler = new AntlrErrorHandler();
            _errorHandler.Error += (s, e) => OnSyntaxError(e);
        }

        protected abstract Parser CreateParser(QsiScript script);

        protected abstract IQsiTreeNode Parse(QsiScript script, Parser parser);

        protected virtual void OnSyntaxError(QsiSyntaxErrorException e)
        {
            SyntaxError?.Invoke(this, e);
        }

        #region IQsiParser
        IQsiTreeNode IQsiTreeParser.Parse(QsiScript script)
        {
            var parser = CreateParser(script);
            parser.AddErrorListener(_errorHandler);
            return Parse(script, parser);
        }
        #endregion
    }
}
