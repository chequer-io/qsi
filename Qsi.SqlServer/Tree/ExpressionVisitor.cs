using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Qsi.Data;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.SqlServer.Tree
{
    internal sealed class ExpressionVisitor : VisitorBase
    {
        public ExpressionVisitor(IVisitorContext visitorContext) : base(visitorContext)
        {
        }

        #region BooleanExpression
        public QsiExpressionNode VisitBooleanExpression(BooleanExpression booleanExpression)
        {
            switch (booleanExpression)
            {
                case BooleanBinaryExpression booleanBinaryExpression:
                    return VisitBooleanBinaryExpression(booleanBinaryExpression);

                case BooleanComparisonExpression booleanComparisonExpression:
                    return VisitBooleanComparisonExpression(booleanComparisonExpression);

                case BooleanExpressionSnippet booleanExpressionSnippet:
                    return VisitBooleanExpressionSnippet(booleanExpressionSnippet);

                case BooleanIsNullExpression booleanIsNullExpression:
                    return VisitBooleanIsNullExpression(booleanIsNullExpression);

                case BooleanNotExpression booleanNotExpression:
                    return VisitBooleanNotExpression(booleanNotExpression);

                case BooleanParenthesisExpression booleanParenthesisExpression:
                    return VisitBooleanParenthesisExpression(booleanParenthesisExpression);

                case BooleanTernaryExpression booleanTernaryExpression:
                    return VisitBooleanTernaryExpression(booleanTernaryExpression);

                case EventDeclarationCompareFunctionParameter eventDeclarationCompareFunctionParameter:
                    return VisitEventDeclarationCompareFunctionParameter(eventDeclarationCompareFunctionParameter);

                case ExistsPredicate existsPredicate:
                    return VisitExistsPredicate(existsPredicate);

                case FullTextPredicate fullTextPredicate:
                    return VisitFullTextPredicate(fullTextPredicate);

                case GraphMatchCompositeExpression graphMatchCompositeExpression:
                    return VisitGraphMatchCompositeExpression(graphMatchCompositeExpression);

                case GraphMatchExpression graphMatchExpression:
                    return VisitGraphMatchExpression(graphMatchExpression);

                case GraphMatchLastNodePredicate graphMatchLastNodePredicate:
                    return VisitGraphMatchLastNodePredicate(graphMatchLastNodePredicate);

                case GraphMatchNodeExpression graphMatchNodeExpression:
                    return VisitGraphMatchNodeExpression(graphMatchNodeExpression);

                case GraphMatchPredicate graphMatchPredicate:
                    return VisitGraphMatchPredicate(graphMatchPredicate);

                case GraphMatchRecursivePredicate graphMatchRecursivePredicate:
                    return VisitGraphMatchRecursivePredicate(graphMatchRecursivePredicate);

                case GraphRecursiveMatchQuantifier graphRecursiveMatchQuantifier:
                    return VisitGraphRecursiveMatchQuantifier(graphRecursiveMatchQuantifier);

                case InPredicate inPredicate:
                    return VisitInPredicate(inPredicate);

                case LikePredicate likePredicate:
                    return VisitLikePredicate(likePredicate);

                case SubqueryComparisonPredicate subqueryComparisonPredicate:
                    return VisitSubqueryComparisonPredicate(subqueryComparisonPredicate);

                // TSEQUAL ( expression, expression )
                case TSEqualCall tsEqualCall:
                    return CreateInvokeExpression(SqlServerKnownFunction.TsEqual, tsEqualCall.FirstExpression, tsEqualCall.SecondExpression);

                // UPDATE ( column )
                case UpdateCall updateCall:
                    return CreateInvokeExpression(SqlServerKnownFunction.Update, updateCall.Identifier);
            }

            throw TreeHelper.NotSupportedTree(booleanExpression);
        }

        public QsiExpressionNode VisitGraphMatchCompositeExpression(GraphMatchCompositeExpression graphMatchCompositeExpression)
        {
            throw TreeHelper.NotSupportedFeature("Graph match");
        }

        public QsiExpressionNode VisitGraphMatchExpression(GraphMatchExpression graphMatchExpression)
        {
            throw TreeHelper.NotSupportedFeature("Graph match");
        }

        public QsiExpressionNode VisitGraphMatchLastNodePredicate(GraphMatchLastNodePredicate graphMatchLastNodePredicate)
        {
            throw TreeHelper.NotSupportedFeature("Graph match");
        }

        public QsiExpressionNode VisitGraphMatchNodeExpression(GraphMatchNodeExpression graphMatchNodeExpression)
        {
            throw TreeHelper.NotSupportedFeature("Graph match");
        }

        public QsiExpressionNode VisitGraphMatchPredicate(GraphMatchPredicate graphMatchPredicate)
        {
            throw TreeHelper.NotSupportedFeature("Graph match");
        }

        public QsiExpressionNode VisitGraphMatchRecursivePredicate(GraphMatchRecursivePredicate graphMatchRecursivePredicate)
        {
            throw TreeHelper.NotSupportedFeature("Graph match");
        }

        public QsiExpressionNode VisitGraphRecursiveMatchQuantifier(GraphRecursiveMatchQuantifier graphRecursiveMatchQuantifier)
        {
            throw TreeHelper.NotSupportedFeature("Graph match");
        }

        public QsiLogicalExpressionNode VisitBooleanBinaryExpression(BooleanBinaryExpression booleanBinaryExpression)
        {
            return TreeHelper.Create<QsiLogicalExpressionNode>(n =>
            {
                n.Left.SetValue(VisitBooleanExpression(booleanBinaryExpression.FirstExpression));
                n.Right.SetValue(VisitBooleanExpression(booleanBinaryExpression.SecondExpression));

                n.Operator = booleanBinaryExpression.BinaryExpressionType switch
                {
                    BooleanBinaryExpressionType.And => SqlServerKnownOperator.And,
                    BooleanBinaryExpressionType.Or => SqlServerKnownOperator.Or,
                    _ => throw new InvalidOperationException()
                };
            });
        }

        public QsiLogicalExpressionNode VisitBooleanComparisonExpression(BooleanComparisonExpression booleanComparisonExpression)
        {
            return TreeHelper.Create<QsiLogicalExpressionNode>(n =>
            {
                n.Left.SetValue(VisitScalarExpression(booleanComparisonExpression.FirstExpression));
                n.Right.SetValue(VisitScalarExpression(booleanComparisonExpression.SecondExpression));

                n.Operator = ConvertBooleanComparisonType(booleanComparisonExpression.ComparisonType);
            });
        }

        public QsiExpressionNode VisitBooleanExpressionSnippet(BooleanExpressionSnippet booleanExpressionSnippet)
        {
            throw TreeHelper.NotSupportedFeature("Snippet");
        }

        public QsiLogicalExpressionNode VisitBooleanIsNullExpression(BooleanIsNullExpression booleanIsNullExpression)
        {
            return TreeHelper.Create<QsiLogicalExpressionNode>(n =>
            {
                n.Left.SetValue(VisitScalarExpression(booleanIsNullExpression.Expression));
                n.Right.SetValue(TreeHelper.CreateNullLiteral());

                n.Operator = booleanIsNullExpression.IsNot ? SqlServerKnownOperator.NotEqualToExclamation : SqlServerKnownOperator.Equals;
            });
        }

        public QsiUnaryExpressionNode VisitBooleanNotExpression(BooleanNotExpression booleanNotExpression)
        {
            return TreeHelper.Create<QsiUnaryExpressionNode>(n =>
            {
                n.Operator = SqlServerKnownOperator.Not;
                n.Expression.SetValue(VisitBooleanExpression(booleanNotExpression.Expression));
            });
        }

        public QsiExpressionNode VisitBooleanParenthesisExpression(BooleanParenthesisExpression booleanParenthesisExpression)
        {
            return VisitBooleanExpression(booleanParenthesisExpression.Expression);
        }

        public QsiInvokeExpressionNode VisitExistsPredicate(ExistsPredicate existsPredicate)
        {
            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunctionAccess(SqlServerKnownFunction.Exists));
                n.Parameters.Add(VisitScalarSubquery(existsPredicate.Subquery));
            });
        }

        public QsiExpressionNode VisitFullTextPredicate(FullTextPredicate fullTextPredicate)
        {
            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunctionAccess(SqlServerKnownFunction.FullText));
                n.Parameters.AddRange(fullTextPredicate.Columns.Select(ExpressionVisitor.VisitColumnReferenceExpression));

                n.Parameters.Add(fullTextPredicate.Value != null ?
                    VisitValueExpression(fullTextPredicate.Value) :
                    VisitLiteral(fullTextPredicate.PropertyName));

                n.Parameters.Add(VisitValueExpression(fullTextPredicate.LanguageTerm));
            });
        }

        public QsiExpressionNode VisitBooleanTernaryExpression(BooleanTernaryExpression booleanTernaryExpression)
        {
            var invoke = TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunctionAccess(SqlServerKnownFunction.Ternary));

                n.Parameters.Add(VisitScalarExpression(booleanTernaryExpression.FirstExpression));
                n.Parameters.Add(VisitScalarExpression(booleanTernaryExpression.SecondExpression));
                n.Parameters.Add(VisitScalarExpression(booleanTernaryExpression.ThirdExpression));
            });

            var operand = booleanTernaryExpression.TernaryExpressionType switch
            {
                BooleanTernaryExpressionType.Between => SqlServerKnownOperator.Between,
                BooleanTernaryExpressionType.NotBetween => SqlServerKnownOperator.NotBetween,
                _ => throw new InvalidOperationException()
            };

            return TreeHelper.CreateUnary(operand, invoke);
        }

        public QsiExpressionNode VisitEventDeclarationCompareFunctionParameter(EventDeclarationCompareFunctionParameter eventDeclarationCompareFunctionParameter)
        {
            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunctionAccess(SqlServerKnownFunction.EventDeclarationCompare));

                if (eventDeclarationCompareFunctionParameter.Name != null)
                {
                    n.Parameters.Add(TreeHelper.Create<QsiTableExpressionNode>(tn =>
                    {
                        tn.Table.SetValue(new QsiTableAccessNode
                        {
                            Identifier = IdentifierVisitor.CreateQualifiedIdentifier(eventDeclarationCompareFunctionParameter.Name.MultiPartIdentifier)
                        });
                    }));
                }

                if (eventDeclarationCompareFunctionParameter.EventValue != null)
                {
                    n.Parameters.Add(VisitScalarExpression(eventDeclarationCompareFunctionParameter.EventValue));
                }

                if (eventDeclarationCompareFunctionParameter.SourceDeclaration != null)
                {
                    n.Parameters.Add(VisitSourceDeclaration(eventDeclarationCompareFunctionParameter.SourceDeclaration));
                }
            });
        }

        public QsiExpressionNode VisitInPredicate(InPredicate inPredicate)
        {
            var logicalNode = TreeHelper.Create<QsiLogicalExpressionNode>(n =>
            {
                n.Left.SetValue(VisitScalarExpression(inPredicate.Expression));

                if (inPredicate.Subquery != null)
                {
                    n.Right.SetValue(VisitScalarSubquery(inPredicate.Subquery));
                }
                else
                {
                    n.Right.SetValue(TreeHelper.Create<QsiMultipleExpressionNode>(mn =>
                    {
                        mn.Elements.AddRange(inPredicate.Values.Select(VisitScalarExpression));
                    }));
                }
            });

            if (!inPredicate.NotDefined)
                return logicalNode;

            return TreeHelper.CreateUnary(SqlServerKnownOperator.Not, logicalNode);
        }

        public QsiExpressionNode VisitLikePredicate(LikePredicate likePredicate)
        {
            var invokeNode = TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunctionAccess(SqlServerKnownFunction.Like));

                n.Parameters.Add(VisitScalarExpression(likePredicate.FirstExpression));
                n.Parameters.Add(VisitScalarExpression(likePredicate.SecondExpression));

                if (likePredicate.EscapeExpression != null)
                {
                    n.Parameters.Add(VisitScalarExpression(likePredicate.EscapeExpression));
                }
            });

            if (!likePredicate.NotDefined)
                return invokeNode;

            return TreeHelper.CreateUnary(SqlServerKnownOperator.Not, invokeNode);
        }

        public QsiExpressionNode VisitSubqueryComparisonPredicate(SubqueryComparisonPredicate subqueryComparisonPredicate)
        {
            var expressionNode = TreeHelper.Create<QsiLogicalExpressionNode>(ln =>
            {
                ln.Left.SetValue(VisitScalarExpression(subqueryComparisonPredicate.Expression));
                ln.Right.SetValue(VisitScalarSubquery(subqueryComparisonPredicate.Subquery));

                ln.Operator = ConvertBooleanComparisonType(subqueryComparisonPredicate.ComparisonType);
            });

            if (subqueryComparisonPredicate.SubqueryComparisonPredicateType == SubqueryComparisonPredicateType.None)
                return expressionNode;

            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunctionAccess(subqueryComparisonPredicate.SubqueryComparisonPredicateType switch
                {
                    SubqueryComparisonPredicateType.All => SqlServerKnownFunction.All,
                    SubqueryComparisonPredicateType.Any => SqlServerKnownFunction.Any,
                    _ => throw new InvalidOperationException()
                }));

                n.Parameters.Add(expressionNode);
            });
        }
        #endregion

        #region ScalarExpression
        public QsiExpressionNode VisitScalarExpression(ScalarExpression scalarExpression)
        {
            switch (scalarExpression)
            {
                case BinaryExpression binaryExpression:
                    return VisitBinaryExpression(binaryExpression);

                case ExtractFromExpression extractFromExpression:
                    return CreateInvokeExpression(SqlServerKnownFunction.ExtractFrom, extractFromExpression.Expression, extractFromExpression.ExtractedElement);

                case IdentityFunctionCall identityFunctionCall:
                    return VisitIdentityFunctionCall(identityFunctionCall);

                case OdbcConvertSpecification odbcConvertSpecification:
                    return VisitOdbcConvertSpecification(odbcConvertSpecification);

                case PrimaryExpression primaryExpression:
                    return VisitPrimaryExpression(primaryExpression);

                case ScalarExpressionSnippet scalarExpressionSnippet:
                    return VisitScalarExpressionSnippet(scalarExpressionSnippet);

                case SourceDeclaration sourceDeclaration:
                    return VisitSourceDeclaration(sourceDeclaration);

                case UnaryExpression unaryExpression:
                    return VisitUnaryExpression(unaryExpression);
            }

            throw TreeHelper.NotSupportedTree(scalarExpression);
        }

        public QsiLogicalExpressionNode VisitBinaryExpression(BinaryExpression binaryExpression)
        {
            return TreeHelper.Create<QsiLogicalExpressionNode>(n =>
            {
                n.Left.SetValue(VisitScalarExpression(binaryExpression.FirstExpression));
                n.Right.SetValue(VisitScalarExpression(binaryExpression.SecondExpression));

                n.Operator = binaryExpression.BinaryExpressionType switch
                {
                    BinaryExpressionType.Add => SqlServerKnownOperator.Add,
                    BinaryExpressionType.Divide => SqlServerKnownOperator.Divide,
                    BinaryExpressionType.Modulo => SqlServerKnownOperator.Modulo,
                    BinaryExpressionType.Multiply => SqlServerKnownOperator.Multiply,
                    BinaryExpressionType.Subtract => SqlServerKnownOperator.Subtract,
                    BinaryExpressionType.BitwiseAnd => SqlServerKnownOperator.BitwiseAnd,
                    BinaryExpressionType.BitwiseOr => SqlServerKnownOperator.BitwiseOr,
                    BinaryExpressionType.BitwiseXor => SqlServerKnownOperator.BitwiseXor,
                    _ => throw new InvalidOperationException()
                };
            });
        }

        public QsiUnaryExpressionNode VisitUnaryExpression(UnaryExpression unaryExpression)
        {
            var expressionType = unaryExpression.UnaryExpressionType switch
            {
                UnaryExpressionType.Positive => SqlServerKnownOperator.Positive,
                UnaryExpressionType.Negative => SqlServerKnownOperator.Negative,
                UnaryExpressionType.BitwiseNot => SqlServerKnownOperator.BitwiseNot,
                _ => throw new InvalidOperationException()
            };

            return TreeHelper.CreateUnary(expressionType, VisitScalarExpression(unaryExpression.Expression));
        }

        public QsiExpressionNode VisitSourceDeclaration(SourceDeclaration _)
        {
            throw TreeHelper.NotSupportedFeature("source declaration");
        }

        #region Primary Expression
        public QsiExpressionNode VisitPrimaryExpression(PrimaryExpression primaryExpression)
        {
            switch (primaryExpression)
            {
                // inputdate AT TIME ZONE timezone
                case AtTimeZoneCall atTimeZoneCall:
                    return CreateInvokeExpression(SqlServerKnownFunction.AtTimeZone, atTimeZoneCall.DateValue, atTimeZoneCall.TimeZone);

                // COALESCE ( expression [ ,...n ] )
                case CoalesceExpression coalesceExpression:
                    return CreateInvokeExpression(SqlServerKnownFunction.Coalesce, coalesceExpression.Expressions);

                // IIF ( boolean_expression, true_value, false_value )
                case IIfCall iifCall:
                    return CreateInvokeExpression(SqlServerKnownFunction.IIf, iifCall.Predicate, iifCall.ThenExpression, iifCall.ElseExpression);

                // LEFT ( character_expression , integer_expression )
                case LeftFunctionCall leftFunctionCall:
                    return CreateInvokeExpression(SqlServerKnownFunction.Left, leftFunctionCall.Parameters);

                // RIGHT ( character_expression , integer_expression )
                case RightFunctionCall rightFunctionCall:
                    return CreateInvokeExpression(SqlServerKnownFunction.Right, rightFunctionCall.Parameters);

                // NEXT VALUE FOR [ database_name . ] [ schema_name . ]  sequence_name [ OVER (<over_order_by_clause>) ]
                case NextValueForExpression nextValueForExpression:
                    return CreateInvokeExpression(SqlServerKnownFunction.NextValueFor, nextValueForExpression.SequenceName, nextValueForExpression.OverClause);

                // NULLIF ( expression , expression )
                case NullIfExpression nullIfExpression:
                    return CreateInvokeExpression(SqlServerKnownFunction.NullIf, nullIfExpression.FirstExpression, nullIfExpression.SecondExpression);

                // CAST ( expression AS data_type [ ( length ) ] )
                case CastCall castCall:
                    return CreateInvokeExpression(SqlServerKnownFunction.Cast, castCall.DataType, castCall.Parameter);

                // TRY_CAST ( expression AS data_type [ ( length ) ] )
                case TryCastCall tryCastCall:
                    return CreateInvokeExpression(SqlServerKnownFunction.TryConvert, tryCastCall.Parameter, tryCastCall.DataType);

                // CONVERT ( data_type [ ( length ) ] , expression [ , style ] )
                case ConvertCall convertCall:
                    return CreateInvokeExpression(SqlServerKnownFunction.Convert, convertCall.DataType, convertCall.Parameter, convertCall.Style);

                // TRY_CONVERT ( data_type [ ( length ) ], expression [, style ] )
                case TryConvertCall tryConvertCall:
                    return CreateInvokeExpression(SqlServerKnownFunction.TryConvert, tryConvertCall.DataType, tryConvertCall.Parameter, tryConvertCall.Style);

                // PARSE ( string_value AS data_type [ USING culture ] )
                case ParseCall parseCall:
                    return CreateInvokeExpression(SqlServerKnownFunction.Parse, parseCall.StringValue, parseCall.DataType, parseCall.Culture);

                // TRY_PARSE ( string_value AS data_type [ USING culture ] )
                case TryParseCall tryParseCall:
                    return CreateInvokeExpression(SqlServerKnownFunction.TryParse, tryParseCall.StringValue, tryParseCall.DataType, tryParseCall.Culture);

                case FunctionCall functionCall:
                {
                    var callTarget = functionCall.CallTarget switch
                    {
                        MultiPartIdentifierCallTarget multiPartIdentifierCallTarget => multiPartIdentifierCallTarget.MultiPartIdentifier,
                        UserDefinedTypeCallTarget userDefinedTypeCallTarget => userDefinedTypeCallTarget.SchemaObjectName,
                        ExpressionCallTarget _ => throw TreeHelper.NotSupportedFeature("expression call target"),
                        null => new MultiPartIdentifier(),
                        _ => throw TreeHelper.NotSupportedTree(functionCall.CallTarget)
                    };

                    return CreateInvokeExpression(IdentifierVisitor.ConcatIdentifier(callTarget, functionCall.FunctionName), functionCall.Parameters);
                }

                case CaseExpression caseExpression:
                    return VisitCaseExpression(caseExpression);

                case ColumnReferenceExpression columnReferenceExpression:
                    return VisitColumnReferenceExpression(columnReferenceExpression);

                case ScalarSubquery scalarSubquery:
                    return VisitScalarSubquery(scalarSubquery);

                case ValueExpression valueExpression:
                    return VisitValueExpression(valueExpression);

                case UserDefinedTypePropertyAccess _:
                    throw TreeHelper.NotSupportedFeature("User defined type property access");

                case OdbcFunctionCall odbcFunctionCall:
                    return CreateInvokeExpression(odbcFunctionCall.Name.Value, odbcFunctionCall.Parameters);

                case ParameterlessCall parameterlessCall:
                    return VisitParameterlessCall(parameterlessCall);

                case PartitionFunctionCall partitionFunctionCall:
                    return VisitPartitionFunctionCall(partitionFunctionCall);
            }

            throw TreeHelper.NotSupportedTree(primaryExpression);
        }

        #region CaseExpression
        public QsiSwitchExpressionNode VisitCaseExpression(CaseExpression caseExpression)
        {
            switch (caseExpression)
            {
                case SearchedCaseExpression searchedCaseExpression:
                    return VisitSearchedCaseExpression(searchedCaseExpression);

                case SimpleCaseExpression simpleCaseExpression:
                    return VisitSimpleCaseExpression(simpleCaseExpression);
            }

            throw TreeHelper.NotSupportedTree(caseExpression);
        }

        public QsiSwitchExpressionNode VisitSearchedCaseExpression(SearchedCaseExpression searchedCaseExpression)
        {
            return TreeHelper.Create<QsiSwitchExpressionNode>(n =>
            {
                n.Cases.AddRange(searchedCaseExpression.WhenClauses.Select(VisitSearchedWhenClause));

                if (searchedCaseExpression.ElseExpression != null)
                {
                    n.Cases.Add(TreeHelper.Create<QsiSwitchCaseExpressionNode>(en =>
                    {
                        en.Consequent.SetValue(VisitScalarExpression(searchedCaseExpression.ElseExpression));
                    }));
                }
            });
        }

        public QsiSwitchCaseExpressionNode VisitSearchedWhenClause(SearchedWhenClause searchedWhenClause)
        {
            return TreeHelper.Create<QsiSwitchCaseExpressionNode>(n =>
            {
                n.Condition.SetValue(VisitBooleanExpression(searchedWhenClause.WhenExpression));
                n.Consequent.SetValue(VisitScalarExpression(searchedWhenClause.ThenExpression));
            });
        }

        public QsiSwitchExpressionNode VisitSimpleCaseExpression(SimpleCaseExpression simpleCaseExpression)
        {
            return TreeHelper.Create<QsiSwitchExpressionNode>(n =>
            {
                n.Value.SetValue(VisitScalarExpression(simpleCaseExpression.InputExpression));

                n.Cases.AddRange(simpleCaseExpression.WhenClauses.Select(VisitSimpleWhenClause));

                if (simpleCaseExpression.ElseExpression != null)
                {
                    n.Cases.Add(TreeHelper.Create<QsiSwitchCaseExpressionNode>(en =>
                    {
                        en.Consequent.SetValue(VisitScalarExpression(simpleCaseExpression.ElseExpression));
                    }));
                }
            });
        }

        public QsiSwitchCaseExpressionNode VisitSimpleWhenClause(SimpleWhenClause simpleWhenClause)
        {
            return TreeHelper.Create<QsiSwitchCaseExpressionNode>(n =>
            {
                n.Condition.SetValue(VisitScalarExpression(simpleWhenClause.WhenExpression));
                n.Consequent.SetValue(VisitScalarExpression(simpleWhenClause.ThenExpression));
            });
        }
        #endregion

        public QsiColumnExpressionNode VisitColumnReferenceExpression(ColumnReferenceExpression columnReferenceExpression)
        {
            return TreeHelper.Create<QsiColumnExpressionNode>(n =>
            {
                QsiQualifiedIdentifier name = null;

                if (columnReferenceExpression.MultiPartIdentifier != null)
                    name = IdentifierVisitor.CreateQualifiedIdentifier(columnReferenceExpression.MultiPartIdentifier);

                QsiColumnNode node = columnReferenceExpression.ColumnType switch
                {
                    ColumnType.Regular => new QsiDeclaredColumnNode { Name = name },
                    ColumnType.Wildcard => new QsiAllColumnNode { Path = name },
                    _ => throw TreeHelper.NotSupportedFeature($"{columnReferenceExpression.ColumnType} type column")
                };

                n.Column.SetValue(node);
            });
        }

        public QsiTableExpressionNode VisitScalarSubquery(ScalarSubquery scalarSubquery)
        {
            return TreeHelper.Create<QsiTableExpressionNode>(n =>
            {
                n.Table.SetValue(TableVisitor.VisitQueryExpression(scalarSubquery.QueryExpression));
            });
        }

        #region ValueExpression
        public QsiExpressionNode VisitValueExpression(ValueExpression valueExpression)
        {
            switch (valueExpression)
            {
                case GlobalVariableExpression globalVariableExpression:
                    return VisitGlobalVariableExpression(globalVariableExpression);

                case Literal literal:
                    return VisitLiteral(literal);

                case VariableReference variableReference:
                    return VisitVariableReference(variableReference);
            }

            throw TreeHelper.NotSupportedTree(valueExpression);
        }

        #region Literal
        public QsiLiteralExpressionNode VisitLiteral(Literal literal)
        {
            return TreeHelper.Create<QsiLiteralExpressionNode>(n =>
            {
                n.Type = literal.LiteralType switch
                {
                    LiteralType.Binary => QsiLiteralType.Binary,
                    LiteralType.Default => QsiLiteralType.Default,
                    LiteralType.Identifier => QsiLiteralType.String,
                    LiteralType.Integer => QsiLiteralType.Numeric,
                    LiteralType.Max => QsiLiteralType.Unknown,
                    LiteralType.Money => QsiLiteralType.Decimal,
                    LiteralType.Null => QsiLiteralType.Null,
                    LiteralType.Numeric => QsiLiteralType.Numeric,
                    LiteralType.Odbc => QsiLiteralType.Unknown,
                    LiteralType.Real => QsiLiteralType.Decimal,
                    LiteralType.String => QsiLiteralType.String,
                    _ => throw new InvalidOperationException()
                };

                n.Value = literal.Value;
            });
        }

        // TODO: Impl variable
        public QsiVariableAccessExpressionNode VisitVariableReference(VariableReference variableReference)
        {
            return TreeHelper.Create<QsiVariableAccessExpressionNode>(n =>
            {
                n.Identifier = new QsiQualifiedIdentifier(new QsiIdentifier(variableReference.Name, false));
            });
        }

        // TODO: Impl variable
        public QsiVariableAccessExpressionNode VisitGlobalVariableExpression(GlobalVariableExpression globalVariableExpression)
        {
            return TreeHelper.Create<QsiVariableAccessExpressionNode>(n =>
            {
                n.Identifier = new QsiQualifiedIdentifier(new QsiIdentifier(globalVariableExpression.Name, false));
            });
        }
        #endregion
        #endregion

        public QsiInvokeExpressionNode VisitParameterlessCall(ParameterlessCall parameterlessCall)
        {
            return CreateInvokeExpression(parameterlessCall.ParameterlessCallType switch
            {
                ParameterlessCallType.User => "USER",
                ParameterlessCallType.CurrentTimestamp => "CURRENT_TIMESTAMP",
                ParameterlessCallType.CurrentUser => "CURRENT_USER",
                ParameterlessCallType.SessionUser => "SESSION_USER",
                ParameterlessCallType.SystemUser => "SYSTEM_USER",
                _ => throw new InvalidOperationException()
            });
        }

        public QsiExpressionNode VisitPartitionFunctionCall(PartitionFunctionCall partitionFunctionCall)
        {
            return CreateInvokeExpression(
                IdentifierVisitor.CreateQualifiedIdentifier(partitionFunctionCall.DatabaseName, partitionFunctionCall.FunctionName),
                partitionFunctionCall.Parameters
            );
        }
        #endregion

        public QsiExpressionNode VisitScalarExpressionSnippet(ScalarExpressionSnippet scalarExpressionSnippet)
        {
            throw TreeHelper.NotSupportedFeature("Snippet");
        }

        public QsiExpressionNode VisitIdentityFunctionCall(IdentityFunctionCall identityFunctionCall)
        {
            throw TreeHelper.NotSupportedFeature("Identity function");
        }

        public QsiExpressionNode VisitOdbcConvertSpecification(OdbcConvertSpecification _)
        {
            throw TreeHelper.NotSupportedFeature("odbc convert specification");
        }
        #endregion

        #region Fragment
        public QsiRowValueExpressionNode VisitRowValue(RowValue rowValue)
        {
            return TreeHelper.Create<QsiRowValueExpressionNode>(n =>
            {
                n.ColumnValues.AddRange(rowValue.ColumnValues.Select(VisitScalarExpression));
            });
        }
        #endregion

        public QsiInvokeExpressionNode CreateInvokeExpression(QsiQualifiedIdentifier functionName, IEnumerable<TSqlFragment> parameters)
        {
            return CreateInvokeExpression(new QsiFunctionAccessExpressionNode { Identifier = functionName }, parameters);
        }

        public QsiInvokeExpressionNode CreateInvokeExpression(string functionName, params TSqlFragment[] parameters)
        {
            return CreateInvokeExpression(TreeHelper.CreateFunctionAccess(functionName), parameters);
        }

        public QsiInvokeExpressionNode CreateInvokeExpression(string functionName, IEnumerable<TSqlFragment> parameters)
        {
            return CreateInvokeExpression(TreeHelper.CreateFunctionAccess(functionName), parameters);
        }

        public QsiInvokeExpressionNode CreateInvokeExpression(QsiFunctionAccessExpressionNode functionAccessExpressionNode, IEnumerable<TSqlFragment> parameters)
        {
            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(functionAccessExpressionNode);

                n.Parameters.AddRange(parameters
                    .Where(p => p != null)
                    .Select(p =>
                    {
                        switch (p)
                        {
                            case BooleanExpression booleanExpression:
                                return VisitBooleanExpression(booleanExpression);

                            case ScalarExpression scalarExpression:
                                return VisitScalarExpression(scalarExpression);

                            case DataTypeReference dataTypeReference:
                                return new QsiTypeAccessExpressionNode
                                {
                                    Identifier = IdentifierVisitor.CreateQualifiedIdentifier(dataTypeReference.Name)
                                };

                            case Identifier identifier:
                                return TreeHelper.Create<QsiColumnExpressionNode>(cn =>
                                {
                                    cn.Column.SetValue(new QsiDeclaredColumnNode
                                    {
                                        Name = new QsiQualifiedIdentifier(IdentifierVisitor.CreateIdentifier(identifier))
                                    });
                                });

                            case MultiPartIdentifier multiPartIdentifier:
                                return TreeHelper.Create<QsiTableExpressionNode>(tn =>
                                {
                                    tn.Table.SetValue(new QsiTableAccessNode
                                    {
                                        Identifier = IdentifierVisitor.CreateQualifiedIdentifier(multiPartIdentifier)
                                    });
                                });

                            case OverClause _:
                                throw TreeHelper.NotSupportedFeature("over clause");

                            default:
                                throw new InvalidOperationException();
                        }
                    })
                );
            });
        }

        public string ConvertBooleanComparisonType(BooleanComparisonType booleanComparisonType)
        {
            return booleanComparisonType switch
            {
                BooleanComparisonType.Equals => SqlServerKnownOperator.Equals,
                BooleanComparisonType.GreaterThan => SqlServerKnownOperator.GreaterThan,
                BooleanComparisonType.GreaterThanOrEqualTo => SqlServerKnownOperator.GreaterThanOrEqualTo,
                BooleanComparisonType.LeftOuterJoin => SqlServerKnownOperator.LeftOuterJoin,
                BooleanComparisonType.LessThan => SqlServerKnownOperator.LessThan,
                BooleanComparisonType.LessThanOrEqualTo => SqlServerKnownOperator.LessThanOrEqualTo,
                BooleanComparisonType.NotEqualToBrackets => SqlServerKnownOperator.NotEqualToBrackets,
                BooleanComparisonType.NotEqualToExclamation => SqlServerKnownOperator.NotEqualToExclamation,
                BooleanComparisonType.NotGreaterThan => SqlServerKnownOperator.NotGreaterThan,
                BooleanComparisonType.NotLessThan => SqlServerKnownOperator.NotLessThan,
                BooleanComparisonType.RightOuterJoin => SqlServerKnownOperator.RightOuterJoin,
                _ => throw new InvalidOperationException()
            };
        }
    }
}
