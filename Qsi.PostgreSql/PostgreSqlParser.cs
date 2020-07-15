using System;
using Qsi.Data;
using Qsi.Parsing;
using Qsi.PostgreSql.Internal;
using Qsi.Tree;

namespace Qsi.PostgreSql
{
    public class PostgreSqlParser : IQsiTreeParser, IDisposable
    {
        public event EventHandler<QsiSyntaxErrorException> SyntaxError;

        private IPgParser _pgParser;

        public IQsiTreeNode Parse(QsiScript script)
        {
            try
            {
                _pgParser ??= new PgQuery10();
                //return _pgQuery.Parse(script.Script);
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
