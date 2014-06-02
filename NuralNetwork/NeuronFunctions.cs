using System;

namespace NuralNetwork
{
    public delegate double SigmoidFunction(double input); 
    public static class NeuronFunctions
    {
        public static double Sigmoid(double input)
        {
            return 1.0 / (1.0 + Math.Exp(-input));
        }
        public static double HyperbolicTangens(double input)
        {
            return Math.Tanh(input);
        }
        public static double Sinusoidal(double input)
        {
            return Math.Sin(input);
        }
        public static double Cosinusoidal(double input)
        {
            return Math.Cos(input);
        }
        public static double Function001(double input)
        {
            return input / (1 + Math.Abs(input));
        }

        public static SigmoidFunction GetFunctionFor(NeuronFunction function)
        {
            switch (function)
            {
                case NeuronFunction.Sigmoid:
                    return Sigmoid;
                case NeuronFunction.HyperbolicTangens:
                    return HyperbolicTangens;
                case NeuronFunction.Sinusoida:
                    return Sigmoid;
                case NeuronFunction.Cosinusoidal:
                    return Cosinusoidal;
                case NeuronFunction.Function001:
                    return Function001;
                default:
                    throw new ArgumentOutOfRangeException("function");
            }
        }
    }
}