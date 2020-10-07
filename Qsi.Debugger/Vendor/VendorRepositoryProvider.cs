using System.Linq;
using Qsi.Data;
using Qsi.Services;
using Qsi.Utilities;

namespace Qsi.Debugger.Vendor
{
    internal abstract class VendorRepositoryProvider : QsiRepositoryProviderBase
    {
        protected QsiQualifiedIdentifier CreateIdentifier(params string[] path)
        {
            return new QsiQualifiedIdentifier(path.Select(p => new QsiIdentifier(p, IdentifierUtility.IsEscaped(p))));
        }

        protected virtual QsiTableStructure CreateTable(params string[] path)
        {
            return new QsiTableStructure
            {
                Type = QsiTableType.Table,
                Identifier = CreateIdentifier(path)
            };
        }

        protected void AddColumns(QsiTableStructure table, params string[] names)
        {
            foreach (var name in names)
            {
                var c = table.NewColumn();
                c.Name = new QsiIdentifier(name, IdentifierUtility.IsEscaped(name));
            }
        }
    }
}
