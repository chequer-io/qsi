/* Generated by QSI

 Date: 2020-08-12
 Span: 241:1 - 246:11
 File: src/postgres/include/nodes/parsenodes.h

*/

using Qsi.PostgreSql.Internal.Serialization;

namespace Qsi.PostgreSql.Internal.PG10.Types
{
    [PgNode("ParamRef")]
    internal class ParamRef : IPg10Node
    {
        public virtual NodeTag Type => NodeTag.T_ParamRef;

        public int? number { get; set; }
    }
}
