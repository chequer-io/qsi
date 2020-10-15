using Qsi.MongoDB;
using Qsi.Services;

namespace Qsi.Debugger.Vendor.MongoDB
{
    public class MongoDBLanguageService : MongoDBLanguageServiceBase
    {
        public override IQsiRepositoryProvider CreateRepositoryProvider()
        {
            return new MongoDBRepositoryProvider();
        }
    }
}
