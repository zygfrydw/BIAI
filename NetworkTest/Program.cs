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
            //Sieć rozpoznająca 4 znaki w macierzy 3x3
            var network = new SigmoidalNeuralNetwork(3, 9, 4);
            var teachingVectors = createTeachingVectors();
            network.TeachNetwork(teachingVectors, teachingVectors,  0.05);
            Console.WriteLine("Query 1");
            var query = new double[] { 0, 0, 0, 0, 1, 1, 1, 0, 0 };
            var answere = network.CalculateResponse(query);
            for (int i = 0; i < answere.Length; i++)
            {
                Console.WriteLine("{0}: {1}", i, answere[i]);
            }
            Console.WriteLine("Query 2");
            query = new double[] { 1, 1, 0, 0, 1, 0, 1, 0, 0 };
            answere = network.CalculateResponse(query);
            for (int i = 0; i < answere.Length; i++)
            {
                Console.WriteLine("{0}: {1}", i, answere[i]);
            }
            Console.ReadKey();
        }

        private static List<TeachingVector> createTeachingVectors()
        {
            var teachingVectors = new List<TeachingVector>();
            //Znak X
            var vector = new TeachingVector
            {
                Inputs = new double[] {1, 0, 1, 0, 1, 0, 1, 0, 1},
                Outputs = new double[] {1, 0, 0, 0}
            };
            teachingVectors.Add(vector);
            //Znak O
            vector= new TeachingVector
            {
                Inputs = new double[] {1, 1, 1, 1, 0, 1, 1, 1, 1},
                Outputs = new double[] {0, 1, 0, 0}
            };
            teachingVectors.Add(vector);
            //Znak +
            vector = new TeachingVector
            {
                Inputs = new double[] {0, 1, 0, 1, 1, 1, 0, 1, 0},
                Outputs = new double[] {0, 0, 1, 0}
            };
            teachingVectors.Add(vector);
            //Znak -
            vector = new TeachingVector
            {
                Inputs = new double[] {0, 0, 0, 1, 1, 1, 0, 0, 0},
                Outputs = new double[] {0, 0, 0, 1}
            };
            teachingVectors.Add(vector);
            return teachingVectors;
        }
    }
}
