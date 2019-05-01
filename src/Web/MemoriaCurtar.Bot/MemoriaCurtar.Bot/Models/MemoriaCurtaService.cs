using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MemoriaCurtar.Bot.Models
{
    public class MemoriaCurtaService
    {
        private const string HelpString = @"
Hi!
Welcome to Memoria Curta! Powered by arquivo.pt!
Usage:
    =<query> 

Examples:
=ImaginationOverflow
=António Costa
=Cristiano Ronaldo
";

        public void EnsureQuery(string text)
        {
            if (text.StartsWith("=") == false)
                throw new Exception(HelpString);
        }

        /// <summary>
        /// Processes requests starting with >
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public async Task<ICollection<NewsQuotes>> Process(string text)
        {

            var query = text.Substring(1);

            var resp = await new ArquivoAgent().Search(query, 20, sites: NewsSites);

            var tasks = resp.response_items.Select(item => Task.Run(() => DumbQuotes(item, text.Split(" "))));

            await Task.WhenAll(tasks);

            return tasks.Select(t =>
            {
                try
                {
                    return t.Result;
                }
                catch (Exception e)
                {
                    return null;
                }
            }).Where(t => t?.Quotes != null && t.Quotes.Count != 0).ToList();
        }

        public class NewsQuotes
        {
            public ResponseItem News { get; set; }
            public ICollection<string> Quotes { get; set; }
        }

        #region DUMB QUOTES

        private async Task<NewsQuotes> DumbQuotes(ResponseItem item, string[] keywords)
        {
            var extractedText = await GetString(item.linkToExtractedText);

            var quotes = ExtractQuote(extractedText, item, keywords);

            return new NewsQuotes
            {
                News = item,
                Quotes = quotes
            };
        }

        static Task<string> GetString(string url)
        {
            return new HttpClient().GetStringAsync(url);
        }

        private static ICollection<string> ExtractQuote(string text, ResponseItem item, string[] keywords)
        {
            var ret = new List<string>();

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
                    ret.Add(quote.Quote);
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

        #endregion




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
}
