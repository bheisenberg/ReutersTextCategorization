using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralTextCategorization
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlParser xmlParser = new XmlParser();
            NeuralCategorization neuralCategorization = new NeuralCategorization(xmlParser.GetNeuralData());
        }
    }
}
