/* Generated by QSI

 Date: 2020-08-12
 Span: 1595:1 - 1601:12
 File: src/postgres/include/nodes/relation.h

*/

using Qsi.PostgreSql.Internal.Serialization;

namespace Qsi.PostgreSql.Internal.PG10.Types
{
    [PgNode("LimitPath")]
    internal class LimitPath : IPg10Node
    {
        public virtual NodeTag Type => NodeTag.T_LimitPath;

        public Path path { get; set; }

        public Path[] subpath { get; set; }

        public IPg10Node[] limitOffset { get; set; }

        public IPg10Node[] limitCount { get; set; }
    }
}