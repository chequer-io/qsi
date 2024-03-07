namespace Qsi.Cql.Schema;

public abstract class CqlNativeType : CqlType
{
}

public sealed class CqlAsciiType : CqlNativeType
{
    public static readonly CqlAsciiType Default;

    static CqlAsciiType()
    {
        Default = new CqlAsciiType();
    }

    private CqlAsciiType()
    {
    }

    public override string ToSql() => "ascii";
}

public sealed class CqlBigIntType : CqlNativeType
{
    public static readonly CqlBigIntType Default;

    static CqlBigIntType()
    {
        Default = new CqlBigIntType();
    }

    private CqlBigIntType()
    {
    }

    public override string ToSql() => "bigint";
}

public sealed class CqlBlobType : CqlNativeType
{
    public static readonly CqlBlobType Default;

    static CqlBlobType()
    {
        Default = new CqlBlobType();
    }

    private CqlBlobType()
    {
    }

    public override string ToSql() => "blob";
}

public sealed class CqlBooleanType : CqlNativeType
{
    public static readonly CqlBooleanType Default;

    static CqlBooleanType()
    {
        Default = new CqlBooleanType();
    }

    private CqlBooleanType()
    {
    }

    public override string ToSql() => "boolean";
}

public sealed class CqlCounterType : CqlNativeType
{
    public static readonly CqlCounterType Default;

    static CqlCounterType()
    {
        Default = new CqlCounterType();
    }

    private CqlCounterType()
    {
    }

    public override string ToSql() => "counter";
}

public sealed class CqlDecimalType : CqlNativeType
{
    public static readonly CqlDecimalType Default;

    static CqlDecimalType()
    {
        Default = new CqlDecimalType();
    }

    private CqlDecimalType()
    {
    }

    public override string ToSql() => "decimal";
}

public sealed class CqlDoubleType : CqlNativeType
{
    public static readonly CqlDoubleType Default;

    static CqlDoubleType()
    {
        Default = new CqlDoubleType();
    }

    private CqlDoubleType()
    {
    }

    public override string ToSql() => "double";
}

public sealed class CqlDurationType : CqlNativeType
{
    public static readonly CqlDurationType Default;

    static CqlDurationType()
    {
        Default = new CqlDurationType();
    }

    private CqlDurationType()
    {
    }

    public override string ToSql() => "duration";
}

public sealed class CqlFloatType : CqlNativeType
{
    public static readonly CqlFloatType Default;

    static CqlFloatType()
    {
        Default = new CqlFloatType();
    }

    private CqlFloatType()
    {
    }

    public override string ToSql() => "float";
}

public sealed class CqlInetType : CqlNativeType
{
    public static readonly CqlInetType Default;

    static CqlInetType()
    {
        Default = new CqlInetType();
    }

    private CqlInetType()
    {
    }

    public override string ToSql() => "inet";
}

public sealed class CqlIntType : CqlNativeType
{
    public static readonly CqlIntType Default;

    static CqlIntType()
    {
        Default = new CqlIntType();
    }

    private CqlIntType()
    {
    }

    public override string ToSql() => "int";
}

public sealed class CqlSmallIntType : CqlNativeType
{
    public static readonly CqlSmallIntType Default;

    static CqlSmallIntType()
    {
        Default = new CqlSmallIntType();
    }

    private CqlSmallIntType()
    {
    }

    public override string ToSql() => "smallint";
}

public sealed class CqlTextType : CqlNativeType
{
    public static readonly CqlTextType Default;

    static CqlTextType()
    {
        Default = new CqlTextType();
    }

    private CqlTextType()
    {
    }

    public override string ToSql() => "text";
}

public sealed class CqlTimestampType : CqlNativeType
{
    public static readonly CqlTimestampType Default;

    static CqlTimestampType()
    {
        Default = new CqlTimestampType();
    }

    private CqlTimestampType()
    {
    }

    public override string ToSql() => "timestamp";
}

public sealed class CqlTinyintType : CqlNativeType
{
    public static readonly CqlTinyintType Default;

    static CqlTinyintType()
    {
        Default = new CqlTinyintType();
    }

    private CqlTinyintType()
    {
    }

    public override string ToSql() => "tinyint";
}

public sealed class CqlUuidType : CqlNativeType
{
    public static readonly CqlUuidType Default;

    static CqlUuidType()
    {
        Default = new CqlUuidType();
    }

    private CqlUuidType()
    {
    }

    public override string ToSql() => "uuid";
}

public sealed class CqlVarcharType : CqlNativeType
{
    public static readonly CqlVarcharType Default;

    static CqlVarcharType()
    {
        Default = new CqlVarcharType();
    }

    private CqlVarcharType()
    {
    }

    public override string ToSql() => "varchar";
}

public sealed class CqlVarintType : CqlNativeType
{
    public static readonly CqlVarintType Default;

    static CqlVarintType()
    {
        Default = new CqlVarintType();
    }

    private CqlVarintType()
    {
    }

    public override string ToSql() => "varint";
}

public sealed class CqlTimeUuidType : CqlNativeType
{
    public static readonly CqlTimeUuidType Default;

    static CqlTimeUuidType()
    {
        Default = new CqlTimeUuidType();
    }

    private CqlTimeUuidType()
    {
    }

    public override string ToSql() => "timeuuid";
}

public sealed class CqlDateType : CqlNativeType
{
    public static readonly CqlDateType Default;

    static CqlDateType()
    {
        Default = new CqlDateType();
    }

    private CqlDateType()
    {
    }

    public override string ToSql() => "date";
}

public sealed class CqlTimeType : CqlNativeType
{
    public static readonly CqlTimeType Default;

    static CqlTimeType()
    {
        Default = new CqlTimeType();
    }

    private CqlTimeType()
    {
    }

    public override string ToSql() => "time";
}