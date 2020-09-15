using System;
using System.Linq;
using System.Text.RegularExpressions;
using ikvm.extensions;
using net.sf.jsqlparser.expression;
using net.sf.jsqlparser.expression.operators.relational;
using net.sf.jsqlparser.schema;
using net.sf.jsqlparser.statement.@select;
using net.sf.jsqlparser.util.cnfexpression;
using Qsi.Data;
using Qsi.JSql.Extensions;
using Qsi.Tree.Base;
using Qsi.Utilities;

namespace Qsi.JSql.Tree
{
    public class JSqlExpressionVisitor : JSqlVisitorBase
    {
        private static readonly Regex _dateTimePattern = new Regex("(?<=')[^']+(?=')");

        public JSqlExpressionVisitor(IJSqlVisitorContext context) : base(context)
        {
        }

        public virtual QsiExpressionNode Visit(Expression expression)
        {
            switch (expression)
            {
                case StringValue stringValue:
                    return VisitStringValue(stringValue);

                case DateTimeLiteralExpression dateTimeLiteralExpression:
                    return VisitDateTimeLiteralExpression(dateTimeLiteralExpression);

                case AllComparisonExpression allComparisonExpression:
                    return VisitAllComparisonExpression(allComparisonExpression);

                case AnalyticExpression analyticExpression:
                    return VisitAnalyticExpression(analyticExpression);

                case AnyComparisonExpression anyComparisonExpression:
                    return VisitAnyComparisonExpression(anyComparisonExpression);

                case ArrayExpression arrayExpression:
                    return VisitArrayExpression(arrayExpression);

                case BinaryExpression binaryExpression:
                    return VisitBinaryExpression(binaryExpression);

                case CaseExpression caseExpression:
                    return VisitCaseExpression(caseExpression);

                case CastExpression castExpression:
                    return VisitCastExpression(castExpression);

                case CollateExpression collateExpression:
                    return VisitCollateExpression(collateExpression);

                case DateValue dateValue:
                    return VisitDateValue(dateValue);

                case DoubleValue doubleValue:
                    return VisitDoubleValue(doubleValue);

                case ExtractExpression extractExpression:
                    return VisitExtractExpression(extractExpression);

                case Function function:
                    return VisitFunction(function);

                case HexValue hexValue:
                    return VisitHexValue(hexValue);

                case IntervalExpression intervalExpression:
                    return VisitIntervalExpression(intervalExpression);

                case JdbcNamedParameter jdbcNamedParameter:
                    return VisitJdbcNamedParameter(jdbcNamedParameter);

                case JdbcParameter jdbcParameter:
                    return VisitJdbcParameter(jdbcParameter);

                case JsonExpression jsonExpression:
                    return VisitJsonExpression(jsonExpression);

                case KeepExpression keepExpression:
                    return VisitKeepExpression(keepExpression);

                case LongValue longValue:
                    return VisitLongValue(longValue);

                case MySQLGroupConcat mySQLGroupConcat:
                    return VisitMySQLGroupConcat(mySQLGroupConcat);

                case NextValExpression nextValExpression:
                    return VisitNextValExpression(nextValExpression);

                case NotExpression notExpression:
                    return VisitNotExpression(notExpression);

                case NullValue _:
                    return VisitNullValue();

                case NumericBind numericBind:
                    return VisitNumericBind(numericBind);

                case OracleHierarchicalExpression oracleHierarchicalExpression:
                    return VisitOracleHierarchicalExpression(oracleHierarchicalExpression);

                case OracleHint oracleHint:
                    return VisitOracleHint(oracleHint);

                case Parenthesis parenthesis:
                    return VisitParenthesis(parenthesis);

                case RowConstructor rowConstructor:
                    return VisitRowConstructor(rowConstructor);

                case SignedExpression signedExpression:
                    return VisitSignedExpression(signedExpression);

                case TimeKeyExpression timeKeyExpression:
                    return VisitTimeKeyExpression(timeKeyExpression);

                case TimeValue timeValue:
                    return VisitTimeValue(timeValue);

                case TimestampValue timestampValue:
                    return VisitTimestampValue(timestampValue);

                case UserVariable userVariable:
                    return VisitUserVariable(userVariable);

                case ValueListExpression valueListExpression:
                    return VisitValueListExpression(valueListExpression);

                case VariableAssignment variableAssignment:
                    return VisitVariableAssignment(variableAssignment);

                case WhenClause whenClause:
                    return VisitWhenClause(whenClause);

                case XMLSerializeExpr xMLSerializeExpr:
                    return VisitXMLSerializeExpr(xMLSerializeExpr);

                case Between between:
                    return VisitBetween(between);

                case ExistsExpression existsExpression:
                    return VisitExistsExpression(existsExpression);

                case FullTextSearch fullTextSearch:
                    return VisitFullTextSearch(fullTextSearch);

                case InExpression inExpression:
                    return VisitInExpression(inExpression);

                case IsBooleanExpression isBooleanExpression:
                    return VisitIsBooleanExpression(isBooleanExpression);

                case IsNullExpression isNullExpression:
                    return VisitIsNullExpression(isNullExpression);

                case Column column:
                    return VisitColumn(column);

                case SubSelect subSelect:
                    return VisitSubSelect(subSelect);

                case MultipleExpression multipleExpression:
                    return VisitMultipleExpression(multipleExpression);
            }

            throw TreeHelper.NotSupportedTree(expression);
        }

