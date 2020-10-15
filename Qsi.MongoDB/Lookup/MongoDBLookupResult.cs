using Qsi.MongoDB.Lookup.Data;

namespace Qsi.MongoDB.Lookup
{
    public class MongoDBLookupResult
    {
        public MongoDBLookupType Type { get; set; }
        
        public IMongoDBLookupData Value { get; set; }
    }
}
