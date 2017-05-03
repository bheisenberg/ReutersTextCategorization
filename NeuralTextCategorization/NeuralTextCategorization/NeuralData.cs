using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class NeuralData
{
    public int[][] input { get; set; }
    public int[][] output { get; set; }

    public NeuralData(int[][] input, int[][] output)
    {
        this.input = input;
        this.output = output;
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

