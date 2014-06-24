using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NuralNetwork;

namespace NeuralNetorkResearch
{
    public abstract class TestAllFunctions : Tester
    {
        protected TestAllFunctions(string learningSetPath, string testSetPath, string verifySetPath)
            : base(learningSetPath, testSetPath, verifySetPath)
        {
        }

        protected void Test(NetworkParameters parameters, string fileName)
        {
            var statistic = new NetworkStatisticsForManyFunctions();
            foreach (var value in Enum.GetValues(typeof(NeuronFunction)).Cast<NeuronFunction>())
            {
                statistic.NeuronFunction = value;
                TestFunction(value,parameters, statistic);
            }
            statistic.Save(fileName);
        }

        protected abstract void TestFunction(NeuronFunction value, NetworkParameters parameters, NetworkStatisticsForManyFunctions statistic);
    }


    class StatisticRecord
    {
        public uint Iteration;
        public double Error;

        public StatisticRecord(uint iteration, double error)
        {
            Iteration = iteration;
            Error = error;
        }
    }


    internal interface INetworkStatistics
    {
         int TestSetLenght { get; set; }
         int NetworkMistakesForTestSet { get; set; }
        void Record(uint iteration, double error);
        void Save(string path);
        void RecordVerify(string name, int mistakes, int count);
    }


    public class NetworkStatisticsForManyFunctions : INetworkStatistics
    {
        private readonly Dictionary<uint, double[]> errors;
        private Dictionary<NeuronFunction, TestResult> results;
        private int neuronFunctionCount;
        public NetworkStatisticsForManyFunctions()
        {
            errors = new Dictionary<uint, double[]>();
            results = new Dictionary<NeuronFunction, TestResult>();
            foreach (var fun in Enum.GetValues(typeof(NeuronFunction)).Cast<NeuronFunction>())
            {
                var testRes = new TestResult();
                results.Add(fun, testRes);
            }
            neuronFunctionCount = Enum.GetValues(typeof (NeuronFunction)).Length;
        }
        public NeuronFunction NeuronFunction { get; set; }

        class TestResult
        {
            public int TestSetLenght { get; set; }
            public int NetworkMistakesForTestSet { get; set; }

            public List<string> verifyResults { get; set; }
            public List<string> verifySetName { get; set; }

            public TestResult()
            {
                verifyResults = new List<string>();
                verifySetName = new List<string>();
            }
        }

        public int TestSetLenght
        {
            get { return results[NeuronFunction].TestSetLenght; }
            set { results[NeuronFunction].TestSetLenght = value; }
        }

        public int NetworkMistakesForTestSet
        {
            get { return results[NeuronFunction].NetworkMistakesForTestSet; }
            set { results[NeuronFunction].NetworkMistakesForTestSet = value; }
        }

        public void Record(uint iteration, double error)
        {
            if (!errors.ContainsKey(iteration))
            {
                var record = new double[neuronFunctionCount];
                errors.Add(iteration, record);
            }

            var recordLine = errors[iteration];
            recordLine[(int) NeuronFunction] = error;
        }

        public void Save(string path)
        {
            var statFile = new StreamWriter(path);
            string header = Enum.GetNames(typeof (NeuronFunction)).Aggregate("", (current, fun) => string.Format("{0}{1}{2,18}", current, "; ", fun));


            string testSetResult = string.Format("{0,15}", "Test");
            
            foreach (var testResult in results.Select(x => x.Value))
            {
                var calc = testResult.NetworkMistakesForTestSet +"/"+ testResult.TestSetLenght;
                testSetResult += string.Format("; {0,18}", calc);
            }
            statFile.WriteLine("{0,15}{1}","Func", header);
            statFile.WriteLine(testSetResult);
            for (int i = 0; i < results[NeuronFunction.Sigmoid].verifyResults.Count; i++)
            {
                string verifySetResult = string.Format("{0,15}", results[NeuronFunction.Sigmoid].verifySetName[i]);
                foreach (var testResult in results.Select(x => x.Value))
                {
                    verifySetResult += string.Format("; {0,18}", testResult.verifyResults[i]);
                }
                statFile.WriteLine(verifySetResult);
            }


            header = Enum.GetNames(typeof(NeuronFunction)).Aggregate("", (current, fun) => string.Format("{0}{1}{2,20}", current, "; ", fun));
            statFile.WriteLine("{0,10}{1}", "Iteration", header);

            foreach (var errorRecord in errors)
            {
                string errorLine = string.Format("{0,10}", errorRecord.Key.ToString());
                errorLine = errorRecord.Value.Aggregate(errorLine, (current, e) => string.Format("{0}{1}{2,20}", current, "; ", e));
                statFile.WriteLine(errorLine);
            }
            
            statFile.Close();
        }

        public void RecordVerify(string name, int mistakes, int count)
        {
            results[NeuronFunction].verifyResults.Add(mistakes + "/" + count);
            results[NeuronFunction].verifySetName.Add(name);
        }
    }

    class NetworkStatistics : INetworkStatistics
    {
        private List<StatisticRecord> records; 
        public NetworkStatistics()
        {
            records = new List<StatisticRecord>();
        }

        public int TestSetLenght { get; set; }
        public int NetworkMistakesForTestSet { get; set; }
        public int VerifySetLenght { get; set; }
        public int NetworkMistakesForVerifySet { get; set; }

        public void Record(uint iteration, double error)
        {
            var rec = new StatisticRecord(iteration, error);
            records.Add(rec);
        }

        public void Save(string path)
        {
            var statFile = new StreamWriter(path);
            statFile.WriteLine("Iteration; Error");
            foreach (var record in records)
            {
                statFile.WriteLine(record.Iteration + "; " + record.Error);
            }
            statFile.WriteLine("--------------------");

            statFile.WriteLine("TS; " + NetworkMistakesForTestSet + "; " + TestSetLenght);
            statFile.WriteLine("VS; " + NetworkMistakesForVerifySet + "; " +VerifySetLenght);
            statFile.Close();

        }

        public void RecordVerify(string name, int mistakes, int count)
        {
            throw new NotImplementedException();
        }
    }
}