using Discord;
using Discord.Webhook;
using ForkHierarchy.Core.Options;
using Microsoft.Extensions.Options;
using Serilog.Core;
using Serilog.Events;

namespace ForkHierarchy.Core.Sinks;

public class DiscordSink : ILogEventSink
{
    private readonly IFormatProvider? _formatProvider;
    private readonly IOptions<DiscordOptions> _discordOptions;
    private readonly TimeZoneInfo _europeanTimeZone;

    public DiscordSink(IFormatProvider? formatProvider, IOptions<DiscordOptions> discordOptions)
    {
        _formatProvider = formatProvider;
        _discordOptions = discordOptions;
        _europeanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
    }

    public void Emit(LogEvent logEvent)
    {
        Task.Run(async () => await EmitAsync(logEvent));
    }

    public async Task EmitAsync(LogEvent logEvent)
    {
        if (logEvent.Level is LogEventLevel.Error or LogEventLevel.Fatal)
        {
            var message = logEvent.RenderMessage(_formatProvider);

            using var client = new DiscordWebhookClient(_discordOptions.Value.WebHookUrl);

            var timeStamp = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, _europeanTimeZone);

            var embed = new EmbedBuilder()
                .WithTitle(logEvent.Level.ToString())
                //.WithTitle(timeStamp.ToString("dd.MM.yyyy HH:mm:ss"))
                .WithDescription($"```{message}```")
                .WithTimestamp(timeStamp)
                .WithAuthor("ForkHierarchy")
                .WithColor(Color.DarkRed)
                .Build();

            await client.SendMessageAsync(embeds: new[] { embed });
        }
    }
}