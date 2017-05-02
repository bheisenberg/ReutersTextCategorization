using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

public class Article
{
    public List<int> words { get; set; }
    public List<int> topics { get; set; }

    public Article (List<int> words, List<int> topics)
    {
        this.words = words;
        this.topics = topics;
    }

    public override string ToString()
    {
        string output = "";
        output += "\nwords: < ";
        for (int i=0; i < words.Count; i++)
        {
            output +=(string.Format("{0}, ", words[i]));
        }
        output += ">\n";
        output += ("topics: < ");
        for (int i = 0; i < topics.Count; i++)
        {
            output += (string.Format("{0}, ", topics[i]));
        }
        output += (">");
        return output;
    }
}
