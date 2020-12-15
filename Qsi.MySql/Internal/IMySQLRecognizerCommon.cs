namespace Qsi.MySql.Internal
{
    public interface IMySQLRecognizerCommon
    {
        long serverVersion { get; set; }

        bool isSqlModeActive(int mode);
    }
}
