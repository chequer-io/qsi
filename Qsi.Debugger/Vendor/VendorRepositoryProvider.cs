using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Qsi.Data;
using Qsi.Engines;
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
        public event EventHandler<DataTableRequestEventArgs> DataReaderRequested;

        protected sealed override Task<QsiDataTable> GetDataTable(QsiScript script, QsiParameter[] parameters, ExecuteOptions executeOptions, CancellationToken cancellationToken)
        {
            DataReaderRequested?.Invoke(this, new DataTableRequestEventArgs(script, parameters));
            throw new NotImplementedException();
        }

        protected override Task<IDataReader> GetDataReaderAsync(QsiScript script, QsiParameter[] parameters, ExecuteOptions executeOptions, CancellationToken cancellationToken)
        {
            DataReaderRequested?.Invoke(this, new DataTableRequestEventArgs(script, parameters));
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

        protected void AddInvisibleColumns(QsiTableStructure table, params string[] names)
        {
            foreach (var name in names)
            {
                var c = table.NewColumn();
                c.IsVisible = false;
                c.Name = new QsiIdentifier(name, IdentifierUtility.IsEscaped(name));
            }
        }
    }
}
