using System;
using JavaScriptEngineSwitcher.ChakraCore;

namespace Qsi.PostgreSql.Internal
{
    internal abstract class PgQueryBase<T> : IPgParser where T : IPgNode
    {
        private ChakraCoreJsEngine _jsEngine;
        private bool _initialized;

        private void Initialize()
        {
            if (_initialized)
                return;

            try
            {
                _jsEngine = new ChakraCoreJsEngine();
                OnInitialize();
            }
            catch
            {
                _jsEngine?.Dispose();
                _jsEngine = null;
                throw;
            }

            _initialized = true;
        }

        protected virtual void OnInitialize()
        {
        }

        protected abstract T Parse(string input);

        protected string Evaluate(string expression)
        {
            return _jsEngine.Evaluate<string>(expression);
        }

        protected void Execute(string code)
        {
            _jsEngine.Execute(code);
        }

        IPgNode IPgParser.Parse(string input)
        {
            Initialize();
            return Parse(input);
        }

        void IDisposable.Dispose()
        {
            _jsEngine?.Dispose();
            _jsEngine = null;
        }
    }
}
