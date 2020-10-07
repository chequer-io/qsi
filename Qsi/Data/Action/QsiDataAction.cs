namespace Qsi.Data
{
    public class QsiDataAction : IQsiAction
    {
        public QsiTableStructure Table { get; set; }

        public QsiDataRowCollection InsertRows { get; set; }

        public QsiDataRowCollection UpdateRows { get; set; }

        public QsiDataRowCollection DeleteRows { get; set; }
    }
}
