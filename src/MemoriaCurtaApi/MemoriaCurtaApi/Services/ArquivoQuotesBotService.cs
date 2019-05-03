 using MemoriaCurtaApi.Services.Bots;
using MemoriaCurtar.Bot.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MemoriaCurtaApi.Services
{
    public class ArquivoQuotesBotService
    {
        private string _query;

        private string _context;

        private Message _message;

        private MCTelegramBot _telegramBot;

        private MemoriaCurtaService _mcService;

        public ArquivoQuotesBotService(MCTelegramBot bot, MemoriaCurtaService mcService)
        {
            _telegramBot = bot;

            _mcService = mcService;
        }

        public async Task ReceiveMessageTelegram(Update update)
        {
              if (update.Type != UpdateType.Message)
            {
                return;
            }

            _message = update.Message;

          
            if (_message.Type == MessageType.Text)
            {
                // Echo each Message
                await ProcessCommand(_message.Text);
            }

        }

        #region Private Methods
 
        private async Task ProcessCommand(string command)
        {
            if ((command == "/start") || (command == "/ajuda"))
            {
                await _telegramBot.Client.SendTextMessageAsync(_message.From.Id,
                            string.Format("Olá eu sou o Bot Memória Curta e estou aqui para ajudar a recordar citações de personalidades!\n " +
                            "Diga-me quem procurar enviando uma mensagem da forma: = <nome da personalidade>\n" +
                            "Por exemplo: = António Costa\n" +
                            "Se quiser ver outra vez esta mensagem basta enviar o comando /ajuda.\n" +
                            "Powered by arquivo.pt!"));
            }

            if (command.Contains("="))
            {
                try
                {
                    await _telegramBot.Client.SendTextMessageAsync(_message.Chat.Id,
                            string.Format("A procurar citações... Posso demorar alguns minutos!"));
                    var responses = await _mcService.ProcessClassifierQuotes(command);

                    await _telegramBot.Client.SendTextMessageAsync(_message.Chat.Id,
                           string.Format("Encontrei {0} citações de {1} em {2} noticias!", responses.Sum(c => c.Quotes.Count), command.Substring(1), responses.Count));

                    foreach (var quote in responses)
                    {
                        System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                        var date = dtDateTime.AddSeconds(Int64.Parse(quote.News.date)).ToLocalTime();
                     
                        var str = quote.Quotes.Select(q => $"{q}" + ", " + command.Substring(1) + " em " + date.ToString("yyyy/MM/dd")).Aggregate((s1, s2) => $"{ s1}\n\n{s2}");

                        await _telegramBot.Client.SendTextMessageAsync(_message.Chat.Id,
                            string.Format("Citações:\n\n{1}\n\nFonte: {0}", quote.News.linkToArchive, str));

                    }

                    await _telegramBot.Client.SendTextMessageAsync(_message.Chat.Id,
                            string.Format("{0} citações em {1} notícias.", responses.Sum(c => c.Quotes.Count), responses.Count));
                 
                }
                catch (Exception e)
                {
                    await _telegramBot.Client.SendTextMessageAsync(_message.Chat.Id,
                            e.Message);
                }
            }
            
        }

        private void ProcessPessoa(string command)
        { 

        }

        #endregion
    }
}
