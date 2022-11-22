﻿using System;
using Qsi.Diagnostics;
using Qsi.PostgreSql.Internal;
using Qsi.PostgreSql.Internal.PG10;

namespace Qsi.PostgreSql.Diagnostics
{
    public class PostgreSqlRawParser : IRawTreeParser, IDisposable
    {
        private IPgParser _pgParser;

        public IRawTree Parse(string input)
        {
            _pgParser ??= new PgQuery10();
            var result = _pgParser.Parse(input);

            return new PostgreSqlRawTree(result);
        }

        void IDisposable.Dispose()
        {
            _pgParser?.Dispose();
        }
    }
}
