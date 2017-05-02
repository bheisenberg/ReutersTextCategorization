using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class RawArticle
{
    public List<string> words { get; set; }
    public List<string> topics { get; set; }

    public RawArticle(List<string> words, List<string> topics)
    {
        this.words = words;
        this.topics = topics;
    }
} 
