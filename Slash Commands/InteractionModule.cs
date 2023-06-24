using Discord;
using Discord.Commands;
using Discord.Interactions;

public class InteractionModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("test", "Receive a message")]
    public async Task Ping()
    {
        ulong channelId = Context.Channel.Id;
        if (Context.Channel is ITextChannel channel)
        {
            IThreadChannel? newThread = await channel.CreateThreadAsync(
                name: "test",
                autoArchiveDuration: ThreadArchiveDuration.OneHour,
                invitable: false,
                type: ThreadType.PublicThread
            );
            await newThread.SendMessageAsync("Thread test message");
        }
        else
        {
            await RespondAsync($"Channel {channelId} is not a text channel");
            return;
        }

        await RespondAsync(text: $"Created thread");
    }
}