        public virtual QsiLiteralExpressionNode VisitNullValue()
        {
            return TreeHelper.CreateNullLiteral();
        }

        public virtual QsiLiteralExpressionNode VisitStringValue(StringValue expression)
        {
            return TreeHelper.CreateLiteral(expression.toString());
        }

        public virtual QsiLiteralExpressionNode VisitLongValue(LongValue expression)
        {
            return TreeHelper.CreateLiteral(expression.getValue());
        }

        public virtual QsiLiteralExpressionNode VisitDoubleValue(DoubleValue expression)
        {
            return TreeHelper.CreateLiteral(expression.getValue());
        }

        public virtual QsiExpressionNode VisitHexValue(HexValue expression)
        {
            return TreeHelper.CreateLiteral(expression.getValue(), QsiLiteralType.Hexadecimal);
        }

        // {d 'yyyy-mm-dd'}
        public virtual QsiLiteralExpressionNode VisitDateValue(DateValue expression)
        {
            var value = _dateTimePattern.Match(expression.toString()).Value;
            return TreeHelper.CreateLiteral(DateTime.Parse(value), QsiLiteralType.Date);
        }

        // {t 'hh:mm:ss'}
        public virtual QsiLiteralExpressionNode VisitTimeValue(TimeValue expression)
        {
            var value = _dateTimePattern.Match(expression.toString()).Value;
            return TreeHelper.CreateLiteral(TimeSpan.Parse(value), QsiLiteralType.Time);
        }

        // {ts 'yyyy-mm-dd hh:mm:ss.f . . .'}
        public virtual QsiLiteralExpressionNode VisitTimestampValue(TimestampValue expression)
        {
            var value = _dateTimePattern.Match(expression.toString()).Value;
            return TreeHelper.CreateLiteral(DateTimeOffset.Parse(value), QsiLiteralType.DateTimeOffset);
        }

        public virtual QsiLiteralExpressionNode VisitDateTimeLiteralExpression(DateTimeLiteralExpression expression)
        {
            QsiLiteralType literalType;

            switch (expression.getType())
            {
                case var t when t == DateTimeLiteralExpression.DateTime.TIME:
                    literalType = QsiLiteralType.DateTime;
                    break;

                case var t when t == DateTimeLiteralExpression.DateTime.DATE:
                    literalType = QsiLiteralType.Date;
                    break;

                default:
                    literalType = QsiLiteralType.DateTimeOffset;
                    break;
            }

            return TreeHelper.CreateLiteral(expression.getValue(), literalType);
        }

