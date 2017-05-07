using System;
using System.Collections.Generic;
using System.Linq;
using Accord.Neuro;
using Accord.Neuro.Learning;
using System.Diagnostics;
using Accord.Statistics.Analysis;
using System.IO;
using System.Text;

public class NeuralCategorization
{
    public int folds = 10;
    private float targetError = 0.005f;
    private string resultString = "result.csv";
    private string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TextCategorization");
    private string resultFile;
    public NeuralData neuralData;
    public Dictionary<int, TopicResults> ConfusionMatrix;
    public NeuralCategorization(NeuralData neuralData)
    {
        this.neuralData = ShuffledData(neuralData);
        resultFile = Path.Combine(path, resultString);
        InitConfusionMatrix();
        StartCrossValidation();
    }

    public void InitConfusionMatrix ()
    {
        this.ConfusionMatrix = new Dictionary<int, TopicResults>();
        for (int i=0; i < neuralData.topics.Length; i++)
        {
            ConfusionMatrix[i] = new TopicResults(neuralData.topics[i], 0, 0, 0, 0);
        }
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
        string[] tempTopics = new string[neuralData.topics.Length];
        for (int i=0; i < neuralData.input.Length; i++)
        {
            tempInput[i] = neuralData.input[indices[i]];
            tempOutput[i] = neuralData.output[indices[i]];
        }
        return new NeuralData(tempInput, tempOutput, neuralData.topics);
    }

    public void StartCrossValidation()
    {
        int n = neuralData.input.GetLength(0);
        int size = n / folds;
        for (int i=0; i < folds; i++)
        {
            Debug.WriteLine("CURRENT FOLD: " + i);
            int start = i * size;
            int end = (i + 1) * size - 1;
            FoldData currentFold = new FoldData(neuralData, start, end);
            int hiddenLayers = (currentFold.trainX[0].Length + currentFold.trainY[0].Length) / 2;
            Debug.WriteLine(hiddenLayers);
            IActivationFunction function = new BipolarSigmoidFunction();
            var network = new ActivationNetwork(function, currentFold.trainX[0].Length, hiddenLayers, currentFold.trainY[0].Length);
            var teacher = new BackPropagationLearning(network);
            teacher.Momentum = 0.9f;
            teacher.LearningRate = 0.1f;
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
            double[][] predictedY = ComputeOutput(network, currentFold.testX);
            CalculateResults(predictedY, currentFold.testY);
        }
        File.Delete(resultFile);
        File.Create(resultFile).Close();
        using (StreamWriter rw = new StreamWriter(resultFile, true, Encoding.UTF8))
        {
            foreach (TopicResults topicResult in ConfusionMatrix.Values)
            {
                rw.WriteLine(string.Format("{0},{1},{2},{3},{4},{5}", topicResult.topicName, topicResult.truePositives, topicResult.trueNegatives, topicResult.falsePositives, topicResult.falseNegatives, topicResult.MCC()));
            }
        }
    }

    public double[][] ComputeOutput (ActivationNetwork network, double[][] testX)
    {
        double[][] tempOutput = new double[testX.GetLength(0)][];
        for (int i=0; i < testX.GetLength(0); i++)
        {
            tempOutput[i] = network.Compute(testX[i]);
        }
        return tempOutput;
    }

    public void CalculateResults (double[][] predictedY, double[][] actualY)
    {
        for(int i=0; i < predictedY.GetLength(0); i++)
        {
            for(int j=0; j < predictedY[i].Length; j++)
            {
                if (Math.Round(predictedY[i][j]) == 1 && actualY[i][j] == 1) ConfusionMatrix[j].truePositives += 1;
                if (Math.Round(predictedY[i][j]) == 0 && actualY[i][j] == 0) ConfusionMatrix[j].trueNegatives += 1;
                if (Math.Round(predictedY[i][j]) == 1 && actualY[i][j] == 0) ConfusionMatrix[j].falsePositives += 1;
                if (Math.Round(predictedY[i][j]) == 0 && actualY[i][j] == 1) ConfusionMatrix[j].falseNegatives += 1;
            }
        }
    }
}

