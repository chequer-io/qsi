/* Generated by QSI

 Date: 2020-08-12
 Span: 2000:1 - 2015:13
 File: src/postgres/include/nodes/parsenodes.h

*/

using Qsi.PostgreSql.Internal.Serialization;

namespace Qsi.PostgreSql.Internal.PG10.Types
{
    [PgNode("CreateStmt")]
    internal class CreateStmt : IPg10Node
    {
        public virtual NodeTag Type => NodeTag.T_CreateStmt;

        public RangeVar[] relation { get; set; }

        public IPg10Node[] tableElts { get; set; }

        public IPg10Node[] inhRelations { get; set; }

        public PartitionBoundSpec[] partbound { get; set; }

        public PartitionSpec[] partspec { get; set; }

        public TypeName[] ofTypename { get; set; }

        public IPg10Node[] constraints { get; set; }

        public IPg10Node[] options { get; set; }

        public OnCommitAction? oncommit { get; set; }

        public string tablespacename { get; set; }

        public bool? if_not_exists { get; set; }
    }
}