        public virtual QsiInvokeExpressionNode VisitAllComparisonExpression(AllComparisonExpression expression)
        {
            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunctionAccess(JSqlKnownFunction.All));
                n.Parameters.Add(VisitSubSelect(expression.getSubSelect()));
            });
        }

        public virtual QsiInvokeExpressionNode VisitAnyComparisonExpression(AnyComparisonExpression expression)
        {
            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunctionAccess(JSqlKnownFunction.Any));
                n.Parameters.Add(VisitSubSelect(expression.getSubSelect()));
            });
        }

        public virtual QsiInvokeExpressionNode VisitAnalyticExpression(AnalyticExpression expression)
        {
            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(new QsiFunctionAccessExpressionNode
                {
                    Identifier = IdentifierVisitor.VisitAnalyticExpression(expression)
                });

                if (expression.getExpression() != null)
                {
                    n.Parameters.Add(Visit(expression.getExpression()));
                }
            });
        }

        public virtual QsiArrayRankExpressionNode VisitArrayExpression(ArrayExpression expression)
        {
            return TreeHelper.Create<QsiArrayRankExpressionNode>(n =>
            {
                n.Array.SetValue(Visit(expression.getObjExpression()));
                n.Rank.SetValue(Visit(expression.getIndexExpression()));
            });
        }

        public virtual QsiLogicalExpressionNode VisitBinaryExpression(BinaryExpression expression)
        {
            return TreeHelper.Create<QsiLogicalExpressionNode>(n =>
            {
                n.Left.SetValue(Visit(expression.getLeftExpression()));
                n.Operator = expression.getStringExpression();
                n.Right.SetValue(Visit(expression.getRightExpression()));
            });
        }

        public virtual QsiSwitchExpressionNode VisitCaseExpression(CaseExpression expression)
        {
            return TreeHelper.Create<QsiSwitchExpressionNode>(n =>
            {
                if (expression.getSwitchExpression() != null)
                    n.Value.SetValue(Visit(expression.getSwitchExpression()));

                n.Cases.AddRange(expression.getWhenClauses()
                    .AsEnumerable<WhenClause>()
                    .Select(VisitWhenClause));

                if (expression.getElseExpression() != null)
                {
                    n.Cases.Add(TreeHelper.Create<QsiSwitchCaseExpressionNode>(en =>
                    {
                        en.Consequent.SetValue(Visit(expression.getElseExpression()));
                    }));
                }
            });
        }

        public virtual QsiSwitchCaseExpressionNode VisitWhenClause(WhenClause expression)
        {
            return TreeHelper.Create<QsiSwitchCaseExpressionNode>(n =>
            {
                n.Condition.SetValue(Visit(expression.getWhenExpression()));
                n.Consequent.SetValue(Visit(expression.getThenExpression()));
            });
        }

        public virtual QsiInvokeExpressionNode VisitCastExpression(CastExpression expression)
        {
            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunctionAccess(JSqlKnownFunction.Cast));
                n.Parameters.Add(Visit(expression.getLeftExpression()));

                n.Parameters.Add(new QsiTypeAccessExpressionNode
                {
                    Identifier = new QsiQualifiedIdentifier(new QsiIdentifier(expression.getType().toString(), false))
                });
            });
        }

        public virtual QsiInvokeExpressionNode VisitCollateExpression(CollateExpression expression)
        {
            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunctionAccess(JSqlKnownFunction.Collate));
                n.Parameters.Add(Visit(expression.getLeftExpression()));
                n.Parameters.Add(TreeHelper.CreateLiteral(expression.getCollate()));
            });
        }

        public virtual QsiInvokeExpressionNode VisitExtractExpression(ExtractExpression expression)
        {
            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunctionAccess(JSqlKnownFunction.Extract));

                n.Parameters.Add(TreeHelper.Create<QsiLogicalExpressionNode>(ln =>
                {
                    ln.Left.SetValue(TreeHelper.CreateLiteral(expression.getName()));
                    ln.Operator = JSqlKnownOperator.From;
                    ln.Right.SetValue(Visit(expression.getExpression()));
                }));
            });
        }

        public virtual QsiInvokeExpressionNode VisitFunction(Function expression)
        {
            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(new QsiFunctionAccessExpressionNode
                {
                    Identifier = IdentifierVisitor.VisitFunction(expression)
                });

                if (expression.getParameters() != null)
                {
                    n.Parameters.AddRange(
                        expression.getParameters().getExpressions()
                            .AsEnumerable<Expression>()
                            .Select(Visit));
                }

                if (expression.getNamedParameters() != null)
                {
                    n.Parameters.Add(VisitNamedExpressionList(expression.getNamedParameters()));
                }

                if (expression.isAllColumns())
                {
                    n.Parameters.Add(TreeHelper.Create<QsiColumnExpressionNode>(cn =>
                    {
                        cn.Column.SetValue(new QsiAllColumnNode());
                    }));
                }

                // TODO: Attribute
                // TODO: Keep
            });
        }

        public virtual QsiInvokeExpressionNode VisitIntervalExpression(IntervalExpression expression)
        {
            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunctionAccess(JSqlKnownFunction.Cast));

                if (expression.getParameter() != null)
                    n.Parameters.Add(TreeHelper.CreateLiteral(expression.getParameter()));

                if (expression.getExpression() != null)
                    n.Parameters.Add(Visit(expression.getExpression()));

                n.Parameters.Add(TreeHelper.Create<QsiTypeAccessExpressionNode>(tn =>
                {
                    tn.Identifier = new QsiQualifiedIdentifier(
                        new QsiIdentifier("INTERVAL", false),
                        new QsiIdentifier(expression.getIntervalType(), false)
                    );
                }));
            });
        }

        public virtual QsiExpressionNode VisitNumericBind(NumericBind expression)
        {
            return TreeHelper.Create<QsiColumnExpressionNode>(n =>
            {
                n.Column.SetValue(TableVisitor.VisitNumericBind(expression));
            });
        }

        public virtual QsiExpressionNode VisitJdbcParameter(JdbcParameter expression)
        {
            return TreeHelper.Create<QsiColumnExpressionNode>(n =>
            {
                n.Column.SetValue(TableVisitor.VisitJdbcParameter(expression));
            });
        }

        public virtual QsiExpressionNode VisitJdbcNamedParameter(JdbcNamedParameter expression)
        {
            return TreeHelper.Create<QsiColumnExpressionNode>(n =>
            {
                n.Column.SetValue(TableVisitor.VisitJdbcNamedParameter(expression));
            });
        }

        public virtual QsiExpressionNode VisitJsonExpression(JsonExpression expression)
        {
            throw TreeHelper.NotSupportedFeature(expression.GetType().Name);
        }

        public virtual QsiExpressionNode VisitKeepExpression(KeepExpression expression)
        {
            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunctionAccess(JSqlKnownFunction.Keep));
                n.Parameters.Add(TreeHelper.CreateLiteral(expression.isFirst() ? "FIRST" : "LAST"));

                if (expression.getOrderByElements() != null)
                {
                    n.Parameters.AddRange(expression.getOrderByElements()
                        .AsEnumerable<OrderByElement>()
                        .Select(VisitOrderByElement));
                }
            });
        }

        public virtual QsiExpressionNode VisitOrderByElement(OrderByElement expression)
        {
            return TreeHelper.Create<QsiMultipleExpressionNode>(n =>
            {
                n.Elements.Add(Visit(expression.getExpression()));
                n.Elements.Add(TreeHelper.CreateLiteral(expression.isAsc() ? "ASC" : "DESC"));

                if (expression.getNullOrdering() != null)
                    n.Elements.Add(VisitNullOrdering(expression.getNullOrdering()));
            });
        }

        public virtual QsiExpressionNode VisitNullOrdering(OrderByElement.NullOrdering nullOrdering)
        {
            return TreeHelper.CreateLiteral(
                nullOrdering == OrderByElement.NullOrdering.NULLS_FIRST ? "NULLS FIRST" : "NULLS LAST");
        }

        public virtual QsiExpressionNode VisitMySQLGroupConcat(MySQLGroupConcat expression)
        {
            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunctionAccess(JSqlKnownFunction.GroupConcat));

                if (expression.isDistinct())
                    n.Parameters.Add(TreeHelper.CreateLiteral("DISTINCT"));

                n.Parameters.Add(VisitExpressionList(expression.getExpressionList()));

                if (expression.getOrderByElements() != null)
                {
                    n.Parameters.Add(TreeHelper.Create<QsiMultipleExpressionNode>(mn =>
                    {
                        mn.Elements.AddRange(expression.getOrderByElements()
                            .AsEnumerable<OrderByElement>()
                            .Select(VisitOrderByElement));
                    }));
                }

                if (expression.getSeparator() != null)
                    n.Parameters.Add(TreeHelper.CreateLiteral(expression.getSeparator()));
            });
        }

        public virtual QsiExpressionNode VisitNextValExpression(NextValExpression expression)
        {
            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunctionAccess(JSqlKnownFunction.NextValFor));
                n.Parameters.Add(TreeHelper.CreateLiteral(expression.getName()));
            });
        }

        public virtual QsiUnaryExpressionNode VisitNotExpression(NotExpression expression)
        {
            return TreeHelper.Create<QsiUnaryExpressionNode>(n =>
            {
                n.Operator = JSqlKnownOperator.Not;
                n.Expression.SetValue(Visit(expression.getExpression()));
            });
        }

        public virtual QsiExpressionNode VisitOracleHierarchicalExpression(OracleHierarchicalExpression expression)
        {
            throw TreeHelper.NotSupportedFeature("Hierarchical expression");
        }

        public virtual QsiExpressionNode VisitOracleHint(OracleHint expression)
        {
            throw TreeHelper.NotSupportedFeature("Hint expression");
        }

        public virtual QsiExpressionNode VisitParenthesis(Parenthesis expression)
        {
            return Visit(expression.getExpression());
        }

        public virtual QsiExpressionNode VisitRowConstructor(RowConstructor expression)
        {
            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(new QsiFunctionAccessExpressionNode
                {
                    Identifier = new QsiQualifiedIdentifier(IdentifierUtility.Parse(expression.getName()))
                });

                n.Parameters.AddRange(expression.getExprList().getExpressions()
                    .AsEnumerable<Expression>()
                    .Select(Visit));
            });
        }

        public virtual QsiExpressionNode VisitSignedExpression(SignedExpression expression)
        {
            return TreeHelper.CreateUnary(expression.getSign().ToString(), Visit(expression.getExpression()));
        }

        public virtual QsiExpressionNode VisitTimeKeyExpression(TimeKeyExpression expression)
        {
            // TODO: Enum? Constant?
            return TreeHelper.CreateLiteral(expression.getStringValue());
        }

        public virtual QsiExpressionNode VisitUserVariable(UserVariable expression)
        {
            return TreeHelper.Create<QsiVariableAccessExpressionNode>(n =>
            {
                n.Identifier = new QsiQualifiedIdentifier(new QsiIdentifier(expression.toString(), false));
            });
        }

        public virtual QsiExpressionNode VisitValueListExpression(ValueListExpression expression)
        {
            // ( <EXPRESSION_LIST> )
            return VisitExpressionList(expression.getExpressionList());
        }

        public virtual QsiExpressionNode VisitVariableAssignment(VariableAssignment expression)
        {
            return TreeHelper.Create<QsiLogicalExpressionNode>(n =>
            {
                n.Left.SetValue(VisitUserVariable(expression.getVariable()));
                n.Operator = expression.getOperation();
                n.Right.SetValue(Visit(expression.getExpression()));
            });
        }

        public virtual QsiExpressionNode VisitXMLSerializeExpr(XMLSerializeExpr expression)
        {
            throw TreeHelper.NotSupportedFeature("XML Serialize");
        }

        public virtual QsiExpressionNode VisitBetween(Between expression)
        {
            var invoke = TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunctionAccess(JSqlKnownFunction.Between));

                n.Parameters.Add(Visit(expression.getLeftExpression()));
                n.Parameters.Add(Visit(expression.getBetweenExpressionStart()));
                n.Parameters.Add(Visit(expression.getBetweenExpressionEnd()));
            });

            if (expression.isNot())
                return TreeHelper.CreateUnary(JSqlKnownOperator.Not, invoke);

            return invoke;
        }

        public virtual QsiExpressionNode VisitExistsExpression(ExistsExpression expression)
        {
            var invoke = TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunctionAccess(JSqlKnownFunction.Exists));
                n.Parameters.Add(Visit(expression.getRightExpression()));
            });

            if (expression.isNot())
                return TreeHelper.CreateUnary(JSqlKnownOperator.Not, invoke);

            return invoke;
        }

        // MATCH (<COLUMNS..>) AGAINST (<SEARCH VALUE> [<SEARCH MODE>])
        // -> MATCH_AGAINST(<SEARCH MODE>, [<SEARCH MODE>], <COLUMNS..>)
        public virtual QsiInvokeExpressionNode VisitFullTextSearch(FullTextSearch expression)
        {
            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunctionAccess(JSqlKnownFunction.MatchAgainst));

                n.Parameters.Add(VisitStringValue(expression.getAgainstValue()));

                if (expression.getSearchModifier() != null)
                    n.Parameters.Add(TreeHelper.CreateLiteral(expression.getSearchModifier()));

                n.Parameters.Add(TreeHelper.Create<QsiMultipleExpressionNode>(mn =>
                {
                    mn.Elements.AddRange(expression.getMatchColumns()
                        .AsEnumerable<Column>()
                        .Select(VisitColumn));
                }));
            });
        }

        public virtual QsiLogicalExpressionNode VisitInExpression(InExpression expression)
        {
            return TreeHelper.Create<QsiLogicalExpressionNode>(n =>
            {
                n.Left.SetValue(expression.getLeftExpression() == null ?
                    VisitItemsList(expression.getLeftItemsList()) :
                    Visit(expression.getLeftExpression()));

                n.Operator = expression.isNot() ? "NOT IN" : "IN";

                if (expression.getMultiExpressionList() != null)
                {
                    n.Right.SetValue(VisitMultipleExpressionList(expression.getMultiExpressionList()));
                }
                else if (expression.getRightExpression() != null)
                {
                    n.Right.SetValue(Visit(expression.getRightExpression()));
                }
                else
                {
                    n.Right.SetValue(VisitItemsList(expression.getRightItemsList()));
                }
            });
        }

        // <L.Expr> IS NOT TRUE
        // -> IS_NOT_TRUE(<L.Expr>) 
        public virtual QsiExpressionNode VisitIsBooleanExpression(IsBooleanExpression expression)
        {
            var invoke = TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunctionAccess(
                    expression.isTrue() ? JSqlKnownFunction.IsTrue : JSqlKnownFunction.IsFalse));

                n.Parameters.Add(Visit(expression.getLeftExpression()));
            });

            if (expression.isNot())
                return TreeHelper.CreateUnary(JSqlKnownOperator.Not, invoke);

            return invoke;
        }

        // <L.Expr> IS NOT NULL
        // -> IS_NOT_NULL(<L.Expr>)
        public virtual QsiExpressionNode VisitIsNullExpression(IsNullExpression expression)
        {
            var invoke = TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunctionAccess(JSqlKnownFunction.IsNull));
                n.Parameters.Add(Visit(expression.getLeftExpression()));
            });

            if (expression.isNot())
                return TreeHelper.CreateUnary(JSqlKnownOperator.Not, invoke);

            return invoke;
        }

        public virtual QsiExpressionNode VisitColumn(Column expression)
        {
            return TreeHelper.Create<QsiColumnExpressionNode>(n =>
            {
                n.Column.SetValue(TableVisitor.VisitColumn(expression));
            });
        }

        public virtual QsiTableExpressionNode VisitSubSelect(SubSelect expression)
        {
            return TreeHelper.Create<QsiTableExpressionNode>(n =>
            {
                n.Table.SetValue(TableVisitor.VisitSubSelect(expression));
            });
        }

        public virtual QsiMultipleExpressionNode VisitMultipleExpression(MultipleExpression expression)
        {
            return TreeHelper.Create<QsiMultipleExpressionNode>(n =>
            {
                n.Elements.AddRange(expression.getList()
                    .AsEnumerable<Expression>()
                    .Select(Visit));
            });
        }

        public virtual QsiMultipleExpressionNode VisitItemsList(ItemsList itemsList)
        {
            switch (itemsList)
            {
                case ExpressionList expressionList:
                    return VisitExpressionList(expressionList);

                case MultiExpressionList multiExpressionList:
                    return VisitMultipleExpressionList(multiExpressionList);

                case NamedExpressionList namedExpressionList:
                    return VisitNamedExpressionList(namedExpressionList);
            }

            throw TreeHelper.NotSupportedTree(itemsList);
        }

        public virtual QsiMultipleExpressionNode VisitExpressionList(ExpressionList expression)
        {
            return TreeHelper.Create<QsiMultipleExpressionNode>(n =>
            {
                n.Elements.AddRange(expression.getExpressions()
                    .AsEnumerable<Expression>()
                    .Select(Visit));
            });
        }

        public virtual QsiMultipleExpressionNode VisitMultipleExpressionList(MultiExpressionList expression)
        {
            return TreeHelper.Create<QsiMultipleExpressionNode>(n =>
            {
                n.Elements.AddRange(expression.getExpressionLists()
                    .AsEnumerable<ExpressionList>()
                    .Select(VisitExpressionList));
            });
        }

        // ('xyzzy' from 2 for 3)
        public virtual QsiMultipleExpressionNode VisitNamedExpressionList(NamedExpressionList expression)
        {
            return TreeHelper.Create<QsiMultipleExpressionNode>(n =>
            {
                // Skip names
                n.Elements.AddRange(
                    expression.getExpressions()
                        .AsEnumerable<Expression>()
                        .Select(Visit));
            });
        }
    }
}
