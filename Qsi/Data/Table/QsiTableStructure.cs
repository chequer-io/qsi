using System;
using System.Collections.Generic;
using System.Linq;
using Qsi.Shared.Extensions;

namespace Qsi.Data;

public sealed class QsiTableStructure
{
    public QsiTableType Type { get; set; }

    public QsiQualifiedIdentifier Identifier { get; set; }

    public bool HasIdentifier => Identifier != null;

    public bool IsSystem { get; set; }

    public IList<QsiTableStructure> References { get; } = new List<QsiTableStructure>();

    public IList<QsiTableColumn> Columns => _columns;

    /// <summary>
    /// 테이블을 직접적으로 구성하는 컬럼이 아닌 간접적으로 참조하는 컬럼을 의미합니다.
    /// </summary>
    /// <remarks>
    /// <ul>
    /// <li>WHERE문이나 HAVING문의 조건에서 사용하는 컬럼이나, 서브쿼리 내에 들어 있는 컬럼 등을 포함합니다.</li>
    /// <li>주석 작성일 기준으로 해당 필드의 값은 WHERE절 내부에서 사용하는 컬럼들만 포함하고 있습니다.</li>
    /// </ul>
    /// </remarks>
    public QsiTableColumn[] IndirectColumns = Array.Empty<QsiTableColumn>();

    internal IEnumerable<QsiTableColumn> VisibleColumns => _columns.Where(c => c.IsVisible);

    private readonly QsiTableColumnCollection _columns;

    public QsiTableStructure()
    {
        _columns = new QsiTableColumnCollection(this);
    }

    public QsiTableColumn NewColumn()
    {
        var column = new QsiTableColumn();
        _columns.Add(column);

        return column;
    }

    public QsiTableStructure Clone()
    {
        var table = new QsiTableStructure
        {
            Type = Type,
            Identifier = Identifier,
            IsSystem = IsSystem
        };

        table.References.AddRange(References);
        table._columns.AddRange(_columns.Select(c => c.CloneInternal()));
        table.IndirectColumns = (QsiTableColumn[])IndirectColumns.Clone();

        return table;
    }

    // TODO: Refactor to VisibleColumns
    public QsiTableStructure CloneVisibleOnly()
    {
        var table = new QsiTableStructure
        {
            Type = Type,
            Identifier = Identifier,
            IsSystem = IsSystem
        };

        table.References.AddRange(References);
        table._columns.AddRange(VisibleColumns.Select(c => c.CloneInternal()));
        table.IndirectColumns = (QsiTableColumn[])IndirectColumns.Clone();

        return table;
    }
}
