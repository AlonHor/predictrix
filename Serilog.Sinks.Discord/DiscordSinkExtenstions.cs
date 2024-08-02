using Serilog.Configuration;
using Serilog.Events;

namespace Serilog.Sinks.Discord
{
    public static class DiscordSinkExtenstions
    {
        public static LoggerConfiguration Discord(this LoggerSinkConfiguration loggerConfiguration,
            ulong webhookId,
            LogEventLevel restrictedToMinimumLevel1,
            string webhookToken,
            IFormatProvider formatProvider = null,
            LogEventLevel restrictedToMinimumLevel = LogEventLevel.Verbose)
        {
            return loggerConfiguration.Sink(
                new DiscordSink(formatProvider, webhookId, webhookToken, restrictedToMinimumLevel));
        }
    }
}