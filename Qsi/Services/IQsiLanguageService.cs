using Qsi.Compiler;
using Qsi.Data;
using Qsi.Parsing;

namespace Qsi.Services
{
    public interface IQsiLanguageService
    {
        QsiTableCompileOptions CreateCompileOptions();

        IQsiTreeParser CreateTreeParser();

        IQsiScriptParser CreateScriptParser();

        IQsiReferenceResolver CreateResolver();

        bool MatchIdentifier(QsiIdentifier x, QsiIdentifier y);
    }
}
