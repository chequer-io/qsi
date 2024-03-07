namespace Qsi.Diagnostics;

public interface IRawTree
{
    string DisplayName { get; }

    IRawTree[] Children { get; }
}