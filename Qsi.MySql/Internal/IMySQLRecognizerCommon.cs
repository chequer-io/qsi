namespace Qsi.MySql.Internal
{
    internal interface IMySQLRecognizerCommon
    {
        int serverVersion { get; }

        bool isSqlModeActive(int mode);
    }
}
