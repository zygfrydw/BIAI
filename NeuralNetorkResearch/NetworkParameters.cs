using System.Collections.Generic;
using NuralNetwork;

namespace NeuralNetorkResearch
{
    public class NetworkParameters : INetworkParameters

    {
        public double NetworkError { get;  set; }
        public double Alpha { get;  set; }
        public double Eta { get;  set; }
        public double Beta { get;  set; }
        public IEnumerable<ILearningSet> LearningSets { get;  set; }
        public uint Iterations { get;  set; }
        public uint MaxIterations { get; set; }
        public ConversionType ConversionType { get;  set; }
        public NeuronFunction NeuronFunction { get;  set; }
        public IEnumerable<ILearningSet> TestSet { get;  set; }
    }
}