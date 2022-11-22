using System;

namespace Qsi.PostgreSql.Internal.Serialization
{
    [AttributeUsage(AttributeTargets.Class)]
    internal sealed class PgNodeAttribute : Attribute
    {
        public string Name { get; }

        public bool Union { get; set; }

        public PgNodeAttribute(string name)
        {
            Name = name;
        }
    }
}
