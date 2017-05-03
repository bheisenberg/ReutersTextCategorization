using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

public class Article
{
    public int[] words { get; set; }
    public int[] topics { get; set; }

    public Article (int[] words, int[] topics)
    {
        this.words = words;
        this.topics = topics;
    }
}
