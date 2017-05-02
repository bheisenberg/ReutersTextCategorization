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
            //Parse(inputFile);
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
            Dictionary<string, int> wordDict = new Dictionary<string, int>();
            List<string> topics = new List<string>();
            foreach (string input in inputFiles)
            {
                XDocument xdoc = CreateXDoc(input);
                topics.AddRange(GetTopic(xdoc));
                GetWords(xdoc, wordDict);
            }
            List<string> words = GetTopWords(wordDict);
            return new ArticleData(words, topics);
        }

        private List<string> GetTopWords(Dictionary<string, int> wordDict)
        {
            var wordsOrdered = (from entry in wordDict orderby entry.Value descending select entry).ToList();
            for (int i = 0; i < 1000; i++)
            {
                Debug.WriteLine(string.Format("{0}: {1}: {2}", i, wordsOrdered[i].Key, wordsOrdered[i].Value));
                words.Add(wordsOrdered[i].Key);
            }
        }

        private List<string> GetTopics(XDocument xdoc)
        {
            List<string> topics = new List<string>();
            foreach (var parent in xdoc.Elements())
            {
                var topicData = from element in xdoc.Elements().Elements()
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
                            if (!filteredWords.Contains(word))
                            {
                                if (!wordDict.Keys.Contains(word))
                                {
                                    wordDict.Add(word, 0);
                                }
                                wordDict[word] += 1;
                            }
                        }
                    }
                }
            }
            return wordDict;
        }
    }
}