using System.Linq;
using Qsi.Data;
using Qsi.Services;

namespace Qsi.Debugger.Vendor
{
    internal abstract class VendorReferenceResolver : QsiReferenceResolverBase
    {
        protected QsiQualifiedIdentifier CreateIdentifier(params string[] path)
        {
            return new QsiQualifiedIdentifier(path.Select(p => new QsiIdentifier(p, p[0] == '`')));
        }

        protected virtual QsiDataTable CreateTable(params string[] path)
        {
            return new QsiDataTable
            {
                Type = QsiDataTableType.Table,
                Identifier = CreateIdentifier(path)
            };
        }

        protected void AddColumns(QsiDataTable table, params string[] names)
        {
            foreach (var name in names)
            {
                var c = table.NewColumn();
                c.Name = new QsiIdentifier(name, name[0] == '`');
            }
        }
    }
}
