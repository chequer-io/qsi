using System.Threading;
using Antlr4.Runtime;
using Qsi.Data;
using Qsi.Tree;

namespace Qsi.Parsing.Antlr;

public abstract class AntlrParserBase : IQsiTreeParser
{
    private readonly AntlrParserErrorHandler _parserErrorHandler;

    protected AntlrParserBase()
    {
        _parserErrorHandler =  new AntlrParserErrorHandler();
    }

    protected abstract Parser CreateParser(QsiScript script);

    protected abstract IQsiTreeNode Parse(QsiScript script, Parser parser);

    #region IQsiParser
    IQsiTreeNode IQsiTreeParser.Parse(QsiScript script, CancellationToken cancellationToken)
    {
        var parser = CreateParser(script);
        parser.AddErrorListener(_parserErrorHandler);
        return Parse(script, parser);
    }
    #endregion
}