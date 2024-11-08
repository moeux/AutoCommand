# AutoCommand
AutoCommand is an extension library for the [Discord.Net](https://github.com/discord-net/Discord.Net) wrapper enabling to automatically add slash commands to your bot through JSON files. Additionaly it provides a basic slash command routing implementation.

# Slash Command Creation
AutoCommand adds an extension method to the Discord client, which takes care of the slash command creation:

```cs
var client = ...;

await client.CreateSlashCommandsAsync("path/to/commands");
```
## Slash Command Configuration
A slash command has following structure (not all fields are required):

```json
{
  "name": "kick",
  "description": "Kick user",
  "isDefaultPermission": true,
  "isNsfw": false,
  -- NOTE: Only add a guild ID if you intend to create a guild slash command instead of a global one -- 
  "guildId": <guildId>,
  "defaultMemberPermissions": [
    "KickMembers"
  ],
  "integrationTypes": [
    "GuildInstall"
  ],
  "contextTypes": [
    "Guild"
  ],
  "options": [
    <COMMAND_OPTION>
  ],
  "nameLocalizations": {
    "en-US": "kick"
  },
  "descriptionLocalizations": {
    "en-US": "Kick user"
  }
}
```
> [!NOTE]
> `defaultMemberPermissions`, `integrationTypes` and `contextTypes` values can be looked up in the Discord.Net library under `GuildPermission`, `ApplicationIntegrationType` and `InteractionContextType` respectively

## Slash Command Option Configuration
A slash command option has following structure (not all fields are required):

```json
{
  "name": "user",
  "description": "The user to kick",
  "type": "User",
  "isRequired": true,
  "isDefault": true,
  "isAutoComplete": false,
  -- NOTE: Not needed here, included for completeness --
  "minValue": 1,
  "maxValue": 2,
  "minLength": 1,
  "maxLength": 2,
  -- NOTE: Only useful if type == Channel -- 
  "channelTypes": [
    "Text", "DM"
  ],
  "options": [
    <COMMAND_OPTION>
  ],
  "choices": [
    <COMMAND_CHOICE>
  ],
  "nameLocalizations": {
    "en-US": "user"
  },
  "descriptionLocalizations": {
    "en-US": "The user to kick"
  }
}
```
> [!NOTE]
> `type` and `channelTypes` values can be looked up in the Discord.Net library under `ApplicationCommandOptionType` and `ChannelType` respectively

## Slash Command Choice Configuration
A slash command choice has following structure (localizations are not required):

```json
{
  "name": "choice",
  "value": "value",
  "nameLocalizations": {
    "en-US": "choice"
  }
}
```

# Slash Command Routing
You can use the [DefaultCommandRouter](AutoCommand/Handler/DefaultCommandRouter.cs) to register command handlers:

```cs
var client = ...;
var commandRouter = new DefaultCommandRouter();

commandRouter.Register(new MyCommandHandler());

client.SlashCommandExecuted += command => commandRouter.HandleAsync(command);
```

## Creating Command Handler
In order to create a custom command handler, you have to implement the [ICommandHandler](AutoCommand/Handler/ICommandHandler.cs) interface and register it on the command router as shown above:

```cs
public class MyCommandHandler : ICommandHandler
{
    public string CommandName => "mycommand";

    public async Task HandleAsync(ILogger logger, SocketSlashCommand command)
    {
        // Handle command ...

        logger.Information("Handling command!");
        await command.RespondAsync("beep!");
    }
}
```
