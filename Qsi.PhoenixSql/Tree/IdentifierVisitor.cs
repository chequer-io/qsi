using System.Text.RegularExpressions;
using PhoenixSql;
using PhoenixSql.Utilities;
using Qsi.Data;

namespace Qsi.PhoenixSql.Tree
{
    internal static class IdentifierVisitor
    {
        private static readonly Regex _identifierPattern = new Regex(@"^[A-Z_][A-Z\d_]*$");
        
        public static QsiIdentifier Visit(DerivedTableNode node)
        {
            return CreateIdentifier(node.Alias);
        }
        
        public static QsiIdentifier Visit(AliasedNode node)
        {
            return CreateIdentifier(node.Alias, node.IsCaseSensitve);
        }

        public static QsiIdentifier Visit(FamilyWildcardParseNode node)
        {
            return CreateIdentifier(node.Name, node.IsCaseSensitive);
        }

        public static QsiIdentifier Visit(TableWildcardParseNode node)
        {
            return CreateIdentifier(node.Name, node.IsCaseSensitive);
        }

        public static QsiIdentifier Visit(NamedTableNode node)
        {
            return CreateIdentifier(node.Alias);
        }

        public static QsiQualifiedIdentifier Visit(TableName node)
        {
            var hasSchemaName = !string.IsNullOrEmpty(node.SchemaName);
            var identifiers = new QsiIdentifier[hasSchemaName ? 2 : 1];

            if (hasSchemaName)
                identifiers[0] = CreateIdentifier(node.SchemaName, node.IsSchemaNameCaseSensitive);

            identifiers[^1] = CreateIdentifier(node.TableName_, node.IsTableNameCaseSensitive);

            return new QsiQualifiedIdentifier(identifiers);
        }

        public static QsiQualifiedIdentifier Visit(ColumnParseNode node)
        {
            var hasTableName = !string.IsNullOrEmpty(node.TableName);
            var identifiers = new QsiIdentifier[hasTableName ? 2 : 1];

            if (hasTableName)
                identifiers[0] = CreateIdentifier(node.TableName, node.IsTableNameCaseSensitive);

            identifiers[^1] = CreateIdentifier(node.Name, node.IsCaseSensitive);

            return new QsiQualifiedIdentifier(identifiers);
        }

        private static QsiIdentifier CreateIdentifier(string value)
        {
            return CreateIdentifier(value, !_identifierPattern.IsMatch(value));
        }
        
        private static QsiIdentifier CreateIdentifier(string value, bool caseSensitive)
        {
            return new QsiIdentifier(caseSensitive ? $"\"{value}\"" : value, caseSensitive);
        }
    }
}
