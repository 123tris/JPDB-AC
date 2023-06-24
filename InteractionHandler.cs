using System.Reflection;
using System.Windows.Input;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

public class InteractionHandler
{
    private readonly DiscordSocketClient client;
    private readonly InteractionService commands;
    private readonly IServiceProvider services;

    public InteractionHandler(DiscordSocketClient client, InteractionService commands, IServiceProvider services)
    {
        this.client = client;
        this.commands = commands;
        this.services = services;
    }

    public async Task InitializeAsync()
    {
        await commands.AddModulesAsync(Assembly.GetEntryAssembly(), services);
        commands.Modules.ToList().ForEach(module => Console.WriteLine($"Loaded module: {module.Name}"));
        commands.SlashCommands.ToList().ForEach(command => Console.WriteLine($"Loaded command: {command.Name}"));

        var commandBuilder = new SlashCommandBuilder();
        foreach (SlashCommandInfo command in commands.SlashCommands)
        {
            commandBuilder.WithName(command.Name);
            commandBuilder.WithDescription(command.Description);
            commandBuilder.WithDMPermission(true);
        }

        await client.CreateGlobalApplicationCommandAsync(commandBuilder.Build());

        client.InteractionCreated += HandleInteractionAsync;
    }

    private async Task HandleInteractionAsync(SocketInteraction interaction)
    {
        try
        {
            SocketInteractionContext context = new(client, interaction);
            IResult? result = await commands.ExecuteCommandAsync(context, services);

            if (!result.IsSuccess)
            {
                await context.Channel.SendMessageAsync($"Error: {result.ErrorReason}");
            }
        } catch (Exception e)
        {
            Console.WriteLine(e);

            if (interaction.Type == InteractionType.ApplicationCommand)
                await interaction.GetOriginalResponseAsync().ContinueWith(async msg => await msg.Result.DeleteAsync());
        }
    }
}