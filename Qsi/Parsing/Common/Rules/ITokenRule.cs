namespace Qsi.Parsing.Common.Rules;

public interface ITokenRule
{
    bool Run(CommonScriptCursor cursor);
}