using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Qsi.SqlServer.Tree.Common
{
    internal sealed class CommonFunctionInvokeContext
    {
        public string FunctionName { get; }
        
        public ScalarExpression[] Parameters { get; }

        public CommonFunctionInvokeContext(FunctionCall functionCall)
        {
            FunctionName = functionCall.FunctionName.Value;
            Parameters = functionCall.Parameters.ToArray();
        }
        
        public CommonFunctionInvokeContext(string functionName, params ScalarExpression[] parameters)
        {
            FunctionName = functionName;
            Parameters = parameters;
        }
    }
}
