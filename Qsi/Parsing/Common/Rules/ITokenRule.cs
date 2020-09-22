namespace Qsi.Parsing.Common.Rules
{
    public interface ITokenRule
    {
        void Run(CommonScriptCursor cursor);
    }
}