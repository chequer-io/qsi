/* Generated by QSI

 Date: 2020-08-12
 Span: 21:1 - 28:21
 File: src/postgres/include/nodes/lockoptions.h

*/

namespace Qsi.PostgreSql.Internal.PG10.Types
{
    internal enum LockClauseStrength
    {
        LCS_NONE = 0,
        LCS_FORKEYSHARE = 1,
        LCS_FORSHARE = 2,
        LCS_FORNOKEYUPDATE = 3,
        LCS_FORUPDATE = 4
    }
}