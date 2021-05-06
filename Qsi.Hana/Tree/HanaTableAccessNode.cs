using Qsi.Tree;

namespace Qsi.Hana.Tree
{
    public sealed class HanaTableAccessNode : QsiTableAccessNode
    {
        // FOR SYSTEM_TIME AS OF '123'
        // FOR SYSTEM_TIME AS FROM '123' TO '123'
        // FOR SYSTEM_TIME AS BETWEEN '123' AND '123'
        public string ForSystemTime { get; set; }

        // FOR APPLICAITON_TIME AS OF '123'
        public string ForApplicationTime { get; set; }

        // PARTITION (1, 2, ..)
        public string Partition { get; set; }
    }
}
