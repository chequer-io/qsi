namespace PrimarSql.Data.Models.Columns
{
    internal sealed class IdentifierPart : IPart
    {
        public string Identifier { get; }
        
        public IdentifierPart(string identifier)
        {
            Identifier = identifier;
        }

        public override string ToString()
        {
            return Identifier;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is IdentifierPart identifierPart))
                return false;
            
            return Identifier == identifierPart.Identifier;
        }

        public override int GetHashCode()
        {
            return Identifier.GetHashCode();
        }
    }
}
