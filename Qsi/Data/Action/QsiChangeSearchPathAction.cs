namespace Qsi.Data
{
    public class QsiChangeSearchPathAction : IQsiAction
    {
        public QsiQualifiedIdentifier[] Identifiers { get; }

        public QsiChangeSearchPathAction(QsiQualifiedIdentifier[] identifiers)
        {
            Identifiers = identifiers;
        }
    }
}
