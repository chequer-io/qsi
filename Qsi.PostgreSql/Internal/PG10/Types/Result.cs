/* Generated by QSI

 Date: 2020-08-12
 Span: 188:1 - 192:9
 File: src/postgres/include/nodes/plannodes.h

*/

using Qsi.PostgreSql.Internal.Serialization;

namespace Qsi.PostgreSql.Internal.PG10.Types
{
    [PgNode("Result")]
    internal class Result : IPg10Node
    {
        public virtual NodeTag Type => NodeTag.T_Result;

        public Plan plan { get; set; }

        public IPg10Node[] resconstantqual { get; set; }
    }
}