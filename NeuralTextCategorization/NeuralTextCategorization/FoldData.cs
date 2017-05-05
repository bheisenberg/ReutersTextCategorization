using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class FoldData
{
    public double[][] trainX { get; set; }
    public double[][] trainY { get; set; }
    public double[][] testX { get; set; }
    public double[][] testY { get; set; }
    
    public FoldData (NeuralData neuralData, int start, int end)
    {
        this.trainX = new double[neuralData.input.GetLength(0) - (end - start)][];
        this.trainY = new double[neuralData.output.GetLength(0) - (end - start)][];
        this.testX = new double[end - start][];
        this.testY = new double[end - start][];
        int height = neuralData.input.GetLength(0);
        for (int i = 0; i < height; i++)
        {
            if (i <= start && i > end)
            {
                testX[i] = neuralData.input[i];
                testY[i] = neuralData.output[i];
            } else
            {
                trainX[i] = neuralData.input[i];
                trainY[i] = neuralData.output[i];
            }
        }
    }
}
