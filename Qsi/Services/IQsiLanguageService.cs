using Qsi.Data;
using Qsi.Parsing;

namespace Qsi.Services
{
    public interface IQsiLanguageService
    {
        IQsiTreeParser CreateTreeParser();

        IQsiScriptParser CreateScriptParser();

        IQsiReferenceResolver CreateResolver();

        bool MatchIdentifier(QsiQualifiedIdentifier x, QsiQualifiedIdentifier y);
    }
}
