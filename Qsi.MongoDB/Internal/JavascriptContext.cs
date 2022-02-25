using System;
using JavaScriptEngineSwitcher.ChakraCore;

namespace Qsi.MongoDB.Internal;

internal class JavascriptContext : IDisposable
{
    private ChakraCoreJsEngine _jsEngine;

    void IDisposable.Dispose()
    {
        _jsEngine?.Dispose();
        _jsEngine = null;
    }

    public void InitializeEngine()
    {
        _jsEngine = new ChakraCoreJsEngine();
    }

    public string Evaluate(string expression)
    {
        return _jsEngine.Evaluate<string>(expression);
    }

    public void SetVariable(string name, object value)
    {
        _jsEngine.SetVariableValue(name, value);
    }

    public void Execute(string expression)
    {
        _jsEngine.Execute(expression);
    }
}
