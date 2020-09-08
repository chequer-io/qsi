using System.Text.RegularExpressions;
using net.sf.jsqlparser;
using net.sf.jsqlparser.parser;
using Qsi.Parsing;

namespace Qsi.JSql.Extensions
{
    internal static class JSQLParserExceptionExtension
    {
        private static readonly Regex _positionPattern = new Regex(@"at line (?<line>\d+), column (?<column>\d+).");

        public static QsiSyntaxErrorException AsSyntaxError(this JSQLParserException exception)
        {
            var cause = exception.getCause();

            switch (cause)
            {
                case ParseException parseException:
                    return AsSyntaxError(parseException);

                case TokenMgrException tokenMgrException:
                    return AsSyntaxError(tokenMgrException);
            }

            return new QsiSyntaxErrorException(0, 0, cause.Message);
        }

        private static QsiSyntaxErrorException AsSyntaxError(ParseException exception)
        {
            var match = _positionPattern.Match(exception.Message);
            var line = int.Parse(match.Groups["line"].Value);
            var column = int.Parse(match.Groups["column"].Value);
            var message = $"{exception.Message[..match.Index].TrimEnd()} {match.Value}";

            return new QsiSyntaxErrorException(line, column, message);
        }

        private static QsiSyntaxErrorException AsSyntaxError(TokenMgrException exception)
        {
            var match = _positionPattern.Match(exception.Message);
            var line = int.Parse(match.Groups["line"].Value);
            var column = int.Parse(match.Groups["column"].Value);

            return new QsiSyntaxErrorException(line, column, exception.Message);
        }
    }
}
