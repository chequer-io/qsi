/* Generated by QSI

 Date: 2020-08-12
 Span: 1303:1 - 1309:16
 File: src/postgres/include/nodes/primnodes.h

*/

using Qsi.PostgreSql.Internal.Serialization;

namespace Qsi.PostgreSql.Internal.PG10.Types
{
    [PgNode("InferenceElem")]
    internal class InferenceElem : IPg10Node
    {
        public virtual NodeTag Type => NodeTag.T_InferenceElem;

        public IPg10Node xpr { get; set; }

        public IPg10Node[] expr { get; set; }

        public uint? infercollid { get; set; }

        public uint? inferopclass { get; set; }
    }
}