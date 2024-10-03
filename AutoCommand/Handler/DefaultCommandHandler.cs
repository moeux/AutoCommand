using Discord.WebSocket;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace AutoCommand.Handler;

public class DefaultCommandHandler
{
    private readonly Dictionary<string, ICommandHandler> _handlers;
    private readonly ILogger _logger;

    public DefaultCommandHandler(
        IEnumerable<ICommandHandler>? handlers = default,
        ILogger? logger = default,
        string? logPath = default)
    {
        _handlers = handlers?.ToDictionary(handler => handler.CommandName, handler => handler) ??
                    new Dictionary<string, ICommandHandler>();
        _logger = logger ?? CreateLogger(logPath);
    }

    private static ILogger CreateLogger(string? logPath = default)
    {
        var loggerConfiguration = new LoggerConfiguration()
            .Destructure.ByTransforming<SocketSlashCommand>(command => new { command.Id, command.CommandName })
            .Enrich.FromLogContext()
            .WriteTo.Console(
                theme: AnsiConsoleTheme.Literate,
                outputTemplate:
                "[{Timestamp:HH:mm:ss} {Level:u3}] {Properties:j}{NewLine}{Message:lj}{NewLine}{Exception}");

        if (!string.IsNullOrEmpty(logPath))
            loggerConfiguration
                .WriteTo.File(
                    logPath,
                    outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level:u3}] {Properties:j}{NewLine}{Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day);

        return loggerConfiguration
            .CreateLogger()
            .ForContext<DefaultCommandHandler>();
    }

    public bool Register(ICommandHandler handler)
    {
        _logger.Information("Registering command handler: {@Handler}", handler);
        return _handlers.TryAdd(handler.CommandName, handler);
    }

    public bool Unregister(ICommandHandler handler)
    {
        _logger.Information("Unregistering command handler: {@Handler}", handler);
        return _handlers.Remove(handler.CommandName);
    }

    public async Task HandleAsync(SocketSlashCommand command)
    {
        var logger = _logger.ForContext("Token", command.Token);
        var response = string.Empty;

        if (_handlers.TryGetValue(command.CommandName, out var handler))
        {
            if (handler.IsLongRunning) await command.DeferAsync(true);

            logger.Information("Handling command {@Command} with handler {@Handler}", command, handler);

            response = await handler.HandleAsync(command);
        }
        else
        {
            logger.Error("No registered command handler for command: {@Command}", command);
        }

        if (string.IsNullOrWhiteSpace(response))
            response = command.UserLocale == "de"
                ? "Etwas ist schiefgelaufen"
                : "Something went wrong";

        if (handler is { IsLongRunning: true })
            await command.FollowupAsync(response, ephemeral: true);
        else
            await command.RespondAsync(response, ephemeral: true);
    }
}