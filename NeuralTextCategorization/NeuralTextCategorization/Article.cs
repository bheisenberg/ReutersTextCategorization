using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Article
{
    public List<int> words { get; set; }
    public List<int> topics { get; set; }

    public Article (List<int> words, List<int> topics)
    {
        this.words = words;
        this.topics = topics;
    }
}
