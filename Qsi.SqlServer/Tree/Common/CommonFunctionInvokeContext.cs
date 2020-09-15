using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Qsi.SqlServer.Tree.Common
{
    internal sealed class CommonFunctionInvokeContext
    {
        public string FunctionName { get; }

        public IEnumerable<TSqlFragment> Parameters { get; }

        public DataTypeReference DataTypeReference { get; }

        public CommonFunctionInvokeContext(string functionName, params TSqlFragment[] parameters)
        {
            FunctionName = functionName;
            Parameters = parameters;
        }

        public CommonFunctionInvokeContext(string functionName, IEnumerable<TSqlFragment> parameters)
        {
            FunctionName = functionName;
            Parameters = parameters.ToArray();
        }

        public CommonFunctionInvokeContext(string functionName, DataTypeReference dataTypeReference, params TSqlFragment[] parameters)
        {
            FunctionName = functionName;
            DataTypeReference = dataTypeReference;
            Parameters = parameters;
        }
    }
}
