using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord;
using Accord.MachineLearning;
using Accord.Math;
using System.Diagnostics;

public class NeuralCategorization
{
    public int[][] input { get; set; }
    public int[][] output { get; set; }
    public NeuralCategorization(NeuralData neuralData)
    {
        this.input = neuralData.input;
        this.output = neuralData.output;
        Debug.WriteLine("Executing neural network");
    }
}

