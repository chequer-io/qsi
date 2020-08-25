using System;

namespace Qsi.PostgreSql.Internal
{
    internal interface IPgParser : IDisposable
    {
        IPgNode Parse(string input);
    }
}
