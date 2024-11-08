using AutoCommand.Handler;
using Discord.WebSocket;
using Serilog;

namespace AutoCommand.Tests.Handler;

public class FakeCommandHandler : ICommandHandler
{
    public string CommandName => "FakeCommand";

    public Task HandleAsync(ILogger logger, SocketSlashCommand command)
    {
        return Task.CompletedTask;
    }
}