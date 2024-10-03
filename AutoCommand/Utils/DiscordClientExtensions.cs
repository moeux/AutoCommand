using AutoCommand.Config;
using Discord;
using Newtonsoft.Json;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.SystemConsole.Themes;

namespace AutoCommand.Utils;

public static class DiscordClientExtensions
{
    private static readonly Logger Logger = new LoggerConfiguration()
        .WriteTo.Console(theme: AnsiConsoleTheme.Literate)
        .CreateLogger();

    public static async Task CreateSlashCommands(this IDiscordClient client, string commandPath)
    {
        var path = Path.GetFullPath(commandPath);
        var directoryInfo = new DirectoryInfo(path);

        if (!directoryInfo.Exists)
        {
            Logger.Warning("No commands found under {Path}", path);
            return;
        }

        var existingCommands = await client.GetGlobalApplicationCommandsAsync();
        var commandDeserializationTasks = directoryInfo.GetFiles().Select(async file =>
        {
            using var reader = file.OpenText();
            var content = await reader.ReadToEndAsync();
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
                    return client.CreateGlobalApplicationCommand(command);

                var guild = await client.GetGuildAsync(commandConfig.GuildId.GetValueOrDefault());
                return guild.CreateApplicationCommandAsync(command);
            });
        var commands = await Task.WhenAll(commandCreationTasks);
        var skipped = commandConfigs.Length - commands.Length;

        Logger.Information("Created {Commands} new and skipped {Skipped} existing commands", commands.Length, skipped);
    }
}