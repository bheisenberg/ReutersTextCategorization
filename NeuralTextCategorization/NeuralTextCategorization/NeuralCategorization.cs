using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord;
using Accord.MachineLearning;
using Accord.Neuro;
using Accord.Neuro.Learning;
using System.Diagnostics;
using Accord.IO;
using Accord.Statistics.Distributions.Univariate;

public class NeuralCategorization
{
    public int folds = 10;
    private float targetError = 0.005f;
    public NeuralData neuralData;
    public NeuralCategorization(NeuralData neuralData)
    {
        this.neuralData = ShuffledData(neuralData);
        StartCrossValidation();
    }

    public int[] RandomIndices (double[][] inputArray)
    {
        int height = inputArray.GetLength(0);
        int[] indicies = new int[height];
        Debug.WriteLine("Height: " + height);
        for(int i=0; i < height; i++)
        {
            indicies[i] = i;
        }
        Random random = new Random();
        return indicies.OrderBy(x => random.Next()).ToArray();
    }

    public NeuralData ShuffledData (NeuralData neuralData)
    {
        Debug.WriteLine("SHUFFLING DATA");
        int[] indices = RandomIndices(neuralData.input);
        double[][] tempInput = new double[indices.Length][];
        double[][] tempOutput = new double[indices.Length][];
        for (int i=0; i < neuralData.input.Length; i++)
        {
            tempInput[i] = neuralData.input[indices[i]];
            tempOutput[i] = neuralData.output[indices[i]];
        }
        return new NeuralData(tempInput, tempOutput);
    }

    public void StartCrossValidation()
    {
        int size = neuralData.input.GetLength(0);
        int n = size / folds;
        for (int i=0; i < folds; i++)
        {
            Debug.WriteLine("CURRENT FOLD: " + i);
            int start = i * size;
            int end = (i + 1) * size - 1;
            FoldData currentFold = new FoldData(neuralData, start, end);
            int hiddenLayers = (currentFold.trainX.Length / currentFold.trainY.Length) / 2;
            IActivationFunction function = new BipolarSigmoidFunction();
            var network = new ActivationNetwork(function, currentFold.trainX.Count(), hiddenLayers, currentFold.trainY.Count());
            var teacher = new ParallelResilientBackpropagationLearning(network);
            new NguyenWidrow(network).Randomize();
            double error = int.MaxValue;
            double previousError = error;
            Debug.WriteLine("INITIAL ERROR: " + error);
            do
            {
                previousError = error;
                error = teacher.RunEpoch(currentFold.trainX, currentFold.trainY);
                Debug.WriteLine("ERROR: " + error + " CHANGE: " + (Math.Abs(previousError - error) / previousError));
            }
            while (error > 0 && (Math.Abs(previousError - error)) / previousError > targetError);
        }
    }
}

