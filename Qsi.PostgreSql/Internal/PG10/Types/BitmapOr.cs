/* Generated by QSI

 Date: 2020-08-12
 Span: 314:1 - 319:11
 File: src/postgres/include/nodes/plannodes.h

*/

using Qsi.PostgreSql.Internal.Serialization;

namespace Qsi.PostgreSql.Internal.PG10.Types
{
    [PgNode("BitmapOr")]
    internal class BitmapOr : IPg10Node
    {
        public virtual NodeTag Type => NodeTag.T_BitmapOr;

        public Plan plan { get; set; }

        public bool? isshared { get; set; }

        public IPg10Node[] bitmapplans { get; set; }
    }
}
