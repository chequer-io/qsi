using System;
using System.Linq;
using System.Threading.Tasks;
using Qsi.Data;
using Qsi.Services;
using Qsi.Utilities;

namespace Qsi.Debugger.Vendor
{
    internal class DataTableRequestEventArgs : EventArgs
    {
        public QsiScript Script { get; }

        public QsiParameter[] Parameters { get; }

        public DataTableRequestEventArgs(QsiScript script, QsiParameter[] parameters)
        {
            Script = script;
            Parameters = parameters;
        }
    }

    internal abstract class VendorRepositoryProvider : QsiRepositoryProviderBase
    {
        public event EventHandler<DataTableRequestEventArgs> DataTableRequested;

        protected sealed override Task<QsiDataTable> GetDataTable(QsiScript script, QsiParameter[] parameters)
        {
            DataTableRequested?.Invoke(this, new DataTableRequestEventArgs(script, parameters));
            throw new NotImplementedException();
        }

        protected QsiQualifiedIdentifier CreateIdentifier(params string[] path)
        {
            return new(path.Select(p => new QsiIdentifier(p, IdentifierUtility.IsEscaped(p))));
        }

        protected virtual QsiTableStructure CreateTable(params string[] path)
        {
            return new()
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
