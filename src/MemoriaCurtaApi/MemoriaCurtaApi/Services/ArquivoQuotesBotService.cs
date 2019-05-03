 using MemoriaCurtaApi.Services.Bots;
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

        private MCTelegramBot _telegramBot;

        public ArquivoQuotesBotService(MCTelegramBot bot)
        {
            _telegramBot = bot;
        }

        public async Task ReceiveMessageTelegram(Update update)
        {
            if (update.Type != UpdateType.Message)
            {
                return;
            }

            var message = update.Message;

          
            if (message.Type == MessageType.Text)
            {
                // Echo each Message
                await _telegramBot.Client.SendTextMessageAsync(message.Chat.Id, message.Text);
            }
            else if (message.Type == MessageType.Photo)
            {
                // Download Photo
                var fileId = message.Photo.LastOrDefault()?.FileId;
                var file = await _telegramBot.Client.GetFileAsync(fileId);

                var filename = file.FileId + "." + file.FilePath.Split('.').Last();

                using (var saveImageStream = System.IO.File.Open(filename, FileMode.Create))
                {
                    await _telegramBot.Client.DownloadFileAsync(file.FilePath, saveImageStream);
                }

                await _telegramBot.Client.SendTextMessageAsync(message.Chat.Id, "Thx for the Pics");
            }
        }
    


        #region Private Methods

        private void ProcessCommand(string command)
        {
            if (command.StartsWith("="))
            {
               // ProcessQueryCommand(command.Remove(0));
            }
            

        }

        #endregion
    }
}
