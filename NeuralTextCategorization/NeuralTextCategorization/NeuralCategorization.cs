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
    public NeuralCategorization(NeuralData neuralData)
    {
        this.input = neuralData.input;
        this.output = neuralData.output;
        InitializeNetwork();
    }

    public void InitializeNetwork ()
    {
        /*CrossValidation crossValidation = new CrossValidation(size: input.Length, folds: 10);
        crossValidation.Fitting = delegate (int k, int[] indicesTrain, int[] indicesValidation)
        {
            return new CrossValidationValues<object>();
        };*/
        IActivationFunction function = new BipolarSigmoidFunction();
        var network = new ActivationNetwork(function, inputsCount: input.Count(), neuronsCount: new[] { 5, 1 });

        // Create a Levenberg-Marquardt algorithm
        // Because the network is expecting multiple outputs,
        // we have to convert our single variable into arrays
        var teacher = new LevenbergMarquardtLearning(network)
        {
            UseRegularization = true
        };
        //
        var y = output;

        // Iterate until stop criteria is met
        double error = double.PositiveInfinity;
        double previous;

        do
        {
            previous = error;
            error = teacher.RunEpoch(input, y);
        }
        while (Math.Abs(previous - error) < 1e-10 * previous);


        // Classify the samples using the model
        int[] answers = input.Apply(network.Compute).GetColumn(0).Apply(Math.Sign);
        
    }


}

