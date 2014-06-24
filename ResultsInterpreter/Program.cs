using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ResultsInterpreter
{
    class Program
    {
        static void Main(string[] args)
        {

            for (int i =0; i < 10; i++)
            {
                var result =
                    new ResultFile(@"C:\Users\Zygfryd\Programowanie\C#\BIAI\NeuralNetorkResearch\Data\Result\Beta" + i+ @"\");

                var path = @"..\..\Data\00" + i + @"\";
                var info = new DirectoryInfo(path);
                info.Create();

                SaveFunctionStats(result, path);
                SaveIterationsStats(path, result);
            }
        }

        private static void SaveIterationsStats(string path, ResultFile result)
        {
            var writer = new StreamWriter(path + "Iterations.csv");
            var line = ToCsvLine(result.Headers);
            writer.WriteLine(line);
            foreach (var iteraton in result.Iterations)
            {
                line = iteraton.Key + "; " + ToCsvLine(iteraton.Value);
                writer.WriteLine(line);
            }
            writer.Close();
        }

        private static void SaveFunctionStats(ResultFile result, string path)
        {
            foreach (var functionResult in result.FunctionResults)
            {
                var writer = new StreamWriter(path + functionResult.Key + ".csv");
                var line = ToCsvLine(result.Headers);
                writer.WriteLine(line);
                foreach (var pair in functionResult.Value)
                {
                    line = pair.Key + "; " + ToCsvLine(pair.Value);
                    writer.WriteLine(line);
                }
                writer.Close();
            }
        }


        private static string ToCsvLine<T>(IEnumerable<T> list)
        {
            var first = list.First().ToString();
            return list.Skip(1).Aggregate(first, (s, o) => s + "; " + o.ToString());
        }
    }

    class Pair<T,U>
    {
        public T Key;
        public U Value;

        public Pair(T key, U value)
        {
            Key = key;
            Value = value;
        }
    }

    class ResultFile
    {
        
        public List<string> Headers;
        public Dictionary<string, List<Pair<double, int[]>>> FunctionResults;
        public List<Pair<double, int[]>> Iterations;  

        public ResultFile(string path)
        {
            FunctionResults = new Dictionary<string, List<Pair<double, int[]>>>();
            Iterations = new List<Pair<double, int[]>>();

            var directory = new DirectoryInfo(path);
            foreach (var fileName in directory.GetFiles("*.csv").Select(x => x.FullName))
            {
                var beta = ParseBetaFromName(fileName);
                var file = new StreamReader(fileName);
                if (Headers == null)
                {
                    Headers = ParseHeaders(file);
                }
                else
                {
                    file.ReadLine();
                }

                ParseFunctionResults(file, beta);
                ParseIterationResults(file, beta);
                file.Close();
            }
        }

        
        private void ParseIterationResults(StreamReader file, float beta)
        {
            var regex = new Regex(@"\s*(\w*);\s*(\d+,?[\d,]*);\s*(\d+,?[\d,]*);\s*(\d+,?[\d,]*);\s*(\d+,?[\d,]*);\s*(\d+,?[\d,]*)");
            var iterations = new List<Pair<int, double[]>>();
            while (!file.EndOfStream)
            {
                var line = file.ReadLine();
                var match = regex.Match(line);
                if (match.Success)
                {
                    var errors = new double[match.Groups.Count - 2];
                    for (int i = 2; i < match.Groups.Count; i++)
                    {
                        errors[i - 2] = double.Parse(match.Groups[i].Value);
                    }
                    var itNum = int.Parse(match.Groups[1].Value);
                    iterations.Add(new Pair<int, double[]>(itNum, errors));
                }
            }
            var results = new int[Headers.Count - 1];
            for (int i = 0; i < Headers.Count - 1; i++)
            {
                var rowIndex = iterations.FindIndex(x => x.Value[i] == 0);
                if (rowIndex > 0)
                    results[i] = iterations[rowIndex].Key - 1;
                else
                {
                    results[i] = iterations.Count;
                }
            }
            Iterations.Add(new Pair<double, int[]>(beta, results));
        }

        private void ParseFunctionResults(StreamReader file, double beta)
        {
            string line = file.ReadLine();
            var regex = new Regex(@"\s*(\w*);\s*(\d+)/\d+;\s*(\d+)/\d+;\s*(\d+)/\d+;\s*(\d+)/\d+;\s*(\d+)/\d+", RegexOptions.Compiled);
            while (!line.StartsWith(" Iteration"))
            {
                var match = regex.Match(line);
                var results = new int[match.Groups.Count - 2];
                for (int i = 2; i < match.Groups.Count; i++)
                {
                    results[i - 2] = int.Parse(match.Groups[i].Value);
                }


                var functionName = match.Groups[1].Value;
                if(!FunctionResults.ContainsKey(functionName))
                    FunctionResults.Add(functionName, new List<Pair<double, int[]>>());
                FunctionResults[functionName].Add(new Pair<double, int[]>(beta, results));
                line = file.ReadLine();
            }
        }

        private List<string> ParseHeaders(StreamReader file)
        {
            var line = file.ReadLine();
            var regex = new Regex(@"(\w*);\s*(\w*);\s*(\w*);\s*(\w*);\s*(\w*);\s*(\w*)");
            var math = regex.Match(line);
            var list = new List<string>();
            for (int i = 1; i < math.Groups.Count; i++)
            {
                list.Add(math.Groups[i].Value);
            }
            list[0] = "Beta";
            return list;
        }

        private float ParseBetaFromName(string path)
        {
            var name = Path.GetFileNameWithoutExtension(path);
            var number = name.Replace('_', ',');
            return float.Parse(number);
        }
    }
}
