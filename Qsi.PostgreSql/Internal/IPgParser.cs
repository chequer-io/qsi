using System;

namespace Qsi.PostgreSql.Internal
{
    internal interface IPgParser : IDisposable
    {
        IPgTree Parse(string input);
    }
}
