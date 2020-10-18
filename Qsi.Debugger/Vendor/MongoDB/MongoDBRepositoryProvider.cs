using Qsi.MongoDB;
using Qsi.MongoDB.Lookup;
using Qsi.MongoDB.Lookup.Data;

namespace Qsi.Debugger.Vendor.MongoDB
{
    internal class MongoDBRepositoryProvider : MongoDBRepositoryProviderBase
    {
        public override MongoDBLookupResult LookupObject(string objectName)
        {
            switch (objectName)
            {
                case "db":
                    return new MongoDBLookupResult
                    {
                        Type = MongoDBLookupType.Database,
                        Value = new DatabaseLookupData
                        {
                            DatabaseName = "test"
                        },
                        ObjectName = "db"
                    };

                case "db.inventory":
                    return new MongoDBLookupResult
                    {
                        Type = MongoDBLookupType.Collection,
                        Value = new CollectionLookupData
                        {
                            DatabaseName = "db",
                            CollectionName = "inventory",
                            FullName = "db.inventory"
                        },
                        ObjectName = "db.inventory"
                    };
            }

            return null;
        }
    }
}
