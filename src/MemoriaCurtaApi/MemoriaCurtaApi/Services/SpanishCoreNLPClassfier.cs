using edu.stanford.nlp.pipeline;
using java.nio.file;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.IO;
using java.util;
using java.io;
using edu.stanford.nlp.pipeline;
using Console = System.Console;
using System.Globalization;
using System.Threading;
using edu.stanford.nlp.util;
using edu.stanford.nlp.ling;
using MemoriaCurtaAPI.Data.Exceptions;
using MemoriaCurtaAPI.Data;
using System;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace MemoriaCurtaAPI.Services
{
    public class SpanishCoreNLPClassfier : IClassifierService
    {

        private StanfordCoreNLP _pipeline;

        private string _modelPath;

        public async Task<List<MCQuote>> GetQuotes(string data)
        {
            List<MCQuote> quotes = new List<MCQuote>();

            try
            {
                var curDir = Environment.CurrentDirectory;
                Directory.SetCurrentDirectory(_modelPath);

                CoreDocument doc = new CoreDocument(data);

                _pipeline.annotate(doc);

                for (int i = 0; i < doc.quotes().size(); i++)
                {
                    CoreQuote q = (CoreQuote)doc.quotes().get(i);
                    quotes.Add(new MCQuote(q));
                }

                Directory.SetCurrentDirectory(curDir);
            }
            catch (Exception e)
            {
                throw new SpanishCoreNLPQuoteException(e.Message, e);
            }
            return quotes;
        }

        public async Task Inicializer()
        {
            string BaseDirectory = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\"));

            string MODEL_PATH_DEFAULT = System.IO.Path.GetFullPath(BaseDirectory + "\\Resources\\es\\");

            _modelPath = MODEL_PATH_DEFAULT;
            SetupCoreNLP();
        }

        public async Task Inicializer(string modelPath)
        {
            _modelPath = modelPath;

            SetupCoreNLP();
        }

        #region

        private void SetupCoreNLP()
        {
            try
            {
                var propsFile = System.IO.Path.Combine(_modelPath, "StanfordCoreNLP-spanish.properties");

                // Annotation pipeline configuration

                var props = new Properties();

                props.load(new FileReader(propsFile));
                props.put("ner.useSUTime", "0");
                props.put("tokenize.verbose", "true");
                props.setProperty("annotators", "tokenize, ssplit, pos, lemma, ner, depparse, kbp, coref,entitymentions, quote");

                var curDir = Environment.CurrentDirectory;

                Directory.SetCurrentDirectory(_modelPath);

                _pipeline = new StanfordCoreNLP(props);

                Directory.SetCurrentDirectory(curDir);
            }
            catch (Exception e)
            {
                throw new SpanishCoreNLPSetupException(e.Message, e);
            }
           
        }

        #endregion
    }
}
