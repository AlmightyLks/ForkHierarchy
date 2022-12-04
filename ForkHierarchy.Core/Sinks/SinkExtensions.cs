using ForkHierarchy.Core.Options;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Configuration;

namespace ForkHierarchy.Core.Sinks;

public static class SinkExtensions
{
    public static LoggerConfiguration Discord(
                  this LoggerSinkConfiguration loggerConfiguration,
                  IOptions<DiscordOptions> options,
                  IFormatProvider? formatProvider = null)
    {
        return loggerConfiguration.Sink(new DiscordSink(formatProvider, options));
    }
}