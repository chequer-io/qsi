/* Generated by QSI

 Date: 2020-08-12
 Span: 723:1 - 727:11
 File: src/postgres/include/nodes/plannodes.h

*/

using Qsi.PostgreSql.Internal.Serialization;

namespace Qsi.PostgreSql.Internal.PG10.Types
{
    [PgNode("HashJoin")]
    internal class HashJoin : IPg10Node
    {
        public virtual NodeTag Type => NodeTag.T_HashJoin;

        public Join join { get; set; }

        public IPg10Node[] hashclauses { get; set; }
    }
}
