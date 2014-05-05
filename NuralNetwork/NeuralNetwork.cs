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

    public class NeuralNetwork
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
        private int[] layersCardinality;
        private double beta;
        private int outputLayer;
        private int inputLayer;
        private double eta;
        private double alfa;

        public NeuralNetwork(int layersCount, int inputs, int outputs)
        {
            this.layersCount = layersCount;
            this.inputs = inputs;
            this.outputs = outputs;
            CreateLayersWithNeurons();
            CreateWeightsTableAndRandomize();
            outputLayer = this.layersCount - 1;
            inputLayer = 0;
        }

        private void CreateLayersWithNeurons()
        {
            layers = new Layer[layersCount];
            layers[0].Neurons = new Neuron[inputs];
            layers[outputLayer].Neurons = new Neuron[outputs];
            int cardinality = (int) Math.Ceiling( Math.Sqrt((inputs*outputs)));
            for (int i = 1; i < outputLayer; i++)
            {
                layers[i].Neurons = new Neuron[cardinality];
            }
        }

        private void CreateWeightsTableAndRandomize()
        {
            var rand = new Random();

            InitializeInputLayerWeights();
            
            for (int i = 1; i < layersCount; i++)
            {
                for (int j = 0; j < layers[i].Neurons.Length; j++)
                {
                    var previousLayerCardinality = layers[i - 1].Neurons.Length;
                    layers[i].Neurons[j].Weights = new double[previousLayerCardinality];
                    for (int k = 0; k < layersCardinality[i - 1]; k++)
                    {
                        var randNumber = (((rand.Next() % 1000000L) / 1700.0) - 9.8) * 0.0015;
                        if(randNumber == 0)
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
                layers[0].Neurons[j].Weights = new double[1];
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
                        layers[i].Neurons[j].Output = 1.0/(1.0 + Math.Exp(beta*(-layers[i].Neurons[j].Input)));
                    }
                }
            }
        }

        public double TeachNetworkWithVector(TeachingVector vector)
        {
            var response = CalculateResponse(vector.Inputs);
            CalculateResponseErrorOnOutputs(vector.Outputs, response);
            for (int i = outputs - 2; i > 0; i--)
            {
                for (int j = 0; j < layers[i].Neurons.Length; j++)
                {
                    layers[i].Neurons[j].Error = 0.0;
                    Neuron actualNeuron = layers[i].Neurons[j];
                    for (int k = 0; k < layers[i + 1].Neurons.Length; k++)
                    {
                        Neuron outerNeuron = layers[i + 1].Neurons[k];
                        actualNeuron.Error += actualNeuron.Output*(1.0 - actualNeuron.Output)* outerNeuron.Error * outerNeuron.Weights[j];
                    }
                }
            }
            ApplayNewWeights();
            return CalulateNetworkError();
        }

        private double CalulateNetworkError()
        {
            double RMS = 0;
            for (int j = 0; j < n[LW - 1]; j++)
                RMS += (Wy[wu][j] - O[LW - 1][j]) * (Wy[wu][j] - O[LW - 1][j]);
            var ERMS = Math.Sqrt(RMS/(double) (ile_wek*n[LW - 1]));
            return ERMS;
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

        private void CalculateResponseErrorOnOutputs(double[] expected, double[] response)
        {
            for (int i = 0; i < response.Length; i++)
            {
                layers[outputLayer].Neurons[i].Error = expected[i] - response[i];
            }
        }


    }
}
