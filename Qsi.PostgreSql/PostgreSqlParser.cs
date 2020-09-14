using System;
using Qsi.Data;
using Qsi.Parsing;
using Qsi.PostgreSql.Internal;
using Qsi.PostgreSql.Internal.PG10;
using Qsi.PostgreSql.Internal.PG10.Types;
using Qsi.PostgreSql.Tree.PG10;
using Qsi.Tree;

namespace Qsi.PostgreSql
{
    public sealed class PostgreSqlParser : IQsiTreeParser, IDisposable
    {
        private IPgParser _pgParser;

        public IQsiTreeNode Parse(QsiScript script)
        {
            _pgParser ??= new PgQuery10();

            var pgTree = _pgParser.Parse(script.Script) ?? throw new QsiException(QsiError.NotSupportedScript);

            return TableVisitor.Visit((IPg10Node)pgTree);
        }

        void IDisposable.Dispose()
        {
            _pgParser?.Dispose();
        }
    }
}
