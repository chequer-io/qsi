namespace Qsi.MySql.Internal
{
    internal interface IMySqlRecognizerCommon
    {
        int serverVersion { get; }

        bool isSqlModeActive(int mode);
    }
}
