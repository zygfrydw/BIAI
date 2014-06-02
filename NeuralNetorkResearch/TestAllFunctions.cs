﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NuralNetwork;

namespace NeuralNetorkResearch
{
    internal abstract class TestAllFunctions : Tester
    {
        protected TestAllFunctions(string learningSetPath, string testSetPath): base(learningSetPath, testSetPath)
        {
        }

        public override void Test()
        {
            foreach (var value in Enum.GetValues(typeof(NeuronFunction)).Cast<NeuronFunction>())
            {
                TestFunction(value);
            }

        }

        protected abstract void TestFunction(NeuronFunction value);
    }

    class TestBeta : TestAllFunctions
    {
        private readonly string output;
        const double MinBeta = 0.001;
        const double Step = 0.03;
        const double MaxBeta = 3.0;
        public TestBeta(string learningSetPath, string testSetPath, string output) : base(learningSetPath, testSetPath)
        {
            this.output = output;
        }

        protected override void TestFunction(NeuronFunction value)
        {
            var parameters = GetNetworkParameters();
            var statistic = new NetworkStatistics();
            for (double i = MinBeta; i < MaxBeta; i += Step)
            {
                parameters.Beta = i;
                Console.WriteLine("Testing network for Beta: " + i);
                TestNetwork(parameters, statistic);
                var name = GetStatisticFileName(value, i);
                statistic.Save(name);
            }
        }

        private string GetStatisticFileName(NeuronFunction value, double i)
        {
            string name = output +  @"\" + value.ToString() + "_" + i.ToString().Replace('.', '_') + ".csv";
            return name;
        }
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
    class NetworkStatistics
    {
        private List<StatisticRecord> records; 
        public NetworkStatistics()
        {
            records = new List<StatisticRecord>();
        }

        public int NetworkMistakes { get; set; }

        public void Record(uint iteration, double error)
        {
            var rec = new StatisticRecord(iteration, error);
            records.Add(rec);
        }

        public void Save(string path)
        {
            var statFile = new StreamWriter(path);
            statFile.Write("Iteration; Error");
            foreach (var record in records)
            {
                statFile.WriteLine(record.Iteration + "; " + record.Error);
            }
            statFile.WriteLine("----; ----");
            statFile.WriteLine("NetworkMistakes; " + NetworkMistakes);

        }
    }
}