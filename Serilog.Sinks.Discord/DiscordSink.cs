using Discord.Webhook;
using Serilog.Core;
using Serilog.Events;
using System.Text;

namespace Serilog.Sinks.Discord
{
    public class DiscordSink : ILogEventSink
    {
        private readonly IFormatProvider _formatProvider;
        private readonly UInt64 _webhookId;
        private readonly string _webhookToken;
        private readonly LogEventLevel _restrictedToMinimumLevel;

        public DiscordSink(
            IFormatProvider formatProvider,
            UInt64 webhookId,
            string webhookToken,
            LogEventLevel restrictedToMinimumLevel = LogEventLevel.Information)
        {
            _formatProvider = formatProvider;
            _webhookId = webhookId;
            _webhookToken = webhookToken;
            _restrictedToMinimumLevel = restrictedToMinimumLevel;
        }
        

        public void Emit(LogEvent logEvent)
        {
            if (!ShouldLogMessage(_restrictedToMinimumLevel, logEvent.Level))
                return;

            var webHook = new DiscordWebhookClient(_webhookId, _webhookToken);
            var messageBuilder = new StringBuilder();

            try
            {
                messageBuilder.Append(GetCodeTag(logEvent.Level));
                messageBuilder.Append(logEvent.RenderMessage(_formatProvider));
                if (logEvent.Exception is not null)
                {
                    messageBuilder.AppendLine($"Exception: {logEvent.Exception.GetType().FullName}");
                    messageBuilder.AppendLine(logEvent.Exception.Message);
                    messageBuilder.AppendLine(logEvent.Exception.StackTrace);
                }
                messageBuilder.AppendLine("```");

                webHook.SendMessageAsync(messageBuilder.ToString(), false)
                    .GetAwaiter()
                    .GetResult();
            }

            catch (Exception ex)
            {
                webHook.SendMessageAsync(
                    $"```arm\nooo snap, {ex.Message}\n```", false)
                    .GetAwaiter()
                    .GetResult();
            }
        }

        public static string GetCodeTag(LogEventLevel level)
        {
            return level switch
            {
                LogEventLevel.Information => "```diff\n+ ", // green
                LogEventLevel.Error or LogEventLevel.Fatal => "```diff\n- ", // red
                LogEventLevel.Warning => "```fix\n",
                _ => "```\n", // grey
            };
        }

        private static bool ShouldLogMessage(
            LogEventLevel minimumLogEventLevel,
            LogEventLevel messageLogEventLevel) =>
                (int)messageLogEventLevel < (int)minimumLogEventLevel ? false : true;
    }
}
