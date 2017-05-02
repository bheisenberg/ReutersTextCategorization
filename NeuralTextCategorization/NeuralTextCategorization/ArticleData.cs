using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;

public class ArticleData
{
    public List<string> topWords { get; set; }
    public List<string> topics { get; set; }
    public List<RawArticle> articles { get; set; }

    public ArticleData (List<RawArticle> articles, List<string> topics, List<string> topWords)
    {
        this.articles = articles;
        this.topics = topics;
        this.topWords = topWords;
    }
}
