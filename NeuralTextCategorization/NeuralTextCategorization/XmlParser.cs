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
        public List<string> totalWords;
        public List<string> uniqueTopics;
        public List<string> topWords;
        string[] files = Directory.GetFiles("Resources", "*.sgm");
        public XmlParser()
        {
        }

        public NeuralData GetNeuralData()
        {
            this.totalWords = new List<string>();
            this.uniqueTopics = new List<string>();
            Debug.WriteLine("PARSING");
            List<RawArticle> articles = GetArticles(files);
            topWords = GetTopWords(totalWords);
            return GetArticleVectors(articles);
        }

        private NeuralData GetArticleVectors(List<RawArticle> rawArticles)
        {
            int[][] input = new int[rawArticles.Count][];
            int[][] output = new int[rawArticles.Count][];
            for (int i=0; i < rawArticles.Count; i++)
            {
                int[] wordVector = CreateVector(topWords, rawArticles[i].words);
                int[] topicVector = CreateVector(uniqueTopics, rawArticles[i].topics);
                input[i] = wordVector;
                output[i] = topicVector;
                PrintVectors(wordVector, topicVector);

            }
            NeuralData neuralData = new NeuralData(input, output);
            Debug.WriteLine(neuralData.ToString());
            return neuralData;
        }

        private int[] CreateVector(List<string> classifier, List<string> data)
        {
            int[] vector = new int[classifier.Count];
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
            for (int i = 0; i < 1000; i++)
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

        private void PrintVectors(int[] input, int[] output)
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