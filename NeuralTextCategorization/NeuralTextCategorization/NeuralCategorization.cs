using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord;
using Accord.MachineLearning;
using Accord.Neuro;
using Accord.Math;
using Accord.Neuro.Learning;
using System.Diagnostics;
using Accord.IO;
using Accord.Statistics.Distributions.Univariate;

public class NeuralCategorization
{
    public double[][] input { get; set; }
    public double[][] output { get; set; }
    public float learningRate = 0.3f;
    public float momentum = 0.9f;
    public float targetError = 0.0004f;
    public NeuralCategorization(NeuralData neuralData)
    {
        this.input = neuralData.input;
        this.output = neuralData.output;
        InitializeNetwork();
    }

    /*CrossValidation crossValidation = new CrossValidation(size: input.Length, folds: 10);
crossValidation.Fitting = delegate (int k, int[] indicesTrain, int[] indicesValidation)
{
    return new CrossValidationValues<object>();
};*/

    public void InitializeNetwork ()
    {
        Debug.WriteLine("INITIALIZING NETWORK");
        IActivationFunction function = new SigmoidFunction();
        var network = new ActivationNetwork(function, input[0].Length, neuronsCount: new[] { input[0].Length/2, output[0].Length });
        Debug.WriteLine("INITIALIZED");
        var teacher = new BackPropagationLearning(network);
        teacher.LearningRate = learningRate;
        teacher.Momentum = momentum;
        var y = output;

        // Iterate until stop criteria is met
        double error = teacher.RunEpoch(input, output);
        while (error > targetError)
        {
            error = teacher.RunEpoch(input, output);
            Debug.WriteLine("ERROR: "+error);
        }
    }


}

