using Discord;
using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

class Program
{
    static void Main() => new Program().StartAsync().GetAwaiter().GetResult();

    private const string PROGRAM_SETTINGS_PATH = "settings.json";

    private InteractionHandler interactionHandler;
    private readonly DiscordSocketClient client;
    private readonly InteractionService commands;
    private readonly IServiceProvider services;
    private readonly BotConfig config;

    Program()
    {
        DiscordSocketConfig socketConfig = new()
        {
            GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMembers
        };
        client = new DiscordSocketClient(socketConfig);
        commands = new InteractionService(client);
        services = CreateServices();


        // Load settings from file
        string json = File.ReadAllText(PROGRAM_SETTINGS_PATH);
        config = JsonConvert.DeserializeObject<BotConfig>(json);
    }

    private IServiceProvider CreateServices()
    {
        var services = new ServiceCollection()
            .AddSingleton(client)
            .AddSingleton(commands)
            .AddSingleton<InteractionHandler>()
            // Register other services or dependencies here
            .BuildServiceProvider();

        return services;
    }

    public async Task StartAsync()
    {
        //Add log method to callback
        client.Log += message =>
        {
            Console.WriteLine(message.ToString());
            return Task.CompletedTask;
        };

        client.Ready += ClientReady;

        //Connect to server
        await client.LoginAsync(TokenType.Bot, config.token);
        await client.StartAsync();

        await Task.Delay(-1);
    }

    public async Task ClientReady()
    {
        interactionHandler = new(client, commands, services);
        await interactionHandler.InitializeAsync();
    }
}