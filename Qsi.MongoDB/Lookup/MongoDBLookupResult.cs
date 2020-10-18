using Qsi.MongoDB.Lookup.Data;

namespace Qsi.MongoDB.Lookup
{
    public class MongoDBLookupResult
    {
        public string ObjectName { get; set; }
        
        public MongoDBLookupType Type { get; set; }
        
        public IMongoDBLookupData Value { get; set; }
    }
}
