using System;
using JavaScriptEngineSwitcher.ChakraCore;
using Newtonsoft.Json;
using Qsi.Parsing;
using Qsi.PostgreSql.Internal.Postgres.Converters;

namespace Qsi.PostgreSql.Internal
{
    internal abstract class PgQueryBase<T> : IPgParser
    {
        private ChakraCoreJsEngine _jsEngine;

        private bool _initialized;
        private readonly JsonSerializerSettings _serializerSettings;

        protected PgQueryBase()
        {
            _serializerSettings = new JsonSerializerSettings
            {
                Converters =
                {
                    new PgTreeConverter()
                }
            };
        }

        private void Initialize()
        {
            if (_initialized)
                return;

            string parserJs = GetParserScript();

            try
            {
                _jsEngine = new ChakraCoreJsEngine();
                _jsEngine.Execute(parserJs);
            }
            catch
            {
                _jsEngine?.Dispose();
                _jsEngine = null;
                throw;
            }

            _initialized = true;
        }

        protected abstract string GetParserScript();

        protected abstract string GetParseScript(string input);

        public IPgNode Parse(string input)
        {
            Initialize();

            var parseJs = GetParseScript(input);

            var json = _jsEngine.Evaluate<string>(parseJs);
            var parseResult = JsonConvert.DeserializeObject<PgParseResult>(json, _serializerSettings);

            if (parseResult?.Error != null)
            {
                // TODO: Measure line, column number by Error.CursorPosition
                throw new QsiSyntaxErrorException(0, 0, parseResult.Error.Message);
            }

            if (!string.IsNullOrEmpty(parseResult?.StandardError))
            {
                throw new QsiException(QsiError.Internal, parseResult.StandardError);
            }

            if (parseResult?.Tree?.Length != 1)
                throw new InvalidOperationException();

            return parseResult.Tree[0];
        }

        void IDisposable.Dispose()
        {
            _jsEngine?.Dispose();
            _jsEngine = null;
        }
    }
}
