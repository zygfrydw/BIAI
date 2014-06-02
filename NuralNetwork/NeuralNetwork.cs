using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuralNetwork
{

    public class TeachingVector
    {
        public double[] Inputs;
        public double[] Outputs;
    }


    public class ErrorCalculation {
	private double globalError;
	private int setSize;


    /// <summary>
    ///  Returns the root mean square error for a complete training set.
    ///  <returns>The current error for the neural network.</returns>
    /// </summary>
	public double CalculateRms() {
		double err = Math.Sqrt(this.globalError / (this.setSize));
		return err;
	}

	/// <summary>
    ///  Reset the error accumulation to zero.
	/// </summary>
	public void Reset() {
		globalError = 0;
		setSize = 0;
	}

	/// <summary>
	///  Called to update for each number that should be checked.
    ///  <param name="actual">The actual output.</param> 
    ///  <param name="ideal">The ideal output</param>
    /// </summary>
	public void UpdateError(double[] actual,  double[] ideal) {
		for (int i = 0; i < actual.Length; i++) {
			double delta = ideal[i] - actual[i];
			globalError += delta * delta;
			setSize += ideal.Length;
		}
	}

}

    public abstract class NeuralNetwork
    {
        
        class Neuron
        {
            public double Input;
            public double Output;
            public double[] Weights;
            public double Error { get; set; }
            public double[] PrevWeights;
        }
        class Layer
        {
            public Neuron[] Neurons;
        }

        private readonly int layersCount;
        private readonly int inputs;
        private readonly int outputs;
        private Layer[]   layers;
        
        private int outputLayer;
        private int inputLayer;
        //Wspolczynnik uczenia
        private double eta; 
        //Wspołczynnik mementum
        private double alfa;
        private readonly ErrorCalculation errorCalculator;

        public NeuralNetwork(int layersCount, int inputs, int outputs, double eta = 0.2, double alfa = 0.6)
        {
            this.layersCount = layersCount;
            this.inputs = inputs;
            this.outputs = outputs;
            this.eta = eta;
            this.alfa = alfa;
            inputLayer = 0;
            outputLayer = this.layersCount - 1;
            CreateLayersWithNeurons();
            CreateWeightsTableAndRandomize();
            errorCalculator = new ErrorCalculation();
        }

        private void CreateLayersWithNeurons()
        {
            layers = new Layer[layersCount];
            layers[inputLayer] = new Layer {Neurons = new Neuron[inputs]};
            layers[outputLayer] = new Layer {Neurons = new Neuron[outputs]};
            int cardinality = (int) Math.Ceiling( Math.Sqrt((inputs*outputs)));
            for (int i = 1; i < outputLayer; i++)
            {
                layers[i] = new Layer {Neurons = new Neuron[cardinality]};
            }
        }

        private void CreateWeightsTableAndRandomize()
        {
            InitializeInputLayerWeights();
            RandomizeWeights();
        }

        private void RandomizeWeights()
        {
            var rand = new Random();

            for (int i = 1; i < layersCount; i++)
            {
                for (int j = 0; j < layers[i].Neurons.Length; j++)
                {
                    layers[i].Neurons[j] = new Neuron();
                    var previousLayerCardinality = layers[i - 1].Neurons.Length;
                    layers[i].Neurons[j].Weights = new double[previousLayerCardinality];
                    layers[i].Neurons[j].PrevWeights = new double[previousLayerCardinality];
                    for (int k = 0; k < previousLayerCardinality; k++)
                    {
                        var randNumber = (((rand.Next()%1000000L)/1700.0) - 9.8)*0.0015;
                        if (randNumber == 0)
                            randNumber = 0.01492;
                        layers[i].Neurons[j].Weights[k] = randNumber;
                    }
                }
            }
        }

        private void InitializeInputLayerWeights()
        {
            for (int j = 0; j < layers[0].Neurons.Length; j++)
            {
                layers[0].Neurons[j] = new Neuron {Weights = new double[1], PrevWeights = new double[1]};
                layers[0].Neurons[j].Weights[0] = 1;
            }
        }

        public double[] CalculateResponse(double[] inputData)
        {
            if (inputData.Length != inputs)
            {
                throw new ArgumentException("Incompatible inputs count");
            }

            SetInputsInNetwork(inputData);
            PropagateInputImpuls();

            return layers[outputLayer].Neurons.Select(x => x.Output).ToArray();
        }

        private void SetInputsInNetwork(double[] inputData)
        {
            for (int i = 0; i < inputs; i++)
            {
                layers[inputLayer].Neurons[i].Input = inputData[i];
                layers[inputLayer].Neurons[i].Output = inputData[i];
            }
        }

        private void PropagateInputImpuls()
        {
            for (int i = 1; i < layers.Length; i++)
            {
                for (int j = 0; j < layers[i].Neurons.Length; j++)
                {
                    layers[i].Neurons[j].Input = 0.0;
                    for (int k = 0; k < layers[i].Neurons[j].Weights.Length; k++)
                    {
                        layers[i].Neurons[j].Input += layers[i - 1].Neurons[k].Output*layers[i].Neurons[j].Weights[k];
                        var input = layers[i].Neurons[j].Input;
                        layers[i].Neurons[j].Output = NeuronFunction(input);
                    }
                }
            }
        }

        protected abstract double NeuronFunction(double input);


        public void TeachNetwork(IEnumerable<TeachingVector> teachingVectors, IEnumerable<TeachingVector> testVectors, double maxNetworkError, ulong maxLoops = 40000, Action<uint, double> notifyIteration = null)
        {
            uint max = 0;
            double networkError;
            
            do
            {
                foreach (var vector in teachingVectors)
                {
                    TeachNetworkWithVector(vector);
                }

                networkError = CalculateNetworkError(testVectors);

                max++;
                if (notifyIteration != null) 
                    notifyIteration(max, networkError);
                
                if (max > maxLoops)
                    throw new InvalidOperationException("Error with teaching");
            } while (networkError > maxNetworkError);
        }

        private void TeachNetworkWithVector(TeachingVector vector)
        {
            var response = CalculateResponse(vector.Inputs);
            CalculateResponseErrorOnOutputs(vector.Outputs, response);
            PropagateErrors();
            ApplayNewWeights();
        }

        private void CalculateResponseErrorOnOutputs(double[] expected, double[] response)
        {
            for (int i = 0; i < response.Length; i++)
            {
                layers[outputLayer].Neurons[i].Error = expected[i] - response[i];
            }
        }

        private void PropagateErrors()
        {
            for (var i = layersCount - 2; i >= 0; i--)
            {
                for (var j = 0; j < layers[i].Neurons.Length; j++)
                {
                    layers[i].Neurons[j].Error = 0.0;
                    var actualNeuron = layers[i].Neurons[j];
                    foreach (var outerNeuron in layers[i + 1].Neurons)
                    {
                        actualNeuron.Error += actualNeuron.Output*(1.0 - actualNeuron.Output)*outerNeuron.Error*
                                              outerNeuron.Weights[j];
                    }
                }
            }
        }

        private void ApplayNewWeights()
        {
            for (int i = 1; i < layersCount; i++)
            {
                for (int j = 0; j < layers[i].Neurons.Length; j++)
                {
                    Neuron actualNeuron = layers[i].Neurons[j];
                    for (int k = 0; k < actualNeuron.Weights.Length; k++)
                    {
                        var tmp = actualNeuron.Weights[k];
                        actualNeuron.Weights[k] += eta*actualNeuron.Error*layers[i - 1].Neurons[k].Output +
                                                   alfa*(actualNeuron.Weights[k] - actualNeuron.PrevWeights[k]);
                        actualNeuron.PrevWeights[k] = tmp;
                    }
                }
            }
        }

        private double CalculateNetworkError(IEnumerable<TeachingVector> testVectors)
        {
            errorCalculator.Reset();
            foreach (var vector in testVectors)
            {
                var networkOutput = CalculateResponse(vector.Inputs);
                errorCalculator.UpdateError(networkOutput, vector.Outputs);
            }
            return errorCalculator.CalculateRms();
        }
    }

    
    public class SigmoidalNeuralNetwork : NeuralNetwork
    {
        //Współczynnik stromości krzywej
        private readonly double beta;
        public SigmoidFunction NeuronActivateFunction; 

        public SigmoidalNeuralNetwork(int layersCount, int inputs, int outputs, double eta = 0.2, double alfa = 0.6, double beta = 1) : base(layersCount, inputs, outputs, eta, alfa)
        {
            this.beta = beta;
            NeuronActivateFunction = NeuronFunctions.Sigmoid;
        }

        protected override double NeuronFunction(double input)
        {
            return NeuronActivateFunction(beta *input);
        }
    }
}
