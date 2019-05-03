using edu.stanford.nlp.pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MemoriaCurtaAPI.Data
{
    public class MCQuote
    {
        public string Quote { get; set; }

        public string Speaker { get; set; }

        public string CanonicalSpeaker { get; set; }

        public string Source { get; set; }

        public DateTime Date { get; set; }

        public string Subject { get; set; }


        public MCQuote(CoreQuote q)
        {
            Quote = q.text();

            Speaker = q.speaker()!= null ? q.speaker().get().ToString(): "";

        }
    }
}
