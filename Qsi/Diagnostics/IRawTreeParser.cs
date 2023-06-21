namespace Qsi.Diagnostics;

public interface IRawTreeParser
{
    IRawTree Parse(string input);
}