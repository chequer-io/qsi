/* Generated by QSI

 Date: 2020-08-12
 Span: 568:1 - 578:17
 File: src/postgres/include/nodes/parsenodes.h

*/

using Qsi.PostgreSql.Internal.Serialization;

namespace Qsi.PostgreSql.Internal.PG10.Types
{
    [PgNode("RangeTableFunc")]
    internal class RangeTableFunc : IPg10Node
    {
        public virtual NodeTag Type => NodeTag.T_RangeTableFunc;

        public bool? lateral { get; set; }

        public IPg10Node[] docexpr { get; set; }

        public IPg10Node[] rowexpr { get; set; }

        public IPg10Node[] namespaces { get; set; }

        public IPg10Node[] columns { get; set; }

        public Alias[] alias { get; set; }
    }
}