using System.Collections.Generic;
using Qsi.MongoDB.Analyzers;

namespace Qsi.MongoDB
{
    public sealed class MongoDBVariableStack
    {
        private readonly Dictionary<string, Stack<JsObjectInfo>> _variableStack;

        public MongoDBVariableStack()
        {
            _variableStack = new Dictionary<string, Stack<JsObjectInfo>>();
        }

        public void SetValue(string variableName, JsObjectInfo value)
        {
            if (_variableStack.ContainsKey(variableName))
            {
                _variableStack[variableName].Push(value);
            }
            else
            {
                var stack = new Stack<JsObjectInfo>();
                stack.Push(value);

                _variableStack[variableName] = stack;
            }
        }

        public bool ClearVariable(string variableName)
        {
            return _variableStack.Remove(variableName);
        }

        public bool ExistsVariable(string variableName)
        {
            return _variableStack.ContainsKey(variableName);
        }
        
        public JsObjectInfo GetValue(string variableName)
        {
            if (!_variableStack.ContainsKey(variableName))
                return null;

            return _variableStack[variableName].Peek();
        }
    }
}
