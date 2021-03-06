/* Generated by QSI

 Date: 2020-08-12
 Span: 3417:1 - 3425:24
 File: src/postgres/include/nodes/parsenodes.h

*/

using Qsi.PostgreSql.Internal.Serialization;

namespace Qsi.PostgreSql.Internal.PG10.Types
{
    [PgNode("AlterSubscriptionStmt")]
    internal class AlterSubscriptionStmt : IPg10Node
    {
        public virtual NodeTag Type => NodeTag.T_AlterSubscriptionStmt;

        public AlterSubscriptionType? kind { get; set; }

        public string subname { get; set; }

        public string conninfo { get; set; }

        public IPg10Node[] publication { get; set; }

        public IPg10Node[] options { get; set; }
    }
}
