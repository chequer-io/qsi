namespace Qsi.Data
{
    public sealed class QsiReferenceAction : IQsiAction
    {
        public QsiReferenceType Type { get; set; }

        public QsiQualifiedIdentifier Target { get; set; }

        public QsiReferenceOperation Operation { get; set; }

        public QsiReferenceIsolationLevel IsolationLevel { get; set; }

        public QsiScript Definition { get; set; }
    }
}
