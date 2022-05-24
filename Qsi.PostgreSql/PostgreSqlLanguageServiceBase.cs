using System;
using System.Collections.Generic;
using Qsi.Analyzers;
using Qsi.Analyzers.Action;
using Qsi.Analyzers.Definition;
using Qsi.Data;
using Qsi.Engines;
using Qsi.Parsing;
using Qsi.PostgreSql.Analyzers;
using Qsi.Services;
using Qsi.Tree;

namespace Qsi.PostgreSql
{
    public abstract class PostgreSqlLanguageServiceBase : QsiLanguageServiceBase
    {
        public override IQsiTreeParser CreateTreeParser()
        {
            return new PostgreSqlParser();
        }

        public override IQsiTreeDeparser CreateTreeDeparser()
        {
            return new PostgreSqlDeparser();
        }

        public override IQsiScriptParser CreateScriptParser()
        {
            return new PostgreSqlScriptParser();
        }

        public override QsiAnalyzerOptions CreateAnalyzerOptions()
        {
            return new()
            {
                AllowEmptyColumnsInSelect = true,
                AllowEmptyColumnsInInline = true,
                AllowNoAliasInDerivedTable = true,
                AllowPartialColumnInDataInsert = true,
            };
        }

        public override IEnumerable<IQsiAnalyzer> CreateAnalyzers(QsiEngine engine)
        {
            yield return new QsiActionAnalyzer(engine);
            // TODO: Check table analyzer.
            // yield return new PgTableAnalyzer(engine);
            yield return new PostgreSqlTableAnalyzer(engine);
            yield return new QsiDefinitionAnalyzer(engine);
        }

        public override QsiParameter FindParameter(QsiParameter[] parameters, IQsiBindParameterExpressionNode node)
        {
            if (parameters == null)
                return null;

            if (node.Type != QsiParameterType.Index)
            {
                throw new QsiException(QsiError.Syntax);
            }

            if (!node.Index.HasValue)
            {
                throw new QsiException(QsiError.Syntax);   
            }

            var postgresIndex = node.Index - 1;
            
            if (postgresIndex < 0 || postgresIndex >= parameters.Length)
            {
                Console.WriteLine(postgresIndex);
                Console.WriteLine(parameters.Length);
                throw new QsiException(QsiError.ParameterIndexOutOfRange, node.Index);   
            }

            return parameters[postgresIndex.Value];
        }
    }
}
