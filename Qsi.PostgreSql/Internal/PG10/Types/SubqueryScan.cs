/* Generated by QSI

 Date: 2020-08-12
 Span: 495:1 - 499:15
 File: src/postgres/include/nodes/plannodes.h

*/

using Qsi.PostgreSql.Internal.Serialization;

namespace Qsi.PostgreSql.Internal.PG10.Types
{
    [PgNode("SubqueryScan")]
    internal class SubqueryScan : IPg10Node
    {
        public virtual NodeTag Type => NodeTag.T_SubqueryScan;

        public Scan scan { get; set; }

        public Plan[] subplan { get; set; }
    }
}