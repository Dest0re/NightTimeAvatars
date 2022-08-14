using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace NightTimeAvatars;

[System.Runtime.Versioning.SupportedOSPlatform("windows")]
internal class AvatarChangeService : BackgroundService
{
    private readonly DayNightTimer _dayNightTimer;
    private readonly ILogger<AvatarChangeService> _logger;
    private readonly Settings? _settings;

    public AvatarChangeService(IConfiguration configuration, ILogger<AvatarChangeService> logger)
    {
        _settings = configuration.GetSection("Settings").Get<Settings>();
        _logger = logger;
        
        if (_settings == null) throw new NullReferenceException("Settings parsing error");
        
        var discordAvatarChanger = new DiscordAvatarChanger(_settings.DiscordToken);
        _dayNightTimer = new DayNightTimer(_settings.SetDayTimeAvatarAtTimeOnly, _settings.SetNightTimeAvatarAtTimeOnly);

        _dayNightTimer.OnDayNotify += async () =>
        {
            logger.LogInformation("It's a daytime! Changing avatar...");
            await discordAvatarChanger.ChangeAvatarAsync(_settings.DayTimeAvatarFilepath);
            logger.LogInformation("The avatar has been changed!");

        };
        _dayNightTimer.OnNightNotify += async () =>
        {
            logger.LogInformation("It's a nighttime... Changing avatar...");
            await discordAvatarChanger.ChangeAvatarAsync(_settings.NightTimeAvatarFilepath);
            logger.LogInformation("The avatar has been changed!");
        };
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var exeLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
        
        _logger.LogInformation(
            "Hello! I will change your avatar to daytime version at {SetDayTimeAvatarAt} and to nighttime " +
            "version at {SetNightTimeAvatarAt}\n" + 
            "DayTime avatar path is `{DayTimeAvatarPath}`\n " +
            "and NightTime avatar path is {NightTimeAvatarPath}\n" +
            "You can adjust these settings in {AppSettingsLocation} file.",
            _settings?.SetDayTimeAvatarAt, _settings?.SetNightTimeAvatarAt, 
            _settings?.DayTimeAvatarFilepath, _settings?.NightTimeAvatarFilepath,
            Path.GetDirectoryName(exeLocation) + "/appsettings.json"
        );
        
        await _dayNightTimer.StartAsync(stoppingToken);
    }
}


[System.Runtime.Versioning.SupportedOSPlatform("windows")]
internal static class NightTimeAvatars
{

    private static void Main(string[] args)
    {
        using var host = CreateHostBuilder(args).UseConsoleLifetime().Build();

        host.Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.AddHostedService<AvatarChangeService>();
            });
    }
}
