using System.Collections.Generic;
using System.Threading;
using Qsi.Data;
using Qsi.Engines;
using Qsi.Tree;

namespace Qsi.Analyzers.Context
{
    public abstract class AnalyzerContextWrapper : IAnalyzerContext
    {
        public QsiEngine Engine => _context.Engine;

        public QsiScript Script => _context.Script;

        public IReadOnlyDictionary<IQsiBindParameterExpressionNode, QsiParameter> Parameters => _context.Parameters;

        public IQsiTreeNode Tree => _context.Tree;

        public QsiAnalyzerOptions Options { get; }

        public CancellationToken CancellationToken => _context.CancellationToken;

        private readonly IAnalyzerContext _context;

        protected AnalyzerContextWrapper(IAnalyzerContext context) : this(context, context.Options)
        {
        }

        protected AnalyzerContextWrapper(IAnalyzerContext context, QsiAnalyzerOptions options)
        {
            _context = context;
            Options = options;
        }
    }
}
