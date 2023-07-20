using System.Linq;
using System.Text.RegularExpressions;
using PhoenixSql;
using Qsi.Data;

namespace Qsi.PhoenixSql.Tree;

internal static class IdentifierVisitor
{
    private static readonly Regex _namePattern = new(@"^[A-Z][A-Z\d_\u0080-\u2001\u2003-\ufffe]*$");
        
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
        
    public static QsiQualifiedIdentifier Visit(ColumnName node)
    {
        var hasFamilyName = !string.IsNullOrEmpty(node.FamilyNode?.Name);
        var identifiers = new QsiIdentifier[hasFamilyName ? 2 : 1];

        if (hasFamilyName)
            identifiers[0] = CreateIdentifier(node.FamilyNode.Name, node.FamilyNode.IsCaseSensitive);

        identifiers[^1] = CreateIdentifier(node.ColumnNode.Name, node.ColumnNode.IsCaseSensitive);

        return new QsiQualifiedIdentifier(identifiers);
    }

    private static QsiIdentifier CreateIdentifier(string value)
    {
        var caseSensitive = !_namePattern.IsMatch(value) || value.Any(char.IsLower);
        return CreateIdentifier(value, caseSensitive);
    }
        
    private static QsiIdentifier CreateIdentifier(string value, bool caseSensitive)
    {
        return new(caseSensitive ? $"\"{value}\"" : value, caseSensitive);
    }
}