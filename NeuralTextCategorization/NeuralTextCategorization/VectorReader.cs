using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

public class VectorReader
{
    private string inputFile;
    private string outputFile;
    private string topicFile;
    public VectorReader (string inputFile, string outputFile, string topicFile)
    {
        this.inputFile = inputFile;
        this.outputFile = outputFile;
        this.topicFile = topicFile;
    }

    public NeuralData CreateVectors()
    {
        string[] inputLines = File.ReadAllLines(inputFile);
        string[] outputLines = File.ReadAllLines(outputFile);
        double[][] inputVectors = new double[inputLines.Length][];
        double[][] outputVectors = new double[outputLines.Length][];
        string[] topics = File.ReadAllLines(topicFile);
        for (int i=0; i < inputLines.Length; i++)
        {
            //Debug.WriteLine(inputLines[i]);
            inputVectors[i] = inputLines[i].Split(' ').Select(n => Convert.ToDouble(n)).ToArray();
            outputVectors[i] = outputLines[i].Split(' ').Select(n => Convert.ToDouble(n)).ToArray();
        }
        return new NeuralData(inputVectors, outputVectors, topics);
    }
}
