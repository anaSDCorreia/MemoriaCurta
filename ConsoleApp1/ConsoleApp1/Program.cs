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

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var jarRoot = @"..\..\..\es\";

           /* CultureInfo ci = new CultureInfo("es-ES");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;*/
            // Text for processing
            var text = "\"O pior erro que poderíamos cometer era, à boleia do crescimento económico, termos a ilusão de que os problemas estruturais da zona euro ficaram resolvidos\", defende António Costa, que deu uma entrevista ao Público que será publicada este domingo e onde o primeiro-ministro diz que a moeda única foi um bonus para a economia alemã.";


            string sb = "António Costa defende que, na criação da moeda única, houve um \"excesso de voluntarismo político\" e nem todos terão percebido que \"o euro foi o maior bónus à competitividade da economia alemã que a Europa lhe poderia ter oferecido\".";

            var propsFile = Path.Combine(jarRoot, "StanfordCoreNLP-spanish.properties");
            // Annotation pipeline configuration
            var props = new Properties();
           // props.setProperty("annotators", "tokenize, ssplit, pos, lemma, ner, depparse, kbp");
            props.load(new FileReader(propsFile));
            props.put("ner.useSUTime", "0");
            props.put("tokenize.verbose", "true");
           // props.setProperty("annotators", "tokenize, ssplit, pos, lemma, ner, depparse, kbp, coref,entitymentions,quote1");
            props.setProperty("annotators", "tokenize, ssplit, pos, lemma, ner, depparse, kbp, coref,entitymentions, quote"); 
            //props.setProperty("customAnnotatorClass.quote1", "edu.stanford.nlp.pipeline.QuoteAnnotator");
            //props.setProperty("quote1.attributeQuotes", "false");

            /*String modPath = @"C:\Users\anacorreia\source\repos\ConsoleApp1\es\";
            props.put("pos.model", modPath + @"edu\stanford\nlp\models\pos-tagger\spanish\spanish-ud.tagger");
            props.put("tokenize.language","es");

            props.put("ner.model", modPath + @"edu\stanford\nlp\models\ner\spanish.ancora.distsim.s512.crf.ser.gz");
            props.put("ner.applyNumericClassifiers", "1");
            
            props.put("ner.language", "1");
            props.put("sutime.language", "1");

            props.put("parse.model", modPath + @"edu\stanford\nlp\models\lexparser\spanishPCFG.ser.gz");
            props.put("depparse.model", modPath + @"edu\stanford\nlp\models\parser\nndep\UD_Spanish.gz");
            props.put("depparse.language", "spanish");

            props.put("ner.fine.regexner.mapping", modPath + @"edu\stanford\nlp\models\kbp\spanish\gazetteers\");
            props.put("ner.fine.regexner.validpospattern", "^(NOUN|ADJ|PROPN).*");
            props.put("ner.fine.regexner.ignorecase", "1");
            props.put("ner.fine.regexner.noDefaultOverwriteLabels", "CITY,COUNTRY,STATE_OR_PROVINCE");

            props.put("kbp.semgrex", modPath + @"edu\stanford\nlp\models\kbp\spanish\semgrex");
            props.put("kbp.tokensregex", modPath + @"edu\stanford\nlp\models\kbp\spanish\tokensregex");
            props.put("kbp.model", "none");
            props.put("kbp.language", "es");

            props.put("entitylink.caseless", "1");
            props.put("entitylink.wikidict", modPath + @"edu\stanford\nlp\models\kbp\spanish\wikidict_spanish.tsv");
          */

            // We should change current directory, so StanfordCoreNLP could find all the model files automatically
            var curDir = Environment.CurrentDirectory;
            Directory.SetCurrentDirectory(jarRoot);
            var pipeline = new StanfordCoreNLP(props);
            

            // Annotation

            CoreDocument testDoc = new CoreDocument(sb);
            pipeline.annotate(testDoc);
           
            // var annotation = new Annotation(sb);
            //pipeline.annotate(annotation);

            string sxc = "\"O pior erro que poderíamos cometer era, à boleia do crescimento económico, termos a ilusão de que os problemas estruturais da zona euro ficaram resolvidos\", defende António Costa, que deu uma entrevista ao Público que será publicada este domingo e onde o primeiro-ministro diz que a moeda única foi um \"bónus\" para a economia alemã. Numa entrevista em que, a julgar pelo excerto publicado para já, se focou essencialmente em questões europeias, e não na política interna, o primeiro-ministro mostrou-se, também, favorável à introdução de impostos europeus." +
"António Costa defende que, na criação da moeda única, houve um \"excesso de voluntarismo político\" e nem todos terão percebido que \"o euro foi o maior bónus à competitividade da economia alemã que a Europa lhe poderia ter oferecido\". Agora, a menos que se tomem medidas de correção das assimetrias, \"a zona euro será mais uma vez confrontada com uma crise como a que vivemos agora\"." +
"O objetivo de todos os líderes europeus deve ser evitar que se volte a cometer \"o erro que nos acompanhou desde 2000 até 2011\", isto é, marcar passo nas reformas. E uma das principais ferramentas de que a zona euro necessita é de um orçamento da zona euro destinado a financiar reformas para acelerar a convergência das economias." +
"Com a saída do Reino Unido e com a necessidade de investir na segurança, na defesa e na ciência, António Costa defende que \"ou estamos disponíveis para sacrificar a parte do Orçamento afeta às políticas de coesão e à PAC, ou temos de encontrar outras fontes de receita\". Onde? Mais contribuições dos Estados, isto é, \"mais impostos dos portugueses\", ou receitas próprias criadas pela União, nomeadamente através de impostos europeus.";

            CoreDocument testDoc3 = new CoreDocument(sxc);
            pipeline.annotate(testDoc3);


            // Result - Pretty Print
            using (var stream = new ByteArrayOutputStream())
             {
                 pipeline.prettyPrint(testDoc.annotation(), new PrintWriter(stream));
                 Console.WriteLine("Stream testDoc3 " + stream.toString());
                 stream.close();
             }

            using (var stream = new ByteArrayOutputStream())
            {
                pipeline.prettyPrint(testDoc3.annotation(), new PrintWriter(stream));
                Console.WriteLine("Stream testDoc3 " + stream.toString());
                stream.close();
            }



            Directory.SetCurrentDirectory(curDir);
        }
}
}
