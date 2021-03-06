﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MemoriaCurtar.Bot.Models
{
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

    public class RequestParameters
    {
        public string q { get; set; }
        public string maxItems { get; set; }
        public string siteSearch { get; set; }
    }

    public class ResponseItem
    {
        public string title { get; set; }
        public string originalURL { get; set; }
        public string linkToArchive { get; set; }
        public string tstamp { get; set; }
        public string contentLength { get; set; }
        public string digest { get; set; }
        public string mimeType { get; set; }
        public string linkToScreenshot { get; set; }
        public string date { get; set; }
        public string encoding { get; set; }
        public string linkToNoFrame { get; set; }
        public string snippet { get; set; }
        public string collection { get; set; }
        public string linkToExtractedText { get; set; }
        public string linkToMetadata { get; set; }
        public string linkToOriginalFile { get; set; }
    }

    public class TextSearchResponse
    {
        public string serviceName { get; set; }
        public string linkToService { get; set; }
        public string next_page { get; set; }
        public string previous_page { get; set; }
        public string total_items { get; set; }
        public RequestParameters request_parameters { get; set; }
        public List<ResponseItem> response_items { get; set; }
    }
}
