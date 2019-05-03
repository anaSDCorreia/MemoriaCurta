using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MihaZupan;
using Telegram.Bot;

namespace MemoriaCurtaApi.Services.Bots
{
    public class MCTelegramBot : IMCBot
    {

        private readonly MCTelegramBotConfiguration _config;

        public MCTelegramBot(IOptions<MCTelegramBotConfiguration> config)
        {
            _config = config.Value;
            // use proxy if configured in appsettings.*.json
            Client = string.IsNullOrEmpty(_config.Socks5Host)
                ? new TelegramBotClient(_config.BotToken)
                : new TelegramBotClient(
                    _config.BotToken,
                    new HttpToSocks5Proxy(_config.Socks5Host, _config.Socks5Port));
        }

        public TelegramBotClient Client { get; }
    }
}
