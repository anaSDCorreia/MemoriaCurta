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
                            string.Format("Ola eu sou Memoria Curta Bot e estou aqui para a ajudar a encontrar citações de personalidades.\n " +
                            "Para realizar uma procura basta seguir o seguinte formato:  =<query> \n Examples:" +
                            "= ImaginationOverflow \n" +
                            "= António Costa \n" +
                            "= Cristiano Ronaldo.\n" +
                            "Se quiser ver outra vez esta mensagem basta inseir o comando /ajuda .\n" +
                            "Powered by arquivo.pt!"));
            }

            if (command.Contains("="))
            {
                try
                {
                    await _telegramBot.Client.SendTextMessageAsync(_message.Chat.Id,
                            string.Format("For recebido o pedido de pesquisa. Vamos agora analiza-lo. Esta operação pode levar alguns minutos. "));
                    var responses = await _mcService.ProcessClassifierQuotes(command);

                    await _telegramBot.Client.SendTextMessageAsync(_message.Chat.Id,
                           string.Format("Foi encontrado {0} citações do {1} em {2} noticias ", responses.Sum(c => c.Quotes.Count), command.Substring(1), responses.Count));

                    foreach (var quote in responses)
                    {
                        System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                        var date = dtDateTime.AddSeconds(Int64.Parse(quote.News.date)).ToLocalTime();
                     
                        var str = quote.Quotes.Select(q => $"{q}" + " " + command.Substring(1) + " em " + date.ToString("MM/dd/yyyy")).Aggregate((s1, s2) => $"{ s1}\n\n{s2}");

                        await _telegramBot.Client.SendTextMessageAsync(_message.Chat.Id,
                            string.Format("Quotes:\n\n{1}\n\nLink to reference: {0}", quote.News.linkToArchive, str));

                    }

                    await _telegramBot.Client.SendTextMessageAsync(_message.Chat.Id,
                            string.Format("Showed {0} quotes in {1} news", responses.Sum(c => c.Quotes.Count), responses.Count));
                 
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
