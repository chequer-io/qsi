namespace Qsi.Data
{
    public class QsiDataAction : IQsiAction
    {
        public QsiTableStructure Table { get; set; }

        public QsiDataRowCollection InsertRows { get; set; }

        public QsiDataRowCollection DuplicateRows { get; set; }

        public QsiDataRowCollection UpdateBeforeRows { get; set; }

        public QsiDataRowCollection UpdateAfterRows { get; set; }

        public QsiDataRowCollection DeleteRows { get; set; }
    }
}
