namespace Qsi.Impala.Tree
{
    public interface IImpalaTableNode
    {
        string PlanHints { get; set; }

        string TableSample { get; set; }
    }
}
