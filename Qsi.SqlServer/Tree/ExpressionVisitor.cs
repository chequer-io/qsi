using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Qsi.Data;
using Qsi.Tree.Base;
using Qsi.Utilities;

namespace Qsi.SqlServer.Tree
{
    public sealed class ExpressionVisitor : VisitorBase
    {
        public ExpressionVisitor(IContext context) : base(context)
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

                // case BooleanExpressionSnippet booleanExpressionSnippet:
                //     return VisitBooleanExpressionSnippet(booleanExpressionSnippet);

                case BooleanIsNullExpression booleanIsNullExpression:
                    return VisitBooleanIsNullExpression(booleanIsNullExpression);

                case BooleanNotExpression booleanNotExpression:
                    return VisitBooleanNotExpression(booleanNotExpression);

                // case BooleanParenthesisExpression booleanParenthesisExpression:
                //     return VisitBooleanParenthesisExpression(booleanParenthesisExpression);

                case BooleanTernaryExpression booleanTernaryExpression:
                    return VisitBooleanTernaryExpression(booleanTernaryExpression);

                // case EventDeclarationCompareFunctionParameter eventDeclarationCompareFunctionParameter:
                //     return VisitEventDeclarationCompareFunctionParameter(eventDeclarationCompareFunctionParameter);

                case ExistsPredicate existsPredicate:
                    return VisitExistsPredicate(existsPredicate);

                // case FullTextPredicate fullTextPredicate:
                //     return VisitFullTextPredicate(fullTextPredicate);
                //
                // case GraphMatchCompositeExpression graphMatchCompositeExpression:
                //     return VisitGraphMatchCompositeExpression(graphMatchCompositeExpression);
                //
                // case GraphMatchExpression graphMatchExpression:
                //     return VisitGraphMatchExpression(graphMatchExpression);
                //
                // case GraphMatchLastNodePredicate graphMatchLastNodePredicate:
                //     return VisitGraphMatchLastNodePredicate(graphMatchLastNodePredicate);
                //
                // case GraphMatchNodeExpression graphMatchNodeExpression:
                //     return VisitGraphMatchNodeExpression(graphMatchNodeExpression);
                //
                // case GraphMatchPredicate graphMatchPredicate:
                //     return VisitGraphMatchPredicate(graphMatchPredicate);
                //
                // case GraphMatchRecursivePredicate graphMatchRecursivePredicate:
                //     return VisitGraphMatchRecursivePredicate(graphMatchRecursivePredicate);
                //
                // case GraphRecursiveMatchQuantifier graphRecursiveMatchQuantifier:
                //     return VisitGraphRecursiveMatchQuantifier(graphRecursiveMatchQuantifier);
                //
                // case InPredicate inPredicate:
                //     return VisitInPredicate(inPredicate);
                //
                // case LikePredicate likePredicate:
                //     return VisitLikePredicate(likePredicate);
                //
                // case SubqueryComparisonPredicate subqueryComparisonPredicate:
                //     return VisitSubqueryComparisonPredicate(subqueryComparisonPredicate);
                //
                // case TSEqualCall tSEqualCall:
                //     return VisitTSEqualCall(tSEqualCall);
                //
                // case UpdateCall updateCall:
                //     return VisitUpdateCall(updateCall);
            }

