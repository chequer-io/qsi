using System.Collections.Generic;

namespace Qsi.Tests.Vendor.PostgreSql.Utilities;

internal interface IPostgreSqlTestQueryImporter
{
    string[] Import();
}