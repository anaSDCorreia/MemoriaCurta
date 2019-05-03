using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MemoriaCurtaApi.Services;
using MemoriaCurtaAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace MemoriaCurtaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MCExtractQuotesController : ControllerBase
    {

        private readonly IClassifierService _quoteExtractService;

        private readonly ArquivoQuotesBotService _arquivoQuotesService;

        public MCExtractQuotesController(ArquivoQuotesBotService s, IClassifierService cl)
        {
            _quoteExtractService = cl;
            _arquivoQuotesService = s;
    }

        /* [HttpPost]
         public async Task<IActionResult> Post([FromBody]Update update)
         {
             var date = DateTime.Now - update.Message.Date;

             if (date.TotalMinutes < 70)
             {
                 await _arquivoQuotesService.ReceiveMessageTelegram(update);
             }

             return Ok();
         }*/

        static int id = 0;
        [HttpPost]
        public async Task Post([FromBody]Update update)
        {
            try {

                if (update == null || update.Message == null)
                    return;

                if (update.Message.Date != null)
                {
                    var date = DateTime.UtcNow - update.Message.Date;

                    if (id == update.Id)
                        return;

                    id = update.Id;
                    if (date.TotalSeconds < 5)
                    {
                        Debug.WriteLine(" Message Text= " + update.Message.Text + " Chat Id= " + update.Message.Chat.Id);
                        await _arquivoQuotesService.ReceiveMessageTelegram(update);
                    }
                }
                else {
                    Debug.WriteLine(" Message Text= " + update.Message.Text + " Chat Id= " + update.Message.Chat.Id);
                    await _arquivoQuotesService.ReceiveMessageTelegram(update);
                }
               

                


            }
            catch (Exception e){
                int y = 0;
            }
            
           
        }

        
       
    }
}
