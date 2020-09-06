using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.Management.SqlParser.SqlCodeDom;
using Qsi.Tree.Base;

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
                // case SqlCreateViewStatement createViewStatement:
                //
                //     break;
            }
            
            return null;
        }
        #endregion
        
        #region Select Statements
        public static QsiTableNode VisitSelectStatement(SqlSelectStatement selectStatement)
        {
            var withClause = selectStatement.QueryWithClause;
            var selectSpecification = selectStatement.SelectSpecification;
            
            QsiTableNode tableNode = null;
            
            return tableNode;
        }

        public static QsiTableNode VisitSelectStatementNormal(SqlSelectStatement selectStatement)
        {
            return null;
        }
        #endregion
    }
}
