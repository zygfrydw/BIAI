using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using NuralNetwork;

namespace NeuralNetorkResearch
{
    internal abstract class Tester
    {
        protected IEnumerable<ILearningSet> LearningSets;
        protected IEnumerable<ILearningSet> TestSets;

        protected Tester(string learningSetPath, string testSetPath)
        {
            TestSetPath = testSetPath;
            LearningSetPath = learningSetPath;
            LearningSets = GetLetterSet(LearningSetPath);
            TestSets = GetLetterSet(TestSetPath);
        }

        public string TestSetPath { get; private set; }
        public string LearningSetPath { get; private set; }

        protected NetworkParameters GetNetworkParameters()
        {
            var parameters = new NetworkParameters();
            parameters.LearningSets = LearningSets;
            parameters.TestSet = TestSets;
            parameters.Iterations = 20000;
            parameters.Alpha = 0.6;
            parameters.Eta = 0.2;
            parameters.Beta = 0.01;
            return parameters;
        }

        protected IEnumerable<ILearningSet> GetLetterSet(string path)
        {
            var mainDirectory = new DirectoryInfo(path);
            List<LearningSet> images = mainDirectory.GetDirectories().Select(SelectLearningSet).ToList();
            if (images.Count == 0)
                images.Add(SelectLearningSet(mainDirectory));
            return images;
        }

        private LearningSet SelectLearningSet(DirectoryInfo directory)
        {
            return new LearningSet
            {
                Name = directory.Name,
                Letters = directory.GetFiles("*.bmp").Select(x => new TeachLetter
                {
                    Name = Path.GetFileNameWithoutExtension(x.Name),
                    Image = new BitmapImage(new Uri(x.FullName))
                }).ToList()
            };
        }

        public abstract void Test();

        protected void TestNetwork(NetworkParameters parameters, NetworkStatistics statistic)
        {
            var network = new NeuralNetworkWraper();
            try
            {
                network.TeachNetwork(parameters, statistic.Record);
            }
            catch
            {
            }
            var mistakes = TestNetworkAnswers(network);
            statistic.NetworkMistakes = mistakes;
        }

        private int TestNetworkAnswers(NeuralNetworkWraper network)
        {
            return (from letter in TestSets.SelectMany(x => x.Letters) 
                let predictedLetter = network.CalculateOutput(letter.Image) 
                where predictedLetter != letter.Name 
                select letter).Count();
        }
    }
}