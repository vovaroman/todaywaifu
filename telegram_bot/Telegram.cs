using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.InlineQueryResults;
using telegram_bot.Services;
using telegram_bot.Repositories;
using telegram_bot.Models;

namespace telegram_bot
{
    internal class Telegram
    {
        private TelegramBotClient _botClient;

        private ILogger<Telegram> _logger;
        
        private IConfiguration _configuration;

        private IWaifuService _waifuService;

        private Random _random;
        private ICacheRepository _cacheRepository;
        public Telegram(IConfiguration configuration, ILogger<Telegram> logger, IWaifuService waifuService, ICacheRepository cacheRepository)
        {

            (_random, _logger, _waifuService, _cacheRepository, _configuration) = 
                (new Random(), logger, waifuService, cacheRepository, configuration);

            _logger.LogDebug(configuration.GetSection("API").Value);

            _botClient = new TelegramBotClient(configuration.GetSection("API").Value);

            using var cts = new CancellationTokenSource();

            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            _botClient.StartReceiving(updateHandler: HandleUpdateAsync,
                               errorHandler: HandleErrorAsync,
                               receiverOptions: new ReceiverOptions()
                               {
                                   AllowedUpdates = Array.Empty<UpdateType>()
                               },
                               cancellationToken: cts.Token);
            //Console.ReadLine();

            // Send cancellation request to stop bot
            //cts.Cancel();

        }

        Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var handler = update.Type switch
            {

                UpdateType.InlineQuery => BotOnInlineQueryReceived(botClient, update.InlineQuery!),
                _ => UnknownUpdateHandlerAsync(botClient, update)
            };

            try
            {
                await handler;
            }
            catch (Exception exception)
            {
                await HandleErrorAsync(botClient, exception, cancellationToken);
            }
        }

        private async Task BotOnInlineQueryReceived(ITelegramBotClient botClient, InlineQuery inlineQuery)
        {
            Console.WriteLine($"Received inline query from: {inlineQuery.From.Id}");

            var isCached = _cacheRepository.HasInRepository(inlineQuery.From.Id);

            WaifuResponse response;

            var rand = _random.Next(10);

            if (isCached)
            {
                var cachedUserResponse = _cacheRepository.GetFromRepository(inlineQuery.From.Id);
                if ((DateTime.Now - cachedUserResponse.DateTime).TotalDays < 1)
                {
                    response = new WaifuResponse() { Url = cachedUserResponse.Url };
                }
                else
                {
                    response = rand == 1 || rand == 2 ? await _waifuService.GetRandomNSFWTrap() : await _waifuService.GetRandomNSFWWaifu();
                    _cacheRepository.UpdateInRepository(new UserResponse() {
                            DateTime = DateTime.Now,
                            Id = inlineQuery.From.Id,
                            Url = response.Url
                            
                    });
                }
            }
            else
            {
                response = rand == 1 || rand == 2 ? await _waifuService.GetRandomNSFWTrap() : await _waifuService.GetRandomNSFWWaifu();
                _cacheRepository.AddToRepository(new UserResponse()
                {
                    DateTime = DateTime.Now,
                    Id = inlineQuery.From.Id,
                    Url = response.Url
                });
            }

            var notGay = await _waifuService.GetRandomNSFWWaifu();
            var gay = await _waifuService.GetRandomNSFWTrap();

            InlineQueryResult[] results = {
                new InlineQueryResultArticle(
                    id: "3",
                    title: "my today waifu?",
                    inputMessageContent: new InputTextMessageContent(response.Url)
                ),
                new InlineQueryResultArticle(
                    id: "4",
                    title: "Random waifu",
                    inputMessageContent: new InputTextMessageContent(notGay.Url)
                ),
                new InlineQueryResultArticle(
                    id: "5",
                    title: "GAY?",
                    inputMessageContent: new InputTextMessageContent(gay.Url)
                )

            };

            await botClient.AnswerInlineQueryAsync(inlineQueryId: inlineQuery.Id,
                                                   results: results,
                                                   isPersonal: true,
                                                   cacheTime: 0);
        }

        private Task UnknownUpdateHandlerAsync(ITelegramBotClient botClient, Update update)
        {
            Console.WriteLine($"Unknown update type: {update.Type}");
            return Task.CompletedTask;
        }

    }
}
