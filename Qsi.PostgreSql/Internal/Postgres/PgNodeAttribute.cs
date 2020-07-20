using System;

namespace Qsi.PostgreSql.Internal.Postgres
{
    [AttributeUsage(AttributeTargets.Class)]
    internal sealed class PgNodeAttribute : Attribute
    {
        public string Name { get; }
        
        public PgNodeAttribute(string name)
        {
            Name = name;
        }
    }
}
