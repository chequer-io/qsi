namespace Qsi.SqlServer.Common
{
    /// <summary>Specifies the database compatibility level.</summary>
    public enum TransactSqlVersion
    {
        /// <summary>Compatible with SQL Server 2000</summary>
        Version80,

        /// <summary>Compatible with SQL Server 2005</summary>
        Version90,

        /// <summary>Compatible with SQL Server 2008</summary>
        Version100,

        /// <summary>Compatible with SQL Server 2012.</summary>
        Version110,

        /// <summary>Compatible with SQL Server 2014.</summary>
        Version120,

        /// <summary>Compatible with SQL Server 2015.</summary>
        Version130,

        /// <summary>Compatible with SQL Server 2017.</summary>
        Version140,

        /// <summary>Compatible with SQL Server 2019.</summary>
        Version150
    }
}
