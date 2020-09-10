using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.SqlServer.Management.SqlParser.Common;
using Microsoft.SqlServer.Management.SqlParser.Parser;
using Microsoft.SqlServer.Management.SqlParser.SqlCodeDom;
using Qsi.Diagnostics;

namespace Qsi.SqlServer.Diagnostics
{
    internal sealed class SqlServerRawTree : IRawTree
    {
        public string DisplayName { get; }

        public IRawTree[] Children { get; }

        internal SqlServerRawTree(SqlCodeObject tree, DatabaseCompatibilityLevel compatibilityLevel)
        {
            if (tree is SqlNullScalarExpression nullScalarExpression)
            {
                var sql = nullScalarExpression.Sql;

                if (Regex.IsMatch(sql, @"^coalesce[^a-z]", RegexOptions.IgnoreCase))
                {
                    var replaceSql = $"SELECT _{sql}";

                    var result = Parser.Parse(replaceSql, new ParseOptions
                    {
                        CompatibilityLevel = compatibilityLevel
                    });

                    if (result.Script.Batches.FirstOrDefault()?.Statements.FirstOrDefault() is SqlSelectStatement selectStatement && 
                        selectStatement.SelectSpecification.QueryExpression is SqlQuerySpecification querySpecification &&
                        querySpecification.SelectClause.SelectExpressions.FirstOrDefault() is SqlSelectScalarExpression scalarExpression)
                    {
                        tree = scalarExpression.Expression;
                    }
                }
            }

            DisplayName = tree.GetType().Name;
            SqlCodeObject[] childrens = tree.Children.ToArray();
            int count = childrens.Length;

            if (childrens.Length == 0)
            {
                Children = new IRawTree[] { new SqlServerRawTreeTerminalNode(tree.Sql) };
            }
            else
            {
                var trees = new IRawTree[count];

                for (int i = 0; i < count; i++)
                {
                    var child = childrens[i];
                    trees[i] = new SqlServerRawTree(child, compatibilityLevel);
                }

                Children = trees;
            }
        }

        internal SqlServerRawTree(string displayName)
        {
            DisplayName = displayName;
        }
    }
}
