using System.Threading;
using Qsi.Data;
using Qsi.Tree;

namespace Qsi.Analyzers.Context
{
    public abstract class AnalyzerContextWrapper : IAnalyzerContext
    {
        public QsiEngine Engine => _context.Engine;

        public QsiScript Script => _context.Script;

        public IQsiTreeNode Tree => _context.Tree;

        public QsiAnalyzerOptions Options => _context.Options;

        public CancellationToken CancellationToken => _context.CancellationToken;

        private readonly IAnalyzerContext _context;

        protected AnalyzerContextWrapper(IAnalyzerContext context)
        {
            _context = context;
        }
    }
}
