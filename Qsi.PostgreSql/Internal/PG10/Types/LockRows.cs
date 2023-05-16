/* Generated by QSI

 Date: 2020-08-12
 Span: 907:1 - 912:11
 File: src/postgres/include/nodes/plannodes.h

*/

using Qsi.PostgreSql.Internal.Serialization;

namespace Qsi.PostgreSql.Internal.PG10.Types
{
    [PgNode("LockRows")]
    internal class LockRows : IPg10Node
    {
        public virtual NodeTag Type => NodeTag.T_LockRows;

        public Plan plan { get; set; }

        public IPg10Node[] rowMarks { get; set; }

        public int? epqParam { get; set; }
    }
}