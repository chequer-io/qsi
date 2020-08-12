using System;
using System.Linq;
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
        public event EventHandler<QsiSyntaxErrorException> SyntaxError;

        private IPgParser _pgParser;

        public IQsiTreeNode Parse(QsiScript script)
        {
            try
            {
                _pgParser ??= new PgQuery10();

                var pgTree = _pgParser.Parse(script.Script);

                return TableVisitor.Visit((IPg10Node)pgTree).SingleOrDefault();
            }
            catch (QsiSyntaxErrorException e)
            {
                SyntaxError?.Invoke(this, e);
            }

            return null;
        }

        void IDisposable.Dispose()
        {
            _pgParser?.Dispose();
        }
    }
}
