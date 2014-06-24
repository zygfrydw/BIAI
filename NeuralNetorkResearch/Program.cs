using System;
using System.Windows.Forms;


namespace NeuralNetorkResearch
{
    class Program
    {
        static void Main(string[] args)
        {
            //if (args.Length > 2)
            //{
                var tester = new TestBeta(@"..\..\Data\Teach", @"..\..\Data\Test", @"..\..\Data\Verify", @"..\..\Data\Result");
                tester.Test();
                System.Windows.Forms.Application.SetSuspendState(PowerState.Hibernate, true, true);
            //}
            //else
            //{
            //    Console.WriteLine("Podaj parametry: zbiory testowe, zbiory testowe, katalog na dane");
            //}
        }
    }


}
