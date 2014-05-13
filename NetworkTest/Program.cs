using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuralNetwork;

namespace NetworkTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var network = new NeuralNetwork(3, 9, 4);
            var teachingVectors = createTeachingVectors();
            network.TeachNetwork(teachingVectors, 0.1);
            var query = new double[] {0, 0, 0, 0, 0, 1, 1, 0, 0};
            var answere = network.CalculateResponse(query);
            for (int i = 0; i < answere.Length; i++)
            {
                Console.WriteLine("{0}: {1}", i, answere[i]);
            }
            Console.ReadKey();
        }

        private static List<TeachingVector> createTeachingVectors()
        {
            var teachingVectors = new List<TeachingVector>();
            var vector = new TeachingVector();
            vector.Inputs = new double[] {1, 0, 1, 0, 1, 0, 1, 0, 1};
            vector.Outputs = new double[] {1, 0, 0, 0};
            teachingVectors.Add(vector);
            vector.Inputs = new double[] {1, 1, 1, 1, 0, 1, 1, 1, 1};
            vector.Outputs = new double[] {0, 1, 0, 0};
            teachingVectors.Add(vector);
            vector.Inputs = new double[] {0, 1, 0, 1, 1, 1, 0, 1, 0};
            vector.Outputs = new double[] {0, 0, 1, 0};
            teachingVectors.Add(vector);
            vector.Inputs = new double[] {0, 0, 0, 1, 1, 1, 0, 0, 0};
            vector.Outputs = new double[] {0, 0, 0, 1};
            teachingVectors.Add(vector);
            return teachingVectors;
        }
    }
}
