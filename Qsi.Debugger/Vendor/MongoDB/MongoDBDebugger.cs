using Qsi.Diagnostics;
using Qsi.MongoDB.Diagnostics;
using Qsi.Services;

namespace Qsi.Debugger.Vendor.MongoDB
{
    internal class MongoDBDebugger : VendorDebugger
    {
        public MongoDBDebugger()
        {
        }

        protected override IQsiLanguageService CreateLanguageService()
        {
            return new MongoDBLanguageService();
        }

        protected override IRawTreeParser CreateRawTreeParser()
        {
            return new MongoDBRawParser();
        }
    }
}
