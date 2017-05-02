using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ArticleData
{
    public List<string> topics { get; set; }
    public List<string> words { get; set; }
    public List<string> topWords { get; set; }

    public ArticleData (List<string> topics, List<string> words, List<string> topWords)
    {
        this.topics = topics;
        this.words = words;
        this.topWords = topWords;
    }
}
