/* Generated by QSI

 Date: 2020-08-12
 Span: 2185:1 - 2192:29
 File: src/postgres/include/nodes/parsenodes.h

*/

using Qsi.PostgreSql.Internal.Serialization;

namespace Qsi.PostgreSql.Internal.PG10.Types
{
    [PgNode("AlterExtensionContentsStmt")]
    internal class AlterExtensionContentsStmt : IPg10Node
    {
        public virtual NodeTag Type => NodeTag.T_AlterExtensionContentsStmt;

        public string extname { get; set; }

        public int? action { get; set; }

        public ObjectType? objtype { get; set; }

        public IPg10Node[] @object { get; set; }
    }
}
