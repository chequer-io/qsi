using System.Collections.Generic;
using System.Linq;
using Qsi.Analyzers;
using Qsi.Analyzers.Action;
using Qsi.Analyzers.Table;
using Qsi.Collections;
using Qsi.Data;
using Qsi.Engines;
using Qsi.Parsing;
using Qsi.Tree;

namespace Qsi.Services
{
    public abstract class QsiLanguageServiceBase : IQsiLanguageService
    {
        public abstract QsiAnalyzerOptions CreateAnalyzerOptions();

        public virtual IEnumerable<IQsiAnalyzer> CreateAnalyzers(QsiEngine engine)
        {
            yield return new QsiActionAnalyzer(engine);
            yield return new QsiTableAnalyzer(engine);
        }

        public abstract IQsiTreeParser CreateTreeParser();

        public abstract IQsiTreeDeparser CreateTreeDeparser();

        public abstract IQsiScriptParser CreateScriptParser();

        public abstract IQsiRepositoryProvider CreateRepositoryProvider();

        private IEqualityComparer<QsiIdentifier> _comparer;

        public virtual bool MatchIdentifier(QsiIdentifier x, QsiIdentifier y)
        {
            _comparer ??= GetIdentifierComparer();
            return _comparer.Equals(x, y);
        }

        public virtual QsiParameter FindParameter(QsiParameter[] parameters, IQsiBindParameterExpressionNode node)
        {
            if (parameters == null)
                return null;

            if (node.Type == QsiParameterType.Index)
            {
                if (!node.Index.HasValue)
                    throw new QsiException(QsiError.Syntax);

                if (node.Index < 0 || node.Index >= parameters.Length)
                    throw new QsiException(QsiError.ParameterIndexOutOfRange, node.Index);

                return parameters[node.Index.Value];
            }

            if (string.IsNullOrEmpty(node.Name))
                throw new QsiException(QsiError.Syntax);

            var parameter = parameters.FirstOrDefault(p => p.Name == node.Name);

            if (parameter == null)
                throw new QsiException(QsiError.ParameterNotFound, node.Name);

            return parameter;
        }

        protected virtual IEqualityComparer<QsiIdentifier> GetIdentifierComparer()
        {
            return QsiIdentifierEqualityComparer.Default;
        }
    }
}
