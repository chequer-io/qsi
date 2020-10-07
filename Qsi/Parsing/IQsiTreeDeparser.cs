using Qsi.Tree;

namespace Qsi.Parsing
{
    public interface IQsiTreeDeparser
    {
        string Deparse(IQsiTreeNode node);
    }
}
