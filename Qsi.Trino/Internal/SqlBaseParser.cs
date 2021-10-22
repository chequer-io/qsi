namespace Qsi.Trino.Internal
{
    internal partial class SqlBaseParser
    {
        private int _bindParamIndex = 1;

        protected int NextBindParameterIndex()
        {
            return _bindParamIndex++;
        }
    }
}
