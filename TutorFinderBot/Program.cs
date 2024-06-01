//

using MySql.Data.MySqlClient;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TutorFinderBot;

class Program
{
    private static ITelegramBotClient botClient;

    static async Task Main()
    {
        botClient = new TelegramBotClient("6995441069:AAEhbPw_PFFRu0145prymHYeHmrRhsGGU8Q");

        var me = await botClient.GetMeAsync();
        Console.WriteLine($"Bot {me.Id} {me.FirstName} started working");

        var commands = new[]
        {
            new BotCommand { Command = "start", Description = "Запустить бота" },
            new BotCommand { Command = "vacancies", Description = "Показать вакансии" },
            new BotCommand { Command = "users", Description = "Показать всех пользователей" }
        };

        await botClient.SetMyCommandsAsync(commands);


        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = { } // получать все типы обновлений
        };

        botClient.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            receiverOptions,
            cancellationToken
        );

        Console.WriteLine("Press any key to stop bot");
        Console.ReadKey();
        cts.Cancel();
    }

    private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Type == UpdateType.Message && update.Message!.Type == MessageType.Text)
        {
            var messageText = update.Message.Text.ToLower();
            var chatId = update.Message.Chat.Id;

            switch (messageText)
            {
                case "/start":
                    await botClient.SendTextMessageAsync(chatId, "Ласкаво просимо до чат-боту для пошуку репетитора! Як я можу вам допомогти?", cancellationToken: cancellationToken);
                    break;

                case "/vacancies":
                    var vacancies = DataBaseConnectionClass.GetActualVacancies();
                    var vacanciesText = vacancies.Count == 0 ? "Вакансії не знайдено." : string.Join("\n", vacancies.Select(v => $"{v.Subject} - {v.HourlyRate} грн/година"));
                    await botClient.SendTextMessageAsync(chatId, vacanciesText, cancellationToken: cancellationToken);
                    break;

                case "/users":
                    var users = DataBaseConnectionClass.GetUsersAll();
                    var usersText = users.Count == 0 ? "Користувачі не знайдені." : string.Join("\n", users.Select(u => $"{u.FirstName} {u.LastName} - Login: {u.Username}"));
                    await botClient.SendTextMessageAsync(chatId, usersText, cancellationToken: cancellationToken);
                    break;

                default:
                    await botClient.SendTextMessageAsync(chatId, "Вибачте, я не розумію цю команду.", cancellationToken: cancellationToken);
                    break;
            }
        }
    }

    private static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException => $"Помилка API Telegram:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(errorMessage);
        return Task.CompletedTask;
    }
}
