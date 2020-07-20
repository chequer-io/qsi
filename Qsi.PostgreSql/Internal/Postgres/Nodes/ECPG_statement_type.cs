// Generate from postgres/src/interfaces/ecpg/include/ecpgtype.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal enum ECPG_statement_type
    {
        ECPGst_normal,
        ECPGst_execute,
        ECPGst_exec_immediate,
        ECPGst_prepnormal,
        ECPGst_prepare,
        ECPGst_exec_with_exprlist
    }
}
