/* Generated by QSI

 Date: 2020-08-12
 Span: 23:1 - 44:10
 File: src/postgres/include/utils/reltrigger.h

*/

namespace Qsi.PostgreSql.Internal.PG10.Types
{
    internal class Trigger
    {
        public uint? tgoid { get; set; }

        public string tgname { get; set; }

        public uint? tgfoid { get; set; }

        public short? tgtype { get; set; }

        public char? tgenabled { get; set; }

        public bool? tgisinternal { get; set; }

        public uint? tgconstrrelid { get; set; }

        public uint? tgconstrindid { get; set; }

        public uint? tgconstraint { get; set; }

        public bool? tgdeferrable { get; set; }

        public bool? tginitdeferred { get; set; }

        public short? tgnargs { get; set; }

        public short? tgnattr { get; set; }

        public short[] tgattr { get; set; }

        public string[] tgargs { get; set; }

        public string tgqual { get; set; }

        public string tgoldtable { get; set; }

        public string tgnewtable { get; set; }
    }
}