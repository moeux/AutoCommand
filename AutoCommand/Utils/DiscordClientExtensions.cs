using AutoCommand.Config;
using Discord;
using Newtonsoft.Json;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.SystemConsole.Themes;

namespace AutoCommand.Utils;

/// <summary>
///     Provides extension methods for <see cref="IDiscordClient" />
/// </summary>
public static class DiscordClientExtensions
{
    private static readonly Logger Logger = new LoggerConfiguration()
        .WriteTo.Console(theme: AnsiConsoleTheme.Literate)
        .CreateLogger();

    /// <summary>
    ///     Creates slash commands found under the specified path
    /// </summary>
    /// <param name="client">The client to create commands with</param>
    /// <param name="commandPath">Directory path to slash command files</param>
    /// <param name="cancellationToken">The cancellation token to cancel slash command creation</param>
    public static async Task CreateSlashCommandsAsync(
        this IDiscordClient client, string commandPath, CancellationToken cancellationToken = default)
    {
        var path = Path.GetFullPath(commandPath);
        var directoryInfo = new DirectoryInfo(path);

        if (!directoryInfo.Exists)
        {
            Logger.Warning("No commands found under {Path}", path);
            return;
        }

        var existingCommands =
            await client.GetGlobalApplicationCommandsAsync(
                options: new RequestOptions { CancelToken = cancellationToken });
        var commandDeserializationTasks = directoryInfo.GetFiles("*.json").Select(async file =>
        {
            using var reader = file.OpenText();
            var content = await reader.ReadToEndAsync(cancellationToken);
            return JsonConvert.DeserializeObject<CommandConfig>(content);
        });
        var commandConfigs = await Task.WhenAll(commandDeserializationTasks);

        Logger.Information("Found {CommandConfigs} command configs under {Path}", commandConfigs.Length, path);

        var commandCreationTasks = commandConfigs
            .Where(commandConfig => commandConfig != null)
            .Where(commandConfig => existingCommands.All(command => command.Name != commandConfig!.Name))
            .Select(async commandConfig =>
            {
                var command = commandConfig!.ToSlashCommand();

                if (!commandConfig.IsGuildCommand)
                    return client.CreateGlobalApplicationCommand(
                        command,
                        new RequestOptions { CancelToken = cancellationToken });

                var guild = await client.GetGuildAsync(
                    commandConfig.GuildId.GetValueOrDefault(),
                    options: new RequestOptions { CancelToken = cancellationToken });
                return guild.CreateApplicationCommandAsync(
                    command,
                    new RequestOptions { CancelToken = cancellationToken });
            });
        var commands = await Task.WhenAll(commandCreationTasks);
        var skipped = commandConfigs.Length - commands.Length;

        Logger.Information("Created {Commands} new and skipped {Skipped} existing commands", commands.Length, skipped);
    }
}