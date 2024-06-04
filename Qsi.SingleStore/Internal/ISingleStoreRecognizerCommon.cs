namespace Qsi.SingleStore.Internal;

internal interface ISingleStoreRecognizerCommon
{
    int serverVersion { get; }

    bool MariaDB { get; }

    bool isSqlModeActive(int mode);
}
