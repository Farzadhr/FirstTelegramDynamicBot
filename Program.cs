using Newtonsoft.Json.Converters;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

List<string> botTokens = new List<string>
        {
            "7373111717:AAHLUsD_9sBTZo4oD9n6fkKgdoZAhHxXaA4",
            "5876329408:AAHqB2htETZIpqEDw9Llrbxdp-wbn-6O5ks",
            // Add more tokens as needed
        };
Dictionary<string, bool> isbotstart = new Dictionary<string, bool>();

// List to hold bot tasks
List<Task> botTasks = new List<Task>();

// Start a bot for each token
// foreach (string token in botTokens)
// {
//     botTasks.Add(StartBotAsync(token));
// }
int i = 0;
while (true)
{
    if (!isbotstart.Any(x => x.Key == botTokens[i]))
    {
        isbotstart.Add(botTokens[i], false);
    }
    if (isbotstart.FirstOrDefault(x => x.Key == botTokens[i]).Value == false)
    {
        botTasks.Add(StartBotAsync(botTokens[i]));
        isbotstart[botTokens[i]] = true;
    }
    i++;
    if (i == botTokens.Count)
    {
        i = 0;
    }
    Thread.Sleep(3000);
}


async Task StartBotAsync(string token)
{
    var botClient = new TelegramBotClient(token);
    var receiverOption = new ReceiverOptions
    {
        AllowedUpdates = { }
    };

    botClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOption);
    var me = await botClient.GetMeAsync();
    // Keep the bot running
    await Task.Delay(-1);
}

static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    var me = await botClient.GetMeAsync();
    System.Console.WriteLine(me.Id);
    // if(update.Type != UpdateType.Message){
    //     return;
    // }
    // if(update.Message!.Type != MessageType.Text)
    //     return;
    var udpatedsd = update;
    if (update.Message == null)
    {
        return;
    }

    if (!string.IsNullOrEmpty(update.Message.Text))
    {
        if (update.Message.Text.ToLower() != "hello" || update.Message.Text.ToLower() != "/start")
        {
            var sendMessage = await botClient.SendTextMessageAsync(update.Message.Chat.Id, $"You said : \n```\n{update.Message.Text}\n```",parseMode: ParseMode.MarkdownV2, cancellationToken: cancellationToken);
        }
    }
}

static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}