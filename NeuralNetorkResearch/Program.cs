using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuralNetwork;

namespace NeuralNetorkResearch
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 2)
            {
                var tester = new TestBeta(args[0], args[1], args[2]);
                tester.Test();
            }
            else
            {
                Console.WriteLine("Podaj parametry: zbiory testowe, zbiory testowe, katalog na dane");
            }
        }
    }


}
