namespace Qsi.Data
{
    public sealed class QsiAction
    {
        public QsiActionType Type { get; set; }

        public QsiQualifiedIdentifier Target { get; set; }

        public object Payload { get; set; }
    }
}
