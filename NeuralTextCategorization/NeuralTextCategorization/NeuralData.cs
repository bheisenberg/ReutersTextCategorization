using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class NeuralData
{
    public double[][] input { get; set; }
    public double[][] output { get; set; }
    public string[] topics { get; set; }

    public NeuralData(double[][] input, double[][] output, string[] topics)
    {
        this.input = input;
        this.output = output;
        this.topics = topics;
    }

    public override string ToString()
    {
        string stringOutput = "";
        for (int i = 0; i < input.Length; i++)
        {
            stringOutput += "input: < ";
            for (int j = 0; j < input[i].Length; j++)
            {
                stringOutput += (string.Format("{0}, ", input[i][j]));
            }
            stringOutput += (">\n");
        }
        return stringOutput;
    }
}

