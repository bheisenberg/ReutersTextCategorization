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
        return string.Format("{0}: TP: {1}, FP: {2}, TN: {3}, FN: {4}", topicName, truePositives, trueNegatives, falsePositives, falseNegatives);
    }

    public double MCC()
    {
        double numerator = (truePositives * trueNegatives) - (falsePositives * falseNegatives);
        double TPFP = (truePositives + falsePositives > 0) ? truePositives + falsePositives : 1;
        double TPFN = (truePositives + falseNegatives > 0) ? truePositives + falseNegatives : 1;
        double TNFP = (trueNegatives + falsePositives > 0) ? trueNegatives + falsePositives : 1;
        double TNFN = (trueNegatives + falseNegatives > 0) ? trueNegatives + falseNegatives : 1;
        double denominatorValue = Math.Sqrt((TPFP) * (TPFN) * (TNFP) * (TNFN));
        double denominator = (denominatorValue == 0) ? 1 : denominatorValue;
        return numerator / denominator;
    }
}

