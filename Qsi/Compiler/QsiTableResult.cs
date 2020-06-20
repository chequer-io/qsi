using System;
using Qsi.Data;

namespace Qsi.Compiler
{
    public sealed class QsiTableResult
    {
        public QsiDataTable Table { get; }

        public Exception[] Exceptions { get; }

        public QsiTableResult(QsiDataTable table, Exception[] exceptions)
        {
            Table = table;
            Exceptions = exceptions;
        }
    }
}