            throw TreeHelper.NotSupportedTree(booleanExpression);
        }

        private QsiLogicalExpressionNode VisitBooleanBinaryExpression(BooleanBinaryExpression booleanBinaryExpression)
        {
            return TreeHelper.Create<QsiLogicalExpressionNode>(n =>
            {
                n.Left.SetValue(VisitBooleanExpression(booleanBinaryExpression.FirstExpression));
                n.Right.SetValue(VisitBooleanExpression(booleanBinaryExpression.SecondExpression));

                n.Operator = booleanBinaryExpression.BinaryExpressionType switch
                {
                    BooleanBinaryExpressionType.And => "AND",
                    BooleanBinaryExpressionType.Or => "OR",
                    _ => throw new InvalidOperationException()
                };
            });
        }

        private QsiLogicalExpressionNode VisitBooleanComparisonExpression(BooleanComparisonExpression booleanComparisonExpression)
        {
            return TreeHelper.Create<QsiLogicalExpressionNode>(n =>
            {
                n.Left.SetValue(VisitScalarExpression(booleanComparisonExpression.FirstExpression));
                n.Right.SetValue(VisitScalarExpression(booleanComparisonExpression.SecondExpression));

                n.Operator = booleanComparisonExpression.ComparisonType switch
                {
                    BooleanComparisonType.Equals => "=",
                    BooleanComparisonType.GreaterThan => ">",
                    BooleanComparisonType.GreaterThanOrEqualTo => ">=",
                    BooleanComparisonType.LeftOuterJoin => "*=",
                    BooleanComparisonType.LessThan => "<",
                    BooleanComparisonType.LessThanOrEqualTo => "<=",
                    BooleanComparisonType.NotEqualToBrackets => "<>",
                    BooleanComparisonType.NotEqualToExclamation => "!=",
                    BooleanComparisonType.NotGreaterThan => "!>",
                    BooleanComparisonType.NotLessThan => "!<",
                    BooleanComparisonType.RightOuterJoin => "=*",
                    _ => throw new InvalidOperationException()
                };
            });
        }

        private QsiLogicalExpressionNode VisitBooleanIsNullExpression(BooleanIsNullExpression booleanIsNullExpression)
        {
            return TreeHelper.Create<QsiLogicalExpressionNode>(n =>
            {
                n.Left.SetValue(VisitScalarExpression(booleanIsNullExpression.Expression));

                n.Right.SetValue(new QsiLiteralExpressionNode
                {
                    Value = null,
                    Type = QsiLiteralType.Null
                });

                n.Operator = booleanIsNullExpression.IsNot ? "!=" : "=";
            });
        }

        private QsiUnaryExpressionNode VisitBooleanNotExpression(BooleanNotExpression booleanNotExpression)
        {
            return TreeHelper.Create<QsiUnaryExpressionNode>(n =>
            {
                n.Operator = SqlServerKnownOperator.Not;
                n.Expression.SetValue(VisitBooleanExpression(booleanNotExpression.Expression));
            });
        }

        private QsiInvokeExpressionNode VisitExistsPredicate(ExistsPredicate existsPredicate)
        {
            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunctionAccess(SqlServerKnownFunction.Exists));
                n.Parameters.Add(VisitScalarSubquery(existsPredicate.Subquery));
            });
        }

        private QsiExpressionNode VisitBooleanTernaryExpression(BooleanTernaryExpression booleanTernaryExpression)
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
                BooleanTernaryExpressionType.Between => "BETWEEN",
                BooleanTernaryExpressionType.NotBetween => "NOT BETWEEN",
                _ => throw new InvalidOperationException()
            };

            return TreeHelper.CreateUnary(operand, invoke);
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
                    return VisitExtractFromExpression(extractFromExpression);

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

        private QsiLogicalExpressionNode VisitBinaryExpression(BinaryExpression binaryExpression)
        {
            return TreeHelper.Create<QsiLogicalExpressionNode>(n =>
            {
                n.Left.SetValue(VisitScalarExpression(binaryExpression.FirstExpression));
                n.Right.SetValue(VisitScalarExpression(binaryExpression.SecondExpression));

                n.Operator = binaryExpression.BinaryExpressionType switch
                {
                    BinaryExpressionType.Add => "+",
                    BinaryExpressionType.Divide => "/",
                    BinaryExpressionType.Modulo => "%",
                    BinaryExpressionType.Multiply => "*",
                    BinaryExpressionType.Subtract => "-",
                    BinaryExpressionType.BitwiseAnd => "&",
                    BinaryExpressionType.BitwiseOr => "|",
                    BinaryExpressionType.BitwiseXor => "^",
                    _ => throw new InvalidOperationException()
                };
            });
        }

        private QsiUnaryExpressionNode VisitUnaryExpression(UnaryExpression unaryExpression)
        {
            var expressionType = unaryExpression.UnaryExpressionType switch
            {
                UnaryExpressionType.Positive => "+",
                UnaryExpressionType.Negative => "-",
                UnaryExpressionType.BitwiseNot => "~",
                _ => throw new InvalidOperationException()
            };

            return TreeHelper.CreateUnary(expressionType, VisitScalarExpression(unaryExpression.Expression));
        }

        private QsiExpressionNode VisitSourceDeclaration(SourceDeclaration sourceDeclaration)
        {
            throw TreeHelper.NotSupportedFeature("source declaration");
        }

        private QsiExpressionNode VisitScalarExpressionSnippet(ScalarExpressionSnippet scalarExpressionSnippet)
        {
            throw TreeHelper.NotSupportedFeature("expresssion snippet");
        }

        #region Primary Expression
        private QsiExpressionNode VisitPrimaryExpression(PrimaryExpression primaryExpression)
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
                
                case UserDefinedTypePropertyAccess userDefinedTypePropertyAccess:
                    break;

                case OdbcFunctionCall odbcFunctionCall:
                    break;

                case ParameterlessCall parameterlessCall:
                    break;

                case PartitionFunctionCall partitionFunctionCall:
                    break;
            }

            throw TreeHelper.NotSupportedTree(primaryExpression);
        }

        #region CaseExpression
        private QsiSwitchExpressionNode VisitCaseExpression(CaseExpression caseExpression)
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

        private QsiSwitchExpressionNode VisitSearchedCaseExpression(SearchedCaseExpression searchedCaseExpression)
        {
            return TreeHelper.Create<QsiSwitchExpressionNode>(n =>
            {
                n.Cases.AddRange(searchedCaseExpression.WhenClauses.Select(VisitSearchedWhenClause));

                n.Cases.Add(TreeHelper.Create<QsiSwitchCaseExpressionNode>(en =>
                {
                    en.Consequent.SetValue(VisitScalarExpression(searchedCaseExpression.ElseExpression));
                }));
            });
        }

        private QsiSwitchCaseExpressionNode VisitSearchedWhenClause(SearchedWhenClause searchedWhenClause)
        {
            return TreeHelper.Create<QsiSwitchCaseExpressionNode>(n =>
            {
                n.Condition.SetValue(VisitBooleanExpression(searchedWhenClause.WhenExpression));
                n.Consequent.SetValue(VisitScalarExpression(searchedWhenClause.ThenExpression));
            });
        }

        private QsiSwitchExpressionNode VisitSimpleCaseExpression(SimpleCaseExpression simpleCaseExpression)
        {
            return TreeHelper.Create<QsiSwitchExpressionNode>(n =>
            {
                n.Value.SetValue(VisitScalarExpression(simpleCaseExpression.InputExpression));

                n.Cases.AddRange(simpleCaseExpression.WhenClauses.Select(VisitSimpleWhenClause));

                n.Cases.Add(TreeHelper.Create<QsiSwitchCaseExpressionNode>(en =>
                {
                    en.Consequent.SetValue(VisitScalarExpression(simpleCaseExpression.ElseExpression));
                }));
            });
        }

        private QsiSwitchCaseExpressionNode VisitSimpleWhenClause(SimpleWhenClause simpleWhenClause)
        {
            return TreeHelper.Create<QsiSwitchCaseExpressionNode>(n =>
            {
                n.Condition.SetValue(VisitScalarExpression(simpleWhenClause.WhenExpression));
                n.Consequent.SetValue(VisitScalarExpression(simpleWhenClause.ThenExpression));
            });
        }
        #endregion

        internal QsiColumnExpressionNode VisitColumnReferenceExpression(ColumnReferenceExpression columnReferenceExpression)
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

        private QsiTableExpressionNode VisitScalarSubquery(ScalarSubquery scalarSubquery)
        {
            return TreeHelper.Create<QsiTableExpressionNode>(n =>
            {
                n.Table.SetValue(TableVisitor.VisitQueryExpression(scalarSubquery.QueryExpression));
            });
        }

        #region ValueExpression
        internal QsiExpressionNode VisitValueExpression(ValueExpression valueExpression)
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
        internal QsiLiteralExpressionNode VisitLiteral(Literal literal)
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
        internal QsiExpressionNode VisitVariableReference(VariableReference variableReference)
        {
            return TreeHelper.Create<QsiVariableAccessExpressionNode>(n =>
            {
                n.Identifier = new QsiQualifiedIdentifier(new QsiIdentifier(variableReference.Name, false));
            });
        }

        // TODO: Impl variable
        private QsiExpressionNode VisitGlobalVariableExpression(GlobalVariableExpression globalVariableExpression)
        {
            return TreeHelper.Create<QsiVariableAccessExpressionNode>(n =>
            {
                n.Identifier = new QsiQualifiedIdentifier(new QsiIdentifier(globalVariableExpression.Name, false));
            });
        }
        #endregion
        #endregion
        #endregion

        private QsiExpressionNode VisitOdbcConvertSpecification(OdbcConvertSpecification odbcConvertSpecification)
        {
            throw TreeHelper.NotSupportedFeature("odbc convert specification");
        }

        private QsiExpressionNode VisitIdentityFunctionCall(IdentityFunctionCall identityFunctionCall)
        {
            throw new System.NotImplementedException();
        }

        private QsiExpressionNode VisitExtractFromExpression(ExtractFromExpression extractFromExpression)
        {
            throw new System.NotImplementedException();
        }
        #endregion

        internal QsiInvokeExpressionNode CreateInvokeExpression(string functionName, IEnumerable<TSqlFragment> parameters)
            => CreateInvokeExpression(functionName, parameters.ToArray());

        internal QsiInvokeExpressionNode CreateInvokeExpression(QsiQualifiedIdentifier qualifiedIdentifier, IEnumerable<TSqlFragment> parameters)
            => CreateInvokeExpression(qualifiedIdentifier, parameters.ToArray());

        internal QsiInvokeExpressionNode CreateInvokeExpression(string functionName, params TSqlFragment[] parameters)
            => CreateInvokeExpression(TreeHelper.CreateFunctionAccess(functionName), parameters);

        internal QsiInvokeExpressionNode CreateInvokeExpression(QsiQualifiedIdentifier qualifiedIdentifier, params TSqlFragment[] parameters)
        {
            var functionAccessExpressionNode = new QsiFunctionAccessExpressionNode
            {
                Identifier = qualifiedIdentifier
            };
            
            return CreateInvokeExpression(functionAccessExpressionNode, parameters);
        }

        internal QsiInvokeExpressionNode CreateInvokeExpression(QsiFunctionAccessExpressionNode functionAccessExpressionNode, params TSqlFragment[] parameters)
        {
            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(functionAccessExpressionNode);

                n.Parameters.AddRange(parameters
                    .Where(p => p != null)
                    .Select(p =>
                    {
                        return p switch
                        {
                            BooleanExpression booleanExpression => VisitBooleanExpression(booleanExpression),
                            ScalarExpression scalarExpression => VisitScalarExpression(scalarExpression),
                            DataTypeReference dataTypeReference => new QsiTypeAccessExpressionNode
                            {
                                Identifier = IdentifierVisitor.CreateQualifiedIdentifier(dataTypeReference.Name)
                            },
                            MultiPartIdentifier multiPartIdentifier => new QsiVariableAccessExpressionNode
                            {
                                Identifier = IdentifierVisitor.CreateQualifiedIdentifier(multiPartIdentifier)
                            },
                            OverClause _ => throw TreeHelper.NotSupportedFeature("over clause"),
                            _ => throw new InvalidOperationException()
                        };
                    })
                );
            });
        }
    }
}
