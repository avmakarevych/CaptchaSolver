using System;
using System.Threading.Tasks;
using CaptchaSolver.Utilities;
using Telegram.Bot;

namespace CaptchaSolver.Services
{
    public class TelegramService
    {
        private readonly TelegramBotClient _botClient;
        private readonly TelegramBotClient _botProfileClient;
        private readonly string _chatId;
        private readonly Logger _logger;

        public TelegramService(string botToken, string botProfileToken, string chatId)
        {
            if (string.IsNullOrEmpty(botToken))
                throw new ArgumentException("Bot token cannot be null or empty.", nameof(botToken));
            if (string.IsNullOrEmpty(botProfileToken))
                throw new ArgumentException("Profile bot token cannot be null or empty.", nameof(botProfileToken));
            if (string.IsNullOrEmpty(chatId))
                throw new ArgumentException("Chat ID cannot be null or empty.", nameof(chatId));

            _botClient = new TelegramBotClient(botToken);
            _botProfileClient = new TelegramBotClient(botProfileToken);
            _chatId = chatId;
        }

        public async Task SendAsync(string message)
        {
            try
            {
                await _botClient.SendTextMessageAsync(_chatId, message);
            }
            catch (Exception ex)
            {
                _logger.PrintError(ex.Message);
            }
        }

        public async Task SendProfileAsync(string message)
        {
            try
            {
                await _botProfileClient.SendTextMessageAsync(_chatId, message);
            }
            catch (Exception ex)
            {
                _logger.PrintError(ex.Message);
            }
        }
    }
}