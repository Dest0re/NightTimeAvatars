namespace NightTimeAvatars;

internal class Settings
{
    public string DiscordToken { get; set; }  = null!;
    public string SetDayTimeAvatarAt { get; set; } = "08:00:00";
    public string SetNightTimeAvatarAt { get; set; } = "23:00:00";
    public string DayTimeAvatarFilepath { get; set; } = null!;
    public string NightTimeAvatarFilepath { get; set; } = null!;

    public TimeOnly SetDayTimeAvatarAtTimeOnly
    {
        get => TimeOnly.Parse(SetDayTimeAvatarAt);
        set => SetDayTimeAvatarAt = value.ToString();
    }
    
    public TimeOnly SetNightTimeAvatarAtTimeOnly
    {
        get => TimeOnly.Parse(SetNightTimeAvatarAt);
        set => SetNightTimeAvatarAt = value.ToString();
    }
}