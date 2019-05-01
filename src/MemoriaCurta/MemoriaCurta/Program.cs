using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MemoriaCurta.Data;
using Newtonsoft.Json;

namespace MemoriaCurta
{
    class Program
    {
        private const int MinWordsToBeConsideredQuote = 4;

        static async Task Main(string[] args)
        {
            var agent = new ArquivoAgent();

            var query = "António Costa";

            Console.WriteLine("Insira uma pesquisa (e.g Antonio Costa)");
            query = Console.ReadLine();

            Console.WriteLine("Insira quantas noticias quer processar (e.g 10)");
            int maxNews = int.Parse(Console.ReadLine());


            var resp = await agent.Search(query, maxNews, NewsSites);

            Console.WriteLine(resp.total_items);
            foreach (var item in resp.response_items)
            {
                Console.WriteLine(item.title);
            }
            var keywords = query.Replace("\"", string.Empty).ToLower().Split(' ');

            var tasks = resp.response_items.Select(item => Task.Run(() =>
            {
                var extractedText = GetString(item.linkToExtractedText).Result;

                var quotes = ExtractQuote(extractedText, item, keywords);
                return quotes;
            }));

            await Task.WhenAll(tasks);

            foreach (var t in tasks)
            {
                var res = await t;

                foreach (var arquivoQuote in res.Where(r => r.Quote.Split(' ').Length >= MinWordsToBeConsideredQuote))
                {
                    Console.WriteLine(arquivoQuote.Quote);
                    Console.WriteLine("Ficheiro Fonte - " + arquivoQuote.Reference.linkToExtractedText);
                    Console.WriteLine();
                }
            }


        }

        private static ICollection<ArquivoQuote> ExtractQuote(string text, ResponseItem item, string[] keywords)
        {
            var ret = new List<ArquivoQuote>();

            var splits = text.Split('"');

            if (splits.Length == 1)
                return ret;

            for (int i = 1; i < splits.Length; i += 2)
            {
                var quote = new ArquivoQuote
                {
                    FullText = text,
                    Quote = splits[i],
                    Reference = item
                };

                SetQuoteContext(splits, i, quote);


                if (EvaluateQuote(quote, keywords))
                    ret.Add(quote);
            }

            return ret.Distinct().ToList();
        }

        private static bool EvaluateQuote(ArquivoQuote quote, string[] keywords)
        {
            var phraseStartIdx = quote.PrevContext.LastIndexOf(".", StringComparison.InvariantCulture);

            if (phraseStartIdx != -1)
            {
                quote.PrevContext = quote.PrevContext.Substring(phraseStartIdx,
                    quote.PrevContext.Length - phraseStartIdx).ToLower();
            }

            phraseStartIdx = quote.PosContext.IndexOf(".", StringComparison.InvariantCulture);

            if (phraseStartIdx != -1)
            {
                quote.PosContext = quote.PosContext.Substring(phraseStartIdx,
                    quote.PosContext.Length - phraseStartIdx).ToLower();
            }


            return keywords.Any(k => quote.PrevContext.Contains(k) || quote.PosContext.Contains(k));

        }



        private static void SetQuoteContext(string[] splits, int i, ArquivoQuote q)
        {
            q.PrevContext = q.PosContext = string.Empty;

            if (splits.Length == 0)
                return;

            if (i > 0)
                q.PrevContext = splits[i - 1];

            if (i + 1 < splits.Length)
                q.PosContext += splits[i + 1];

        }

        static Task<string> GetString(string url)
        {
            return new HttpClient().GetStringAsync(url);
        }

        private static string[] NewsSites =
        {
            "http://acervo.publico.pt/",
            "http://inimigo.publico.pt/",
            "http://publico.pt/",
            "http://www.dn.pt/",
            "http://dn.sapo.pt/",
            "http://dnoticias.pt/",
            "http://www.rtp.pt/",
            "http://www.cmjornal.pt/",
            "http://www.iol.pt/",
            "http://www.tvi24.iol.pt/",
            "http://noticias.sapo.pt/",
            "http://www.sapo.pt/",
            "http://expresso.sapo.pt/",
            "http://sol.sapo.pt/",
            "http://visao.sapo.pt/",
            "http://exameinformatica.sapo.pt/",
            "http://tek.sapo.pt/",
            "http://www.jornaldenegocios.pt/",
            "http://dinheirodigital.sapo.pt/",
            "http://abola.pt/",
            "http://www.abola.pt/",
            "http://www.jn.pt/",
            "http://jn.pt/",
            "http://sicnoticias.sapo.pt/",
            "http://www.lux.iol.pt/",
            "http://maisfutebol.iol.pt/",
            "http://lux.iol.pt/",
            "http://www.ionline.pt/",
            "http://ionline.sapo.pt/",
            "http://news.google.pt/",
            "http://www.dinheirovivo.pt/",
            "http://www.aeiou.pt/",
            "http://zap.aeiou.pt/",
            "http://www.tsf.pt/",
            "http://meiosepublicidade.pt/",
            "http://www.sabado.pt/",
            "http://www.omirante.pt/",
            "http://www.jb.pt/",
            "http://www.mdb.pt/",
            "http://www.avante.pt/",
            "http://www.oje.pt/",
            "http://www.auniao.pt/",
            "http://www.record.pt/",
            "http://www.ojogo.pt/",
            "http://zerozero.pt/",
            "http://maisfutebol.iol.pt/",
            "http://desporto.sapo.pt/",
            "http://jornaleconomico.sapo.pt/",
            "http://www.diarioleiria.pt/",
            "http://www.regiaodeleiria.pt/",
            "http://www.correiodominho.pt/",
            "http://www.diariodominho.pt/",
            "http://economico.sapo.pt/"
        };
    }


    public class ArquivoQuote
    {
        public int CompareTo(ArquivoQuote other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return string.Compare(Quote, other.Quote, StringComparison.Ordinal);
        }

        protected bool Equals(ArquivoQuote other)
        {
            return string.Equals(Quote, other.Quote);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ArquivoQuote)obj);
        }

        public override int GetHashCode()
        {
            return (Quote != null ? Quote.GetHashCode() : 0);
        }

        public static bool operator ==(ArquivoQuote left, ArquivoQuote right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ArquivoQuote left, ArquivoQuote right)
        {
            return !Equals(left, right);
        }

        public string Quote { get; set; }
        public ResponseItem Reference { get; set; }
        public string FullText { get; set; }

        public string PrevContext { get; set; }
        public string PosContext { get; set; }
    }

    public class ArquivoAgent
    {
        public async Task<TextSearchResponse> Search(string query, int maxNews = 10, params string[] sites)
        {
            const string ApiFormat = @"https://arquivo.pt/textsearch?q={0}&maxItems={1}&siteSearch={2}";

            var finalReqUrl = string.Format(ApiFormat, query, maxNews, sites.Length == 0 ? "" : sites.Aggregate((r, n) => r + "," + n));
            var cli = new HttpClient();
            var resp = await cli.GetStringAsync(finalReqUrl);

            return JsonConvert.DeserializeObject<TextSearchResponse>(resp);
        }

       
    }
}
