using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Qsi.Data;
using Qsi.Parsing;
using Qsi.Services;

namespace Qsi.Compiler
{
    public sealed class QsiTableCompiler
    {
        public IQsiLanguageService LanguageService { get; }

        private readonly IQsiParser _parser;
        private readonly IQsiScriptParser _scriptParser;

        public QsiTableCompiler(IQsiLanguageService languageService)
        {
            LanguageService = languageService;
            _parser = LanguageService.CreateParser();
            _scriptParser = LanguageService.CreateScriptParser();
        }

        public Task<QsiTableResult> ExecuteAsync(QsiScript script)
        {
            throw new NotImplementedException();
        }

        public async IAsyncEnumerable<QsiTableResult> ExecuteAsync(string input)
        {
            foreach (var script in _scriptParser.Parse(input))
            {
                yield return await ExecuteAsync(script);
            }
        }
    }
}
