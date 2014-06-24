using System;
using System.Globalization;
using System.IO;
using NuralNetwork;

namespace NeuralNetorkResearch
{
    public class EtaTest : TestAllFunctions
    {
        
        private readonly string output;
        const double MinEta = 0.01;
        const double Step = 0.1;
        const double MaxEta = 0.6;
        //(0.01 : 0.6)>
        public EtaTest(string learningSetPath, string testSetPath, string verifySetPath, string output)
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
            for (int k = 0; k < 3; k++)
            {
                var parameters = GetNetworkParameters();
                for (double i = MinEta; i < MaxEta; i += Step)
                {
                    parameters.Eta = i;
                    Test(parameters, GetStatisticFileName(i, k));
                    //break;
                }
            }
        }

        private string GetStatisticFileName(double d, int i)
        {
            var directory = output + @"\Eta" + i + @"\";
            var directoryInfo = new DirectoryInfo(directory);
            directoryInfo.Create();
            string name = directory + d.ToString(CultureInfo.InvariantCulture).Replace('.', '_') + ".csv";
            return name;
        }
        
 
    }
}