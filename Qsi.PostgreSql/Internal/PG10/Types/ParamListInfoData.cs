/* Generated by QSI

 Date: 2020-08-12
 Span: 68:1 - 77:22
 File: src/postgres/include/nodes/params.h

*/

namespace Qsi.PostgreSql.Internal.PG10.Types
{
    internal class ParamListInfoData
    {
        public string[] paramFetch { get; set; }

        public object[] paramFetchArg { get; set; }

        public string[] parserSetup { get; set; }

        public object[] parserSetupArg { get; set; }

        public int? numParams { get; set; }

        public Bitmapset[] paramMask { get; set; }

        public ParamExternData[] @params { get; set; }
    }
}
