using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Qsi.Data;
using Qsi.Engines;
using Qsi.Tree;

namespace Qsi.Analyzers.Context;

public class AnalyzerContext : IAnalyzerContext
{
    public QsiEngine Engine { get; }

    public QsiScript Script { get; }

    public IReadOnlyDictionary<IQsiBindParameterExpressionNode, QsiParameter> Parameters { get; }

    public IQsiTreeNode Tree { get; }

    public QsiAnalyzerOptions AnalyzerOptions { get; }

    public ExecuteOptions ExecuteOptions { get; }

    public CancellationToken CancellationToken { get; }

    public AnalyzerContext(
        QsiEngine engine,
        QsiScript script,
        IReadOnlyDictionary<IQsiBindParameterExpressionNode, QsiParameter> parameters,
        IQsiTreeNode tree,
        QsiAnalyzerOptions analyzerOptions,
        ExecuteOptions executeOptions,
        CancellationToken cancellationToken)
    {
        Engine = engine;
        Script = script;
        Parameters = parameters;
        Tree = tree;
        AnalyzerOptions = analyzerOptions ?? throw new ArgumentNullException(nameof(analyzerOptions));
        ExecuteOptions = executeOptions ?? new ExecuteOptions();
        CancellationToken = cancellationToken;
    }
}