/* Generated by QSI

 Date: 2020-08-12
 Span: 1200:1 - 1206:14
 File: src/postgres/include/nodes/primnodes.h

*/

using Qsi.PostgreSql.Internal.Serialization;

namespace Qsi.PostgreSql.Internal.PG10.Types
{
    [PgNode("BooleanTest")]
    internal class BooleanTest : IPg10Node
    {
        public virtual NodeTag Type => NodeTag.T_BooleanTest;

        public IPg10Node xpr { get; set; }

        public IPg10Node[] arg { get; set; }

        public BoolTestType? booltesttype { get; set; }
    }
}