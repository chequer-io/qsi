using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace Qsi.Engines;

public class ExecuteOptions
{
    [AllowNull]
    public DbConnection Connection { get; set; }

    [AllowNull]
    public string SchemaName { get; set; }
}
