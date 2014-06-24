using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using NuralNetwork;

namespace NeuralNetorkResearch
{
    public abstract class Tester
    {
        protected IEnumerable<ILearningSet> LearningSets;
        protected IEnumerable<ILearningSet> TestSets;

        protected Tester(string learningSetPath, string testSetPath, string VerifySetPath)
        {
            TestSetPath = testSetPath;
            LearningSetPath = learningSetPath;
            LearningSets = GetLetterSet(LearningSetPath);
            TestSets = GetLetterSet(TestSetPath);
            VerifySet = GetLetterSet(VerifySetPath);
        }

        public IEnumerable<ILearningSet> VerifySet { get; set; }

        public string TestSetPath { get; private set; }
        public string LearningSetPath { get; private set; }

        protected NetworkParameters GetNetworkParameters()
        {
            var parameters = new NetworkParameters();
            parameters.LearningSets = LearningSets;
            parameters.TestSet = TestSets;
            parameters.MaxIterations= 1000;
            parameters.Alpha = 0.6;
            parameters.Eta = 0.2;
            parameters.Beta = 0.01;
            parameters.NetworkError = 0.01;
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

        protected void TestNetwork(NetworkParameters parameters, INetworkStatistics statistic)
        {
            var network = new NeuralNetworkWraper();
            try
            {
                network.TeachNetwork(parameters, statistic.Record);
            }
            catch(Exception exception)
            {
                Console.WriteLine("Catch ex: " + exception.Message);
            }
            var mistakes = TestNetworkAnswers(network, TestSets);
            statistic.TestSetLenght = TestSets.Sum(x => x.Letters.Count());
            statistic.NetworkMistakesForTestSet = mistakes;

            foreach (var set in VerifySet)
            {
                mistakes = TestNetworkAnswers(network, set);
                var count = set.Letters.Count();
                statistic.RecordVerify(set.Name, mistakes, count);
            }
            
            
            
        }

        private int TestNetworkAnswers(NeuralNetworkWraper network,  IEnumerable<ILearningSet> testSets)
        {
            return (from letter in testSets.SelectMany(x => x.Letters) 
                let predictedLetter = network.CalculateOutput(letter.Image) 
                where predictedLetter != letter.Name 
                select letter).Count();
        }
        private int TestNetworkAnswers(NeuralNetworkWraper network, ILearningSet testSets)
        {
            return (from letter in testSets.Letters
                    let predictedLetter = network.CalculateOutput(letter.Image)
                    where predictedLetter != letter.Name
                    select letter).Count();
        }
    }
}