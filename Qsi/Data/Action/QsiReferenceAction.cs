using Qsi.Analyzers;

namespace Qsi.Data
{
    public sealed class QsiReferenceAction : IQsiAnalysisResult
    {
        public QsiReferenceType Type { get; set; }

        public QsiQualifiedIdentifier Target { get; set; }

        public QsiReferenceOperation Operation { get; set; }

        public QsiReferenceIsolationLevel IsolationLevel { get; set; }

        public QsiScript Definition { get; set; }
        
        public QsiSensitiveDataCollection SensitiveDataCollection => QsiSensitiveDataCollection.Empty;
    }
}
