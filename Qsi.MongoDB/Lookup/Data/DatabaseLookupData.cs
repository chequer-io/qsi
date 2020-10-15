namespace Qsi.MongoDB.Lookup.Data
{
    public class DatabaseLookupData : IMongoDBLookupData
    {
        public string DatabaseName { get; set; }
        
        public string CollectionName { get; set; }
        
        public string FullName { get; set; }
    }
}
