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
        string[] files = Directory.GetFiles("Resources", "*.sgm");
        Dictionary<string, int> words;
        public XmlParser()
        {
            foreach (string file in files)
            {
                Debug.WriteLine(file);
            }
            Parse(files);
        }

        private void Parse(string[] inputFiles)
        {
            words = new Dictionary<string, int>();
            ArticleData articleData = GetArticleData(inputFiles);
            foreach (string word in articleData.words)
            {
                Debug.WriteLine(word);
            }
        }

        private ArticleData GetArticleData(string[] inputFiles)
        {
            List<string> words = new List<string>();
            List<string> topics = new List<string>();
            List<XElement> articleElements = GetArticleElements(inputFiles);
            foreach (XElement articleElement in articleElements)
            {
                topics.AddRange(GetTopics(articleElement));
                words.AddRange(GetWords(articleElement));
            }
            List<string> topWords = GetTopWords(words);
            return new ArticleData(words, topics, topWords);
        }

        private List<string> GetTopWords(List<string> words)
        {
            List<string> topWords = new List<string>();
            List<string> filteredWords = new List<string> { "the", "and", "if", "then", "a", "be", "to", "of", "in", "that", "have", "i", "it", "for", "not", "on", "with", "he", "as", "you", "do", "at", "this", "but", "by", "is", "from", "reuter", "reuters", "", "-" };
            Dictionary<string, int> wordDict = new Dictionary<string, int>();
            foreach (string word in words)
            if (!filteredWords.Contains(word))
            {
                if (!wordDict.Keys.Contains(word))
                {
                    wordDict.Add(word, 0);
                }
                wordDict[word] += 1;
            }
            var wordsOrdered = (from entry in wordDict orderby entry.Value descending select entry).ToList();
            for (int i = 0; i < 1000; i++)
            {
                Debug.WriteLine(string.Format("{0}: {1}: {2}", i, wordsOrdered[i].Key, wordsOrdered[i].Value));
                words.Add(wordsOrdered[i].Key);
            }
            return topWords;
        }

        private List<string> GetTopics(XElement articleElement)
        {
            List<string> topics = new List<string>();
            var topicData = from element in articleElement.Elements().Elements()
                            where element.Name == "TOPICS"
                            select element;
            if (topicData.Count() > 0)
            {
                foreach (string topic in topicData)
                {
                    if (topic != "" && !topics.Contains(topic))
                    {
                        topics.Add(topic);
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

        private List<XElement> GetArticleElements(string[] inputFiles)
        {
            List<XElement> articleElements = new List<XElement>();
            foreach (string input in inputFiles)
            {
                XDocument xdoc = CreateXDoc(input);
                foreach (var parent in xdoc.Elements())
                {
                    articleElements.Add(parent);
                }
            }
            return articleElements;
        }

        private List<string> GetWords(XElement articleElement)
        {
            List<string> words = new List<string>();
            foreach (var childElement in articleElement.Elements())
            {
                var bodyData = from element in childElement.Elements().Elements()
                           where element.Name == "BODY"
                           select element;
                if (bodyData.Count() > 0)
                {
                    string body = bodyData.First().Value.ToLower();
                    List<string> articleWords = body.Split().ToList();
                    foreach (string word in articleWords)
                    {
                        word.Replace("+", "".Replace(".", "").Replace(",", ""));
                    }
                    words = articleWords;
                }
            }
            return words;
        }

        private Dictionary<string, int> GetWords(XDocument xdoc, Dictionary<string, int> wordDict)
        {
            List<string> words = new List<string>();
            foreach (var parent in xdoc.Elements())
            {
                foreach (var childElement in parent.Elements())
                {
                    List<string> filteredWords = new List<string> { "the", "and", "if", "then", "a", "be", "to", "of", "in", "that", "have", "i", "it", "for", "not", "on", "with", "he", "as", "you", "do", "at", "this", "but", "by", "is", "from", "reuter", "reuters", "", "-" };
                    var data = from element in childElement.Elements().Elements()
                               where element.Name == "BODY"
                               select element;
                    if (data.Count() > 0)
                    {
                        string body = data.First().Value.ToLower();
                        var bodySplit = body.Split();
                        foreach (string word in bodySplit)
                        {
                            word.Replace("+", "".Replace(".", "").Replace(",", ""));

                        }
                    }
                }
            }
            return wordDict;
        }

    }
}