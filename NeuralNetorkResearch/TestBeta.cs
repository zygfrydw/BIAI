using System;
using System.Globalization;
using System.IO;
using NuralNetwork;

namespace NeuralNetorkResearch
{
    class TestBeta : TestAllFunctions
    {
        private readonly string output;
        const double MinBeta = 0.01;
        const double Step = 0.05;
        const double MaxBeta = 0.9;
        public TestBeta(string learningSetPath, string testSetPath, string verifySetPath, string output )
            : base(learningSetPath, testSetPath, verifySetPath)
        {
            this.output = output;
        }


        protected override void TestFunction(NeuronFunction value, NetworkParameters parameters, NetworkStatisticsForManyFunctions statistic)
        {
            Console.WriteLine("Testing network for Beta: " + parameters.Beta + " with function " + value);
            TestNetwork(parameters, statistic);
        }

        public override void Test()
        {
            for (int k = 0; k < 10; k++)
            {
                var parameters = GetNetworkParameters();
                for (double i = MinBeta; i < MaxBeta; i += Step)
                {
                    parameters.Beta = i;
                    Test(parameters, GetStatisticFileName(i, k));
                    //break;
                }
            }
        }

        private string GetStatisticFileName(double d, int i)
        {
            var directory = output + @"\Beta" + i + @"\";
            var directoryInfo = new DirectoryInfo(directory);
            directoryInfo.Create();
            string name = directory + d.ToString(CultureInfo.InvariantCulture).Replace('.', '_') + ".csv";
            return name;
        }
    }
}