using Discord.WebSocket;
using Serilog;

namespace AutoCommand.Handler;

public interface ICommandHandler
{
    public string CommandName { get; }

    public Task HandleAsync(ILogger logger, SocketSlashCommand command, CancellationToken cancellationToken = default);
}