namespace Qsi.MongoDB.Lookup.Data
{
    public class LiteralLookupData : IMongoDBLookupData
    {
        public string Type { get; set; }
        
        public object Value { get; set; }
    }
}
