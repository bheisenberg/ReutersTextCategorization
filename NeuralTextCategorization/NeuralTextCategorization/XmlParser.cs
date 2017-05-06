using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Xml.Linq;
using System.Xml;

namespace NeuralTextCategorization
{
    public class XmlParser
    {
        private List<string> totalWords;
        private List<string> uniqueTopics;
        private List<string> topWords;
        private int numWords = 250;
        private string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TextCategorization");
        private string input = "input.txt";
        private string output = "output.txt";
        private string topics = "topics.txt";
        private string inputFile;
        private string outputFile;
        private string topicsFile;
        private string[] files = Directory.GetFiles("Resources", "*.sgm");

        public XmlParser()
        {
            inputFile = Path.Combine(path, input);
            outputFile = Path.Combine(path, output);
            topicsFile = Path.Combine(path, topics);
        }

        public NeuralData GetNeuralData()
        {
            if (!File.Exists(inputFile) || !File.Exists(outputFile) || !File.Exists(topicsFile))
            {
                this.totalWords = new List<string>();
                this.uniqueTopics = new List<string>();
                Debug.WriteLine("PARSING");
                List<RawArticle> articles = GetArticles(files);
                topWords = GetTopWords(totalWords);
                return GetArticleVectors(articles);
            } else
            {
                return new VectorReader(this.inputFile, this.outputFile, this.topicsFile).CreateVectors();
            }
        }

        private NeuralData GetArticleVectors(List<RawArticle> rawArticles)
        {
            double[][] input = new double[rawArticles.Count][];
            double[][] output = new double[rawArticles.Count][];
            for (int i=0; i < rawArticles.Count; i++)
            {
                double[] wordVector = CreateVector(topWords, rawArticles[i].words);
                double[] topicVector = CreateVector(uniqueTopics, rawArticles[i].topics);
                input[i] = wordVector;
                output[i] = topicVector;
                
                //PrintVectors(wordVector, topicVector);

            }
            WriteVectors(input, output);
            NeuralData neuralData = new NeuralData(input, output, uniqueTopics.ToArray());
            return neuralData;
        }

        private void WriteVectors (double[][] input, double[][] output)
        {
            Debug.WriteLine("WRITING INPUT VECTORS TO FILE");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            if (!File.Exists(inputFile)) File.Create(inputFile).Close();
            if (!File.Exists(outputFile)) File.Create(outputFile).Close();
            if (!File.Exists(topicsFile)) File.Create(topicsFile).Close();
            using (StreamWriter iw = new StreamWriter(inputFile, true, Encoding.UTF8))
            {
                for (int i = 0; i < input.Length; i++)
                {
                    string result = string.Join(" ", input[i].Select(Convert.ToInt32).ToArray());
                    iw.WriteLine(result);
                }
            }
            using (StreamWriter ow = new StreamWriter(outputFile, true, Encoding.UTF8))
            {
                for (int i = 0; i < output.Length; i++)
                {
                    string result = string.Join(" ", output[i].Select(Convert.ToInt32).ToArray());
                    ow.WriteLine(result);
                }
            }
            using (StreamWriter tw = new StreamWriter(topicsFile, true, Encoding.UTF8))
            {
                for (int i = 0; i < uniqueTopics.Count; i++)
                {
                    tw.WriteLine(uniqueTopics[i]);
                }
            }
        }

        private double[] CreateVector(List<string> classifier, List<string> data)
        {
            double[] vector = new double[classifier.Count];
            for (int i = 0; i < classifier.Count; i++)
            {
                if (data.Contains(classifier[i]))
                {
                    vector[i] = 1;
                }
                else
                {
                    vector[i] = 0;
                }
            }
            return vector;
        }

        private List<string> GetTopWords(List<string> words)
        {
            List<string> topWords = new List<string>();
            List<string> filteredWords = new List<string> { "the", "and", "if", "then", "a", "be", "to", "of", "in", "that", "have", "i", "it", "for", "not", "on", "with", "he", "as", "you", "do", "at", "this", "but", "by", "is", "from", "reuter", "reuters", "", "-" };
            Dictionary<string, int> wordDict = new Dictionary<string, int>();
            foreach (string word in words)
            {
                if (!filteredWords.Contains(word))
                {
                    if (!wordDict.Keys.Contains(word))
                    {
                        wordDict.Add(word, 0);
                    }
                    wordDict[word] += 1;
                }
            }
            var wordsOrdered = (from entry in wordDict orderby entry.Value descending select entry).ToList();
            for (int i = 0; i < numWords; i++)
            {
                Debug.WriteLine(string.Format("{0}: {1}: {2}", i, wordsOrdered[i].Key, wordsOrdered[i].Value));
                topWords.Add(wordsOrdered[i].Key);
            }
            return topWords;
        }

        private List<string> GetTopics(XElement article)
        {
            List<string> topics = new List<string>();
            var topicData = from element in article.Elements()
                            where element.Name == "TOPICS"
                            select element;
            foreach (XElement topicHolder in topicData)
            {
                foreach (string topic in topicHolder.Elements())
                {
                    if (topic != "")
                    {
                        topics.Add(topic);
                        if(!uniqueTopics.Contains(topic))
                        {
                            uniqueTopics.Add(topic);
                        }
                    }
                }
            }
            return topics;
        }

        private XDocument CreateXDoc(string input)
        {
            var text = String.Join("\n", File.ReadAllLines(input).Skip(1));
            var rootedXML = "<root>" + text + "</root>";
            XDocument xdoc = null;
            XmlReaderSettings settings = new XmlReaderSettings
            {
                CheckCharacters = false,
                XmlResolver = null
            };
            using (XmlReader xmlReader = XmlReader.Create(new StringReader(rootedXML), settings))
            {
                xdoc = XDocument.Load(xmlReader);
            }
            return xdoc;
        }

        private List<RawArticle> GetArticles(string[] inputFiles)
        {
            Debug.WriteLine("Finding articles...");
            List<RawArticle> articles = new List<RawArticle>();
            List<XElement> articleElements = new List<XElement>();
            foreach (string input in inputFiles)
            {
                XDocument xdoc = CreateXDoc(input);
                foreach (var article in xdoc.Elements().Elements())
                {
                    List<string> topics = GetTopics(article);
                    List<string> words = GetWords(article);
                    articles.Add(new RawArticle(words, topics));
                }

            }
            return articles;
        }

        private List<string> GetWords(XElement articleElement)
        {
            List<string> words = new List<string>();
            var bodyData = from element in articleElement.Elements().Elements()
                           where element.Name == "BODY"
                           select element;
            if (bodyData.Count() > 0)
            {
                string body = bodyData.First().Value.ToLower();
                List<string> articleWords = body.Split().ToList();
                foreach (string word in articleWords)
                {
                    if (word != "" && word != " ")
                    {
                        string newWord = word.Replace("+", "").Replace(".", "").Replace(",", "");
                        words.Add(newWord);
                        totalWords.Add(newWord);
                    }
                }
            }
            return words;
        }

        private void PrintVectors(double[] input, double[] output)
        {
            string inputPrint = "input: < ";
            for (int j = 0; j < input.Length; j++)
            {
                inputPrint += input[j] + ", ";
            }
            inputPrint += ">";
            Debug.WriteLine(inputPrint);
            string outputPrint = "output: < ";
            for (int k = 0; k < output.Length; k++)
            {
                outputPrint += output[k] + ", ";
            }
            outputPrint += ">";
            Debug.WriteLine(outputPrint);
        }

    }
}