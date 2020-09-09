using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

                case NullValue nullValue:
                    return VisitNullValue(nullValue);

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

        public virtual QsiExpressionNode VisitNullValue(NullValue expression)
        {
            return TreeHelper.CreateNullLiteral();
        }

        public virtual QsiExpressionNode VisitStringValue(StringValue expression)
        {
            return TreeHelper.CreateLiteral(expression.getValue());
        }

        public virtual QsiExpressionNode VisitLongValue(LongValue expression)
        {
            return TreeHelper.CreateLiteral(expression.getValue());
        }

        public virtual QsiExpressionNode VisitDoubleValue(DoubleValue expression)
        {
            return TreeHelper.CreateLiteral(expression.getValue());
        }

        // {d 'yyyy-mm-dd'}
        public virtual QsiExpressionNode VisitDateValue(DateValue expression)
        {
            var value = _dateTimePattern.Match(expression.toString()).Value;
            return TreeHelper.CreateLiteral(DateTime.Parse(value), QsiLiteralType.Date);
        }

        // {t 'hh:mm:ss'}
        public virtual QsiExpressionNode VisitTimeValue(TimeValue expression)
        {
            var value = _dateTimePattern.Match(expression.toString()).Value;
            return TreeHelper.CreateLiteral(TimeSpan.Parse(value), QsiLiteralType.Time);
        }

        // {ts 'yyyy-mm-dd hh:mm:ss.f . . .'}
        public virtual QsiExpressionNode VisitTimestampValue(TimestampValue expression)
        {
            var value = _dateTimePattern.Match(expression.toString()).Value;
            return TreeHelper.CreateLiteral(DateTimeOffset.Parse(value), QsiLiteralType.DateTimeOffset);
        }

        public virtual QsiExpressionNode VisitDateTimeLiteralExpression(DateTimeLiteralExpression expression)
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

        public virtual QsiExpressionNode VisitAllComparisonExpression(AllComparisonExpression expression)
        {
            throw new NotImplementedException();
        }

        public virtual QsiExpressionNode VisitAnalyticExpression(AnalyticExpression expression)
        {
            throw new NotImplementedException();
        }

        public virtual QsiExpressionNode VisitAnyComparisonExpression(AnyComparisonExpression expression)
        {
            throw new NotImplementedException();
        }

        public virtual QsiExpressionNode VisitArrayExpression(ArrayExpression expression)
        {
            return TreeHelper.Create<QsiArrayRankExpressionNode>(n =>
            {
                n.Array.SetValue(Visit(expression.getObjExpression()));
                n.Rank.SetValue(Visit(expression.getIndexExpression()));
            });
        }

        public virtual QsiExpressionNode VisitBinaryExpression(BinaryExpression expression)
        {
            return TreeHelper.Create<QsiLogicalExpressionNode>(n =>
            {
                n.Left.SetValue(Visit(expression.getLeftExpression()));
                n.Operator = expression.getStringExpression();
                n.Right.SetValue(Visit(expression.getRightExpression()));
            });
        }

        public virtual QsiExpressionNode VisitCaseExpression(CaseExpression expression)
        {
            throw new NotImplementedException();
        }

        public virtual QsiExpressionNode VisitCastExpression(CastExpression expression)
        {
            throw new NotImplementedException();
        }

        public virtual QsiExpressionNode VisitCollateExpression(CollateExpression expression)
        {
            throw new NotImplementedException();
        }

        public virtual QsiExpressionNode VisitExtractExpression(ExtractExpression expression)
        {
            throw new NotImplementedException();
        }

        public virtual QsiExpressionNode VisitFunction(Function expression)
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
                    n.Parameters.Add(new QsiColumnAccessExpressionNode
                    {
                        IsAll = true
                    });
                }

                // TODO: Attribute
                // TODO: Keep
            });
        }

        public virtual QsiExpressionNode VisitHexValue(HexValue expression)
        {
            throw new NotImplementedException();
        }

        public virtual QsiExpressionNode VisitIntervalExpression(IntervalExpression expression)
        {
            throw new NotImplementedException();
        }

        public virtual QsiExpressionNode VisitJdbcNamedParameter(JdbcNamedParameter expression)
        {
            throw new NotImplementedException();
        }

        public virtual QsiExpressionNode VisitJdbcParameter(JdbcParameter expression)
        {
            throw new NotImplementedException();
        }

        public virtual QsiExpressionNode VisitJsonExpression(JsonExpression expression)
        {
            throw TreeHelper.NotSupportedFeature("Json expression");
        }

        public virtual QsiExpressionNode VisitKeepExpression(KeepExpression expression)
        {
            throw new NotImplementedException();
        }

        public virtual QsiExpressionNode VisitMySQLGroupConcat(MySQLGroupConcat expression)
        {
            throw new NotImplementedException();
        }

        public virtual QsiExpressionNode VisitNextValExpression(NextValExpression expression)
        {
            throw new NotImplementedException();
        }

        public virtual QsiExpressionNode VisitNotExpression(NotExpression expression)
        {
            throw new NotImplementedException();
        }

        public virtual QsiExpressionNode VisitNumericBind(NumericBind expression)
        {
            throw new NotImplementedException();
        }

        public virtual QsiExpressionNode VisitOracleHierarchicalExpression(OracleHierarchicalExpression expression)
        {
            throw new NotImplementedException();
        }

        public virtual QsiExpressionNode VisitOracleHint(OracleHint expression)
        {
            throw new NotImplementedException();
        }

        public virtual QsiExpressionNode VisitParenthesis(Parenthesis expression)
        {
            throw new NotImplementedException();
        }

        public virtual QsiExpressionNode VisitRowConstructor(RowConstructor expression)
        {
            throw new NotImplementedException();
        }

        public virtual QsiExpressionNode VisitSignedExpression(SignedExpression expression)
        {
            throw new NotImplementedException();
        }

        public virtual QsiExpressionNode VisitTimeKeyExpression(TimeKeyExpression expression)
        {
            throw new NotImplementedException();
        }

        public virtual QsiExpressionNode VisitUserVariable(UserVariable expression)
        {
            throw new NotImplementedException();
        }

        public virtual QsiExpressionNode VisitValueListExpression(ValueListExpression expression)
        {
            throw new NotImplementedException();
        }

        public virtual QsiExpressionNode VisitVariableAssignment(VariableAssignment expression)
        {
            throw new NotImplementedException();
        }

        public virtual QsiExpressionNode VisitWhenClause(WhenClause expression)
        {
            throw new NotImplementedException();
        }

        public virtual QsiExpressionNode VisitXMLSerializeExpr(XMLSerializeExpr expression)
        {
            throw new NotImplementedException();
        }

        public virtual QsiExpressionNode VisitBetween(Between expression)
        {
            throw new NotImplementedException();
        }

        public virtual QsiExpressionNode VisitExistsExpression(ExistsExpression expression)
        {
            throw new NotImplementedException();
        }

        // MATCH (<COLUMNS..>) AGAINST (<SEARCH VALUE> [<SEARCH MODE>])
        // -> MATCH_AGAINST(<SEARCH MODE>, [<SEARCH MODE>], <COLUMNS..>)
        public virtual QsiExpressionNode VisitFullTextSearch(FullTextSearch expression)
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

        public virtual QsiExpressionNode VisitInExpression(InExpression expression)
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
            string methodName;

            if (expression.isTrue())
                methodName = expression.isNot() ? JSqlKnownFunction.IsNotTrue : JSqlKnownFunction.IsTrue;
            else
                methodName = expression.isNot() ? JSqlKnownFunction.IsNotFalse : JSqlKnownFunction.IsFalse;

            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunctionAccess(methodName));
                n.Parameters.Add(Visit(expression.getLeftExpression()));
            });
        }

        // <L.Expr> IS NOT NULL
        // -> IS_NOT_NULL(<L.Expr>)
        public virtual QsiExpressionNode VisitIsNullExpression(IsNullExpression expression)
        {
            var methodName = expression.isNot() ? JSqlKnownFunction.IsNotNull : JSqlKnownFunction.IsNull;

            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunctionAccess(methodName));
                n.Parameters.Add(Visit(expression.getLeftExpression()));
            });
        }

        public virtual QsiExpressionNode VisitColumn(Column expression)
        {
            return TreeHelper.Create<QsiColumnAccessExpressionNode>(n =>
            {
                n.Identifier = IdentifierVisitor.VisitMultiPartName(expression);
            });
        }

        public virtual QsiExpressionNode VisitSubSelect(SubSelect expression)
        {
            return TreeHelper.Create<QsiTableExpressionNode>(n =>
            {
                n.Table.SetValue(TableVisitor.VisitSubSelect(expression));
            });
        }

        public virtual QsiExpressionNode VisitMultipleExpression(MultipleExpression expression)
        {
            return TreeHelper.Create<QsiMultipleExpressionNode>(n =>
            {
                n.Elements.AddRange(expression.getList()
                    .AsEnumerable<Expression>()
                    .Select(Visit));
            });
        }

        public virtual QsiExpressionNode VisitItemsList(ItemsList itemsList)
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

        public virtual QsiExpressionNode VisitExpressionList(ExpressionList expression)
        {
            return TreeHelper.Create<QsiMultipleExpressionNode>(n =>
            {
                n.Elements.AddRange(expression.getExpressions()
                    .AsEnumerable<Expression>()
                    .Select(Visit));
            });
        }

        public virtual QsiExpressionNode VisitMultipleExpressionList(MultiExpressionList expression)
        {
            return TreeHelper.Create<QsiMultipleExpressionNode>(n =>
            {
                n.Elements.AddRange(expression.getExpressionLists()
                    .AsEnumerable<ExpressionList>()
                    .Select(VisitExpressionList));
            });
        }

        // ('xyzzy' from 2 for 3)
        public virtual QsiExpressionNode VisitNamedExpressionList(NamedExpressionList expression)
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
