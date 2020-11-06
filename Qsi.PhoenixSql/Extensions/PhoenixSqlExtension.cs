using Google.Protobuf;
using PhoenixSql;

namespace Qsi.PhoenixSql.Extensions
{
    public static class PhoenixSqlExtension
    {
        public static IMessage Unwrap(IMessage message)
        {
            while (message is IProxyMessage<object> proxy && proxy.Message is IMessage proxyMessage)
            {
                message = proxyMessage;
            }

            return message;
        }
    }
}
