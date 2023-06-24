struct BotConfig
{
    public readonly string token;
    public readonly ulong guildId;

    public BotConfig(string token, ulong guildId)
    {
        this.token = token;
        this.guildId = guildId;
    }
}