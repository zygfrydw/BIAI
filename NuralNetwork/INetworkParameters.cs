using System.Collections.Generic;

namespace NuralNetwork
{
    public interface INetworkParameters
    {
        double NetworkError { get;}
        double Alpha { get;}
        double Eta { get;}
        double Beta { get;}
        IEnumerable<ILearningSet> LearningSets { get; }
        uint Iterations { get; }
        uint MaxIterations { get; set; }
        ConversionType ConversionType { get;  }
        NeuronFunction NeuronFunction { get; }
        IEnumerable<ILearningSet> TestSet { get; }
    }
}