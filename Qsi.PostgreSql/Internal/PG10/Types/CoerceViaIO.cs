/* Generated by QSI

 Date: 2020-08-12
 Span: 808:1 - 817:14
 File: src/postgres/include/nodes/primnodes.h

*/

using Qsi.PostgreSql.Internal.Serialization;

namespace Qsi.PostgreSql.Internal.PG10.Types
{
    [PgNode("CoerceViaIO")]
    internal class CoerceViaIO : IPg10Node
    {
        public virtual NodeTag Type => NodeTag.T_CoerceViaIO;

        public IPg10Node xpr { get; set; }

        public IPg10Node[] arg { get; set; }

        public uint? resulttype { get; set; }

        public uint? resultcollid { get; set; }

        public CoercionForm? coerceformat { get; set; }
    }
}
