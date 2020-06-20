using Antlr4.Runtime;
using Qsi.Parsing;

namespace Qsi.Services
{
    public interface IQsiLanguageService
    {
        IQsiParser CreateParser();

        IQsiReferenceResolver CreateResolver();
    }
}
