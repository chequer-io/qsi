using System.Text.RegularExpressions;
using net.sf.jsqlparser;
using net.sf.jsqlparser.expression;
using net.sf.jsqlparser.parser;
using net.sf.jsqlparser.statement;
using Qsi.Parsing;

namespace Qsi.JSql.Internal
{
    internal static class CCJSqlParserUtility
    {
        private static readonly Regex _positionPattern = new(@"at line (?<line>\d+), column (?<column>\d+).");

        #region CCJSqlParserUtil
        public static Statement Parse(string sql)
        {
            try
            {
                return CCJSqlParserUtil.parse(sql);
            }
            catch (JSQLParserException e)
            {
                throw CreateSyntaxError(e);
            }
        }

        public static Node ParseAST(string sql)
        {
            try
            {
                return CCJSqlParserUtil.parseAST(sql);
            }
            catch (JSQLParserException e)
            {
                throw CreateSyntaxError(e);
            }
        }

        public static Expression ParseExpression(string sql)
        {
            try
            {
                return CCJSqlParserUtil.parseExpression(sql);
            }
            catch (JSQLParserException e)
            {
                throw CreateSyntaxError(e);
            }
        }

        public static Statements ParseStatements(string sql)
        {
            try
            {
                return CCJSqlParserUtil.parseStatements(sql);
            }
            catch (JSQLParserException e)
            {
                throw CreateSyntaxError(e);
            }
        }

        public static Expression ParseConditionExpression(string sql)
        {
            try
            {
                return CCJSqlParserUtil.parseCondExpression(sql);
            }
            catch (JSQLParserException e)
            {
                throw CreateSyntaxError(e);
            }
        }
        #endregion

        private static QsiSyntaxErrorException CreateSyntaxError(JSQLParserException exception)
        {
            var cause = exception.getCause();

            switch (cause)
            {
                case ParseException parseException:
                    return CreateSyntaxError(parseException);

                case TokenMgrException tokenMgrException:
                    return CreateSyntaxError(tokenMgrException);
            }

            return new QsiSyntaxErrorException(0, 0, cause.Message);
        }

        private static QsiSyntaxErrorException CreateSyntaxError(ParseException exception)
        {
            var match = _positionPattern.Match(exception.Message);
            var line = int.Parse(match.Groups["line"].Value);
            var column = int.Parse(match.Groups["column"].Value);
            var message = $"{exception.Message[..match.Index].TrimEnd()} {match.Value}";

            return new QsiSyntaxErrorException(line, column, message);
        }

        private static QsiSyntaxErrorException CreateSyntaxError(TokenMgrException exception)
        {
            var match = _positionPattern.Match(exception.Message);
            var line = int.Parse(match.Groups["line"].Value);
            var column = int.Parse(match.Groups["column"].Value);

            return new QsiSyntaxErrorException(line, column, exception.Message);
        }
    }
}
