using System;
using Newtonsoft.Json;
using Qsi.Parsing;
using Qsi.PostgreSql.Internal.PG10.Types;
using Qsi.PostgreSql.Internal.Serialization.Converters;
using Qsi.PostgreSql.Resources;
using Qsi.PostgreSql.Tree;
using Qsi.PostgreSql.Tree.PG10;

namespace Qsi.PostgreSql.Internal.PG10
{
    internal class PgQuery10 : PgQueryBase<IPg10Node>
    {
        private readonly JsonSerializerSettings _serializerSettings;

        public PgQuery10()
        {
            _serializerSettings = new JsonSerializerSettings
            {
                Converters =
                {
                    new PgTreeConverter()
                }
            };
        }

        protected override void OnInitialize()
        {
            Execute(ResourceManager.GetResourceContent("pg_query_10.js"));
        }

        protected override IPg10Node Parse(string input)
        {
            var json = Evaluate($"JSON.stringify(PgQuery.parse({JsonConvert.SerializeObject(input)}))");
            var parseResult = JsonConvert.DeserializeObject<PgParseResult<IPg10Node>>(json, _serializerSettings);

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

        protected override PgActionVisitor CreateActionVisitor(IPgVisitorSet set)
        {
            return new(set);
        }

        protected override PgTableVisitor CreateTableVisitor(IPgVisitorSet set)
        {
            return new(set);
        }

        protected override PgExpressionVisitor CreateExpressionVisitor(IPgVisitorSet set)
        {
            return new(set);
        }

        protected override PgDefinitionVisitor CreateDefinitionVisitor(IPgVisitorSet set)
        {
            return new(set);
        }

        protected override PgIdentifierVisitor CreateIdentifierVisitor(IPgVisitorSet set)
        {
            return new(set);
        }
    }
}
