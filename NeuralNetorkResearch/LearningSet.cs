using System.Collections.Generic;
using NuralNetwork;

namespace NeuralNetorkResearch
{
    public class LearningSet : ILearningSet
    {
        IEnumerable<ITeachLetter> ILearningSet.Letters
        {
            get { return Letters; }
        }
        public List<TeachLetter> Letters { get; set; }
        public string Name { get; set; }
    }
}