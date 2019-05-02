using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MemoriaCurtar.Bot.Models;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;

namespace MemoriaCurtar.Bot.Controllers
{
    public class HomeController : Controller
    {
        private string fbToken = "mytoken";
        private string postUrl = "https://graph.facebook.com/v2.9/{0}/messages";
        private const string facebookurl = "https://graph.facebook.com/v2.9/";
        private string ConversationMessages = facebookurl + "{0}?fields=messages.fields(message,from)";
        private const string PageId = "368312317119504";
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public string Webhook(
            [FromQuery(Name = "hub.mode")] string mode,
            [FromQuery(Name = "hub.challenge")] string challenge,
            [FromQuery(Name = "hub.verify_token")] string verify_token)
        {
            if (verify_token.Equals("my_token_is_great"))
            {
                return challenge;
            }
            else
            {
                return "";
            }
        }

        [HttpGet]
        public async Task<JsonResult> Search(string query)
        {
            var ser = new MemoriaCurtaService();
            var res  = await ser.Process(" " + query);
            return Json(res);
        }

        [HttpPost]
        public void Webhook()
        {
            var json = (dynamic)null;
            try
            {
                using (StreamReader sr = new StreamReader(this.Request.Body))
                {
                    json = sr.ReadToEnd();
                }

                Task.Run(async () =>
                {
                    dynamic data = JsonConvert.DeserializeObject(json);
                    BotRequest req = JsonConvert.DeserializeObject<BotRequest>(json);


                    if (req?.entry?[0].messaging != null)
                        PostToFb((string)data.entry[0].messaging[0].sender.id, (string)data.entry[0].messaging[0].message.text);
                    else
                    {
                        var convId = (string)data.entry[0].changes[0].value.thread_id;
                        //read conversation
                        var messagesJson = await Get(string.Format(ConversationMessages, convId));
                        dynamic result = JsonConvert.DeserializeObject(messagesJson);

                        if ((string)(result.messages.data[0].from.id) == PageId)
                            return;

                        var lastMsg = (string)(result.messages.data[0].message);

                        await PostToFb(convId, lastMsg);

                    }
                });
            }
            catch (Exception ex)
            {
                return;
            }
        }

        public async Task PostToFb(string recipientId, string messageText)
        {
            var ser = new MemoriaCurtaService();

            try
            {
                ser.EnsureQuery(messageText);
                SendMessageToFacebook(recipientId, "Processing " + messageText);
            }
            catch (Exception e)
            {
                await SendMessageToFacebook(recipientId, e.Message);
                return;
            }


            try
            {
                var responses = await ser.Process(messageText);

                foreach (var quote in responses)
                {
                    var str = quote.Quotes.Select(q=>$"\"{q}\"").Aggregate((s1, s2) => $"{s1}\n\n{s2}");

                    await SendMessageToFacebook(recipientId,
                        string.Format("Quotes:\n\n{1}\n\nLink to reference: {0}", quote.News.originalURL, str));
                }
                await SendMessageToFacebook(recipientId, string.Format("Showed {0} quotes in {1} news", responses.Sum(c => c.Quotes.Count), responses.Count));
            }
            catch (Exception e)
            {
                await SendMessageToFacebook(recipientId, e.Message);
            }

        }

        private async Task SendMessageToFacebook(string recipientId, string messageText)
        {
            //Post to ApiAi
            string messageTextAnswer = messageText;
            string postParameters = string.Format("access_token={0}&message={1}", fbToken, messageTextAnswer);
            //Response from ApiAI or answer to FB question from user post it to   FB back.
            var client = new HttpClient();
            var res = await client.PostAsync(string.Format(postUrl, recipientId),
                new StringContent(postParameters, Encoding.UTF8, "application/json"));
            var resp = res.Content.ReadAsStringAsync();
        }

        public async Task<string> Get(string id)
        {
            //Response from ApiAI or answer to FB question from user post it to   FB back.
            var client = new HttpClient();
            string postParameters = $"access_token={fbToken}";
            var res = await client.GetAsync(id + "&" + postParameters);
            return await res.Content.ReadAsStringAsync();
        }
    }
}
