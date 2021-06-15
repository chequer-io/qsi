using System;
using System.Collections.Generic;
using System.Threading;
using Qsi.Data;
using Qsi.Engines;
using Qsi.Tree;

namespace Qsi.Analyzers.Context
{
    public class AnalyzerContext : IAnalyzerContext
    {
        public QsiEngine Engine { get; }

        public QsiScript Script { get; }

        public IReadOnlyDictionary<IQsiBindParameterExpressionNode, QsiParameter> Parameters { get; }

        public IQsiTreeNode Tree { get; }

        public QsiAnalyzerOptions Options { get; }

        public CancellationToken CancellationToken { get; }

        public AnalyzerContext(
            QsiEngine engine,
            QsiScript script,
            IReadOnlyDictionary<IQsiBindParameterExpressionNode, QsiParameter> parameters,
            IQsiTreeNode tree,
            QsiAnalyzerOptions options,
            CancellationToken cancellationToken)
        {
            Engine = engine;
            Script = script;
            Parameters = parameters;
            Tree = tree;
            Options = options ?? throw new ArgumentNullException(nameof(options));
            CancellationToken = cancellationToken;
        }
    }
}
