using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using telegram_bot;
using telegram_bot.Services;
using Refit;
using telegram_bot.Repositories;

var configuration = new ConfigurationBuilder()
           .SetBasePath(AppContext.BaseDirectory)           
           .AddJsonFile("appsettings.json", false)
           .Build();


var serviceProvider = new ServiceCollection()
            .AddLogging(config =>
            {
                config.ClearProviders();
                config.AddConfiguration(configuration.GetSection("Logging"));
                config.AddConsole();
                config.AddDebug();
            })
            .AddRefitClient<IWaifuService>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://api.waifu.pics"))
            .Services
            .AddSingleton<IConfiguration>(configuration)
            .AddSingleton<telegram_bot.Telegram>()
            .AddTransient<ICacheRepository, DictionaryCacheRepository>()
            //.AddTransient<IWaifuService, WaifuService>()
            .BuildServiceProvider();

var logger = serviceProvider.GetService<ILoggerFactory>()!
    .CreateLogger<Program>();

logger.LogInformation("Starting application");

//do the actual work here
var telegramBot = serviceProvider.GetService<telegram_bot.Telegram>();
//bar.DoSomeRealWork();

logger.LogDebug("All done!");

Console.ReadLine();