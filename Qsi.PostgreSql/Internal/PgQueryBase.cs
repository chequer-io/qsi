using System;
using System.Runtime.InteropServices;
using ChakraCore.NET.API;
using Newtonsoft.Json;
using Qsi.Parsing;

namespace Qsi.PostgreSql.Internal
{
    internal abstract class PgQueryBase<T> : IPgParser
    {
        private JavaScriptRuntime _runtime;
        private JavaScriptContext _context;
        private JavaScriptSourceContext _srcContext;

        private bool _initialized;

        private void Initialize()
        {
            if (_initialized)
                return;

            string parserJs = GetParserScript();

            try
            {
                Native.ThrowIfError(Native.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null, out _runtime));
                Native.ThrowIfError(Native.JsCreateContext(_runtime, out _context));
                Native.ThrowIfError(Native.JsSetCurrentContext(_context));

                _srcContext = JavaScriptSourceContext.FromIntPtr(IntPtr.Zero);

                Native.ThrowIfError(Native.JsRunScript(parserJs, _srcContext++, string.Empty, out _));
            }
            catch (Exception)
            {
                _runtime.Dispose();
                throw;
            }

            _initialized = true;
        }

        protected abstract string GetParserScript();

        protected abstract string GetParseScript(string input);

        public IPgTree Parse(string input)
        {
            Initialize();

            var parseJs = GetParseScript(input);

            Native.ThrowIfError(Native.JsRunScript(parseJs, _srcContext++, string.Empty, out var result));
            Native.JsStringToPointer(result, out var resultPtr, out _);

            var json = Marshal.PtrToStringUni(resultPtr);
            var parseResult = JsonConvert.DeserializeObject<PgParseResult>(json!);

            if (parseResult.Error != null)
            {
                // TODO: Measure line, column number by Error.CursorPosition
                throw new QsiSyntaxErrorException(0, 0, parseResult.Error.Message);
            }

            if (!string.IsNullOrEmpty(parseResult.StandardError))
            {
                throw new QsiException(QsiError.Internal, parseResult.StandardError);
            }

            if (parseResult.Tree?.Length != 1)
                throw new InvalidOperationException();

            return parseResult.Tree[0];
        }

        void IDisposable.Dispose()
        {
            _runtime.Dispose();
            _runtime = default;
            _context = default;
            _srcContext = default;
        }
    }
}
