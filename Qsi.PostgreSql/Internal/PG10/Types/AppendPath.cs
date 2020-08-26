/* Generated by QSI

 Date: 2020-08-12
 Span: 1175:1 - 1181:13
 File: src/postgres/include/nodes/relation.h

*/

using Qsi.PostgreSql.Internal.Serialization;

namespace Qsi.PostgreSql.Internal.PG10.Types
{
    [PgNode("AppendPath")]
    internal class AppendPath : IPg10Node
    {
        public virtual NodeTag Type => NodeTag.T_AppendPath;

        public Path path { get; set; }

        public IPg10Node[] partitioned_rels { get; set; }

        public IPg10Node[] subpaths { get; set; }
    }
}