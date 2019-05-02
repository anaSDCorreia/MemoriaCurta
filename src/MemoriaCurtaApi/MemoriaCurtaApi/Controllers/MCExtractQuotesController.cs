using System;
using System.Collections.Generic;
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

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Update update)
        {
            await _arquivoQuotesService.ReceiveMessageTelegram(update);
            return Ok();
        }

        // GET: api/MCExtractQuotes
        [HttpGet]
        public  IEnumerable<string> Get()
        {
           // string sb = "António Costa defende que, na criação da moeda única, houve um \"excesso de voluntarismo político\" e nem todos terão percebido que \"o euro foi o maior bónus à competitividade da economia alemã que a Europa lhe poderia ter oferecido\".";

            /*var quotes = await _quoteExtractService.GetQuotes(sb);

            var quot = quotes.Select((q) => q.Quote);

            return quot.ToArray();*/
            return new string[] { "asdsad", "asdsad"};
        }

       
    }
}
