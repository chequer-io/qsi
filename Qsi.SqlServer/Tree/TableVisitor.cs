using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.Management.SqlParser.SqlCodeDom;
using Qsi.Data;
using Qsi.Tree.Base;
using Qsi.Utilities;

namespace Qsi.SqlServer.Tree
{
    internal static class TableVisitor
    {
        #region Tree
        public static QsiTableNode Visit(SqlCodeObject codeObject)
        {
            if (codeObject == null)
                return null;

            switch (codeObject)
            {
                case SqlSelectStatement selectStatement:
                    return VisitSelectStatement(selectStatement);
            }
            
            return null;
        }
        #endregion
        
        #region Select Statements
        public static QsiTableNode VisitSelectStatement(SqlSelectStatement selectStatement)
        {
            return VisitQueryExpression(selectStatement.SelectSpecification.QueryExpression);
        }

        public static QsiTableNode VisitQueryExpression(SqlQueryExpression queryExpression)
        {
            switch (queryExpression)
            {
                case SqlQuerySpecification querySpecification:
                    return VisitQuerySpecification(querySpecification);
                case SqlBinaryQueryExpression binaryQueryExpression:
                    return VisitBinaryQueryExpression(binaryQueryExpression);
            }

            return null;
        }

        public static QsiDerivedTableNode VisitQuerySpecification(SqlQuerySpecification querySpecification)
        {
            return TreeHelper.Create<QsiDerivedTableNode>(n =>
            {
                foreach (var codeObject in querySpecification.Children)
                {
                    switch (codeObject)
                    {
                        case SqlSelectClause selectClause:
                            n.Columns.SetValue(VisitSelectClause(selectClause));
                            break;
                        case SqlFromClause fromClause:
                            n.Source.SetValue(VisitFromClause(fromClause));
                            break;
                    }
                }
            });
        }

        public static QsiCompositeTableNode VisitBinaryQueryExpression(SqlBinaryQueryExpression binaryQueryExpression)
        {
            return TreeHelper.Create<QsiCompositeTableNode>(n =>
            {
                n.Sources.Add(VisitQueryExpression(binaryQueryExpression.Left));
                n.Sources.Add(VisitQueryExpression(binaryQueryExpression.Right));
            });
        }
        
        public static QsiColumnsDeclarationNode VisitSelectClause(SqlSelectClause selectClause)
        {
            return TreeHelper.Create<QsiColumnsDeclarationNode>(dn =>
            {
                foreach (var selectExpression in selectClause.SelectExpressions)
                {
                    switch (selectExpression)
                    {
                        case SqlSelectStarExpression starExpression:
                            dn.Columns.Add(VisitStarExpression(starExpression));
                            break;
                        
                        case SqlSelectScalarExpression scalarExpression:
                            dn.Columns.Add(VisitSelectScalarExpression(scalarExpression));
                            break;
                    }
                }
            });
        }

        public static QsiTableNode VisitFromClause(SqlFromClause fromClause)
        {
            // TODO: TableExpressions에 대응 (현재는 바로 return)
            foreach (var tableExpression in fromClause.TableExpressions)
            {
                switch (tableExpression)
                {
                    case SqlTableRefExpression tableRefExpression:
                        return VisitTableRefExpression(tableRefExpression);
                }
            }

            return null;
            //
            // return TreeHelper.Create<QsiTableAccessNode>(n =>
            // {
            //     n.Identifier = 
            // });
        }

        public static QsiTableNode VisitTableRefExpression(SqlTableRefExpression tableRefExpression)
        {
            var tableNode = new QsiTableAccessNode
            {
                Identifier = IdentifierVisitor.VisitMultipartIdentifier(tableRefExpression.ObjectIdentifier)
            };

            if (tableRefExpression.Alias == null)
                return tableNode;

            return TreeHelper.Create<QsiDerivedTableNode>(n =>
            {
                var allDeclaration = new QsiColumnsDeclarationNode();
                allDeclaration.Columns.Add(new QsiAllColumnNode());

                n.Columns.SetValue(allDeclaration);
                n.Source.SetValue(tableNode);
                
                n.Alias.SetValue(new QsiAliasNode
                {
                    Name = new QsiIdentifier(tableRefExpression.Alias.Value, false)
                });
            });
        }

        private static QsiColumnNode VisitSelectScalarExpression(SqlSelectScalarExpression scalarExpression)
        {
            return new QsiDeclaredColumnNode
            {
                Name = IdentifierVisitor.VisitScalarExpression(scalarExpression.Expression),
            };
        }

        public static QsiColumnNode VisitStarExpression(SqlSelectStarExpression starExpression)
        {
            return new QsiAllColumnNode();
        }
        #endregion
    }
}
