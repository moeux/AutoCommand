using Discord.WebSocket;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace AutoCommand.Handler;

/// <summary>
///     Default command router implementation, allowing to register commands to be considered for routing
/// </summary>
/// <example>
///     Minimal example registering the router to the discord client
///     <code>
///         var client = ...;
///         var router = new DefaultCommandRouter();
///         // Register command handler
///         router.Register(new SomeCommandHandler());
///         // Register router as default slash command handler
///         client.SlashCommandExecuted += command => router.HandleAsync(command);
///     </code>
/// </example>
public sealed class DefaultCommandRouter
{
    private readonly Dictionary<string, ICommandHandler> _handlers;
    private readonly ILogger _logger;

    /// <summary>
    ///     Initializes a new instance of <see cref="DefaultCommandRouter" />
    /// </summary>
    /// <param name="handlers">Optional <see cref="ICommandHandler" />s to register</param>
    /// <param name="logger">Optional custom <see cref="ILogger" /> instance for logging</param>
    /// <param name="logPath">Optional file log path. If omitted, logs will not be written to a file</param>
    public DefaultCommandRouter(
        IEnumerable<ICommandHandler>? handlers = default,
        ILogger? logger = default,
        string? logPath = default)
    {
        _handlers = handlers?.ToDictionary(handler => handler.CommandName, handler => handler) ??
                    new Dictionary<string, ICommandHandler>();
        _logger = logger ?? CreateLogger(logPath);
    }

    private static ILogger CreateLogger(string? logPath = null)
    {
        var loggerConfiguration = new LoggerConfiguration()
            .Destructure.ByTransforming<SocketSlashCommand>(command => new { command.Id, command.CommandName })
            .Destructure.ByTransformingWhere<dynamic>(type => typeof(SocketUser).IsAssignableFrom(type),
                user => new { user.Id, user.Username })
            .Destructure.ByTransforming<SocketRole>(role => new
                { role.Id, role.Name, role.Position, Permissions = role.Permissions.RawValue })
            .Enrich.FromLogContext()
            .WriteTo.Console(
                theme: AnsiConsoleTheme.Literate,
                outputTemplate:
                "[{Timestamp:HH:mm:ss} {Level:u3}] {Properties:j}{NewLine}{Message:lj}{NewLine}{Exception}");

        if (!string.IsNullOrWhiteSpace(logPath))
            loggerConfiguration
                .WriteTo.File(
                    logPath,
                    outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level:u3}] {Properties:j}{NewLine}{Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day);

        return loggerConfiguration
            .CreateLogger()
            .ForContext<DefaultCommandRouter>();
    }

    /// <summary>
    ///     Registers a command handler to be considered when routing commands
    /// </summary>
    /// <returns><c>true</c> if the handler was added, <c>false</c> if the handler is already registered</returns>
    /// <inheritdoc cref="Register" />
    public bool TryRegister(ICommandHandler handler)
    {
        ArgumentNullException.ThrowIfNull(handler);
        _logger.Information("Trying to register command handler: {@Handler}", handler);
        return _handlers.TryAdd(handler.CommandName, handler);
    }

    /// <summary>
    ///     Registers a command handler to be considered when routing commands, overwriting any existing handlers
    /// </summary>
    /// <param name="handler">The command handler to register</param>
    /// <exception cref="ArgumentNullException">Handler is <c>null</c></exception>
    public void Register(ICommandHandler handler)
    {
        ArgumentNullException.ThrowIfNull(handler);
        _logger.Information("Registering command handler: {@Handler}", handler);
        _handlers[handler.CommandName] = handler;
    }

    /// <summary>
    ///     Unregisters a command handler from the routing
    /// </summary>
    /// <param name="handler">The command handler to unregister</param>
    /// <returns>
    ///     <c>true</c> if the handler was found and removed, otherwise <c>false</c>
    /// </returns>
    public bool Unregister(ICommandHandler handler)
    {
        _logger.Information("Unregistering command handler: {@Handler}", handler);
        return handler is not null && _handlers.Remove(handler.CommandName);
    }

    /// <summary>
    ///     Routes the command to the appropriate <see cref="ICommandHandler" />, if any has been registered prior
    /// </summary>
    /// <param name="command">The command to be routed</param>
    /// <param name="cancellationToken">The cancellation token to cancel command handling</param>
    /// <remarks>If no <see cref="ICommandHandler" /> has been registered prior, this method will do nothing</remarks>
    public async Task HandleAsync(SocketSlashCommand command, CancellationToken cancellationToken = default)
    {
        var logger = _logger.ForContext("Token", command.Token);

        if (_handlers.TryGetValue(command.CommandName, out var handler))
        {
            logger.Information("Handling command {@Command} with handler {@Handler}", command, handler);

            await handler.HandleAsync(logger, command, cancellationToken);
        }
        else
        {
            logger.Error("No registered command handler for command: {@Command}", command);
        }
    }
}