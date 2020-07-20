namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("String")]
    internal class PgString
    {
        public string str { get; set; }
    }
    
    [PgNode("Integer")]
    internal class PgInteger
    {
        public int ival { get; set; }
    }
    
    [PgNode("Float")]
    internal class PgFloat : PgString
    {
    }
}
