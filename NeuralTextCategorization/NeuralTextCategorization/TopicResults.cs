using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class TopicResults
{

    public int truePositives { get; set; }
    public int trueNegatives { get; set; }
    public int falsePositives { get; set; }
    public int falseNegatives { get; set; }
    public string topicName { get; set; }

    public TopicResults (string topicName, int truePositives, int falsePositives, int trueNegatives, int falseNegatives)
    {
        this.topicName = topicName;
        this.truePositives = truePositives;
        this.falsePositives = falsePositives;
        this.trueNegatives = trueNegatives;
        this.falseNegatives = falseNegatives;
    }

    public override string ToString()
    {
        return string.Format("{0}: TP: {1}, FP: {2}, TN: {3}, FN: {4}", topicName, truePositives, falsePositives, trueNegatives, falseNegatives);
    }

    public double MCC()
    {
        double numeratorValue = (truePositives * trueNegatives) - (falsePositives * falseNegatives);
        double numerator = (numeratorValue == 0) ? numeratorValue : 1;
        double denominator = Math.Sqrt((truePositives + falsePositives) * (truePositives + falseNegatives) * (trueNegatives + falsePositives) + (trueNegatives + falseNegatives));
        return numerator / denominator;
    }
}

