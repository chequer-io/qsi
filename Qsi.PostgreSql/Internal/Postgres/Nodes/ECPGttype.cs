// Generate from postgres/src/interfaces/ecpg/include/ecpgtype.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal enum ECPGttype
    {
        ECPGt_char = 1,
        ECPGt_unsigned_char,
        ECPGt_short,
        ECPGt_unsigned_short,
        ECPGt_int,
        ECPGt_unsigned_int,
        ECPGt_long,
        ECPGt_unsigned_long,
        ECPGt_long_long,
        ECPGt_unsigned_long_long,
        ECPGt_bool,
        ECPGt_float,
        ECPGt_double,
        ECPGt_varchar,
        ECPGt_varchar2,
        ECPGt_numeric,
        ECPGt_decimal,
        ECPGt_date,
        ECPGt_timestamp,
        ECPGt_interval,
        ECPGt_array,
        ECPGt_struct,
        ECPGt_union,
        ECPGt_descriptor,
        ECPGt_char_variable,
        ECPGt_const,
        ECPGt_EOIT,
        ECPGt_EORT,
        ECPGt_NO_INDICATOR,
        ECPGt_string,
        ECPGt_sqlda,
        ECPGt_bytea
    }
}
