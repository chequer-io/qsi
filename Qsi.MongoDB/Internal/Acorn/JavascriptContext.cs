using System;
using JavaScriptEngineSwitcher.ChakraCore;

namespace Qsi.MongoDB.Internal.Acorn
{
    internal class JavascriptContext : IDisposable
    {
        private ChakraCoreJsEngine _jsEngine;
        
        public void InitializeEngine()
        {
            _jsEngine = new ChakraCoreJsEngine();
        }

        public string Evaluate(string expression)
        {
            return _jsEngine.Evaluate<string>(expression);
        }

        public void Execute(string expression)
        {
            _jsEngine.Execute(expression);
        }
        
        void IDisposable.Dispose()
        {
            _jsEngine?.Dispose();
            _jsEngine = null;
        }
    }
}
