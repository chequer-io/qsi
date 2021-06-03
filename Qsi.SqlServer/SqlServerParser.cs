﻿using System;
using System.Linq;
using System.Threading;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Qsi.Data;
using Qsi.Parsing;
using Qsi.SqlServer.Common;
using Qsi.SqlServer.Internal;
using Qsi.SqlServer.Tree;
using Qsi.Tree;

namespace Qsi.SqlServer
{
    public sealed class SqlServerParser : IQsiTreeParser, IVisitorContext
    {
        #region IContext
        TableVisitor IVisitorContext.TableVisitor => _tableVisitor;

        ExpressionVisitor IVisitorContext.ExpressionVisitor => _expressionVisitor;

        IdentifierVisitor IVisitorContext.IdentifierVisitor => _identifierVisitor;

        ActionVisitor IVisitorContext.ActionVisitor => _actionVisitor;

        DefinitionVisitor IVisitorContext.DefinitionVisitor => _definitionVisitor;

        private readonly TableVisitor _tableVisitor;
        private readonly ExpressionVisitor _expressionVisitor;
        private readonly IdentifierVisitor _identifierVisitor;
        private readonly ActionVisitor _actionVisitor;
        private readonly DefinitionVisitor _definitionVisitor;
        #endregion

        private readonly TSqlParserInternal _parser;

        public SqlServerParser(TransactSqlVersion transactSqlVersion)
        {
            _parser = new TSqlParserInternal(transactSqlVersion, false);
            _tableVisitor = CreateTableVisitor();
            _expressionVisitor = CreateExpressionVisitor();
            _identifierVisitor = CreateIdentifierVisitor();
            _actionVisitor = CreateActionVisitor();
            _definitionVisitor = CreateDefinitionVisitor();
        }

        private TableVisitor CreateTableVisitor()
        {
            return new(this);
        }

        private ExpressionVisitor CreateExpressionVisitor()
        {
            return new(this);
        }

        private IdentifierVisitor CreateIdentifierVisitor()
        {
            return new(this);
        }

        private ActionVisitor CreateActionVisitor()
        {
            return new(this);
        }

        private DefinitionVisitor CreateDefinitionVisitor()
        {
            return new(this);
        }

        public IQsiTreeNode Parse(QsiScript script, CancellationToken cancellationToken = default)
        {
            var result = _parser.Parse(script.Script);

            if (result is TSqlScript sqlScript)
            {
                var batch = sqlScript.Batches.FirstOrDefault();

                var statement = batch?.Statements?.FirstOrDefault()
                                ?? throw new QsiException(QsiError.Syntax);

                switch (statement)
                {
                    case UseStatement useStatement:
                        return _actionVisitor.VisitUseStatement(useStatement);

                    case InsertStatement insertStatement:
                        return _actionVisitor.VisitInsertStatement(insertStatement);

                    case DeleteStatement deleteStatement:
                        return _actionVisitor.VisitDeleteStatement(deleteStatement);

                    case UpdateStatement updateStatement:
                        return _actionVisitor.VisitUpdateStatement(updateStatement);

                    case MergeStatement mergeStatement:
                        return _actionVisitor.VisitMergeStatement(mergeStatement);

                    case AlterUserStatement alterUserStatement:
                        return _actionVisitor.VisitAlterUser(alterUserStatement);

                    case ViewStatementBody viewStatementBody:
                        return _definitionVisitor.VisitViewStatementBody(viewStatementBody);

                    default:
                        return _tableVisitor.Visit(statement);
                }
            }

            throw new InvalidOperationException();
        }
    }
}
