using System;
using System.Threading;
using JavaScriptEngineSwitcher.ChakraCore;
using JavaScriptEngineSwitcher.Core;
using Qsi.PostgreSql.Tree;
using Qsi.PostgreSql.Tree.PG10;

namespace Qsi.PostgreSql.Internal
{
    internal abstract class PgQueryBase<T> : IPgParser where T : IPgNode
    {
        private ChakraCoreJsEngine _jsEngine;
        private bool _initialized;

        private void Initialize(CancellationToken token)
        {
            if (_initialized)
                return;

            try
            {
                _jsEngine = new ChakraCoreJsEngine();
                OnInitialize(token);
            }
            catch
            {
                _jsEngine?.Dispose();
                _jsEngine = null;
                throw;
            }

            _initialized = true;
        }

        protected virtual void OnInitialize(CancellationToken token)
        {
        }

        protected abstract T Parse(string input, CancellationToken token);

        protected abstract PgActionVisitor CreateActionVisitor(IPgVisitorSet set);

        protected abstract PgTableVisitor CreateTableVisitor(IPgVisitorSet set);

        protected abstract PgExpressionVisitor CreateExpressionVisitor(IPgVisitorSet set);

        protected abstract PgDefinitionVisitor CreateDefinitionVisitor(IPgVisitorSet set);

        protected abstract PgIdentifierVisitor CreateIdentifierVisitor(IPgVisitorSet set);

        protected string Evaluate(string expression, CancellationToken token)
        {
            try
            {
                using (token.Register(() => _jsEngine?.Interrupt()))
                {
                    return _jsEngine.Evaluate<string>(expression);
                }
            }
            catch (JsInterruptedException)
            {
                token.ThrowIfCancellationRequested();

                throw;
            }
        }

        protected void Execute(string code, CancellationToken token)
        {
            try
            {
                using (token.Register(() => _jsEngine?.Interrupt()))
                {
                    _jsEngine.Execute(code);
                }
            }
            catch (JsInterruptedException)
            {
                token.ThrowIfCancellationRequested();

                throw;
            }
        }

        IPgNode IPgParser.Parse(string input, CancellationToken token)
        {
            Initialize(token);
            return Parse(input, token);
        }

        IPgVisitorSet IPgParser.CreateVisitorSet()
        {
            var set = new PgVisitorSetImpl();

            set.ActionVisitor = CreateActionVisitor(set);
            set.TableVisitor = CreateTableVisitor(set);
            set.ExpressionVisitor = CreateExpressionVisitor(set);
            set.DefinitionVisitor = CreateDefinitionVisitor(set);
            set.IdentifierVisitor = CreateIdentifierVisitor(set);

            return set;
        }

        void IDisposable.Dispose()
        {
            _jsEngine?.Dispose();
            _jsEngine = null;
        }

        private sealed class PgVisitorSetImpl : IPgVisitorSet
        {
            public PgActionVisitor ActionVisitor { get; set; }

            public PgTableVisitor TableVisitor { get; set; }

            public PgExpressionVisitor ExpressionVisitor { get; set; }

            public PgDefinitionVisitor DefinitionVisitor { get; set; }

            public PgIdentifierVisitor IdentifierVisitor { get; set; }
        }
    }
}
