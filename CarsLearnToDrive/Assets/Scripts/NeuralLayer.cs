using System;
using System.Linq;

public class NeuralLayer
{
    private Neuron[] _neurons;
    private Random _random; // Contains a reference to the Random instance of the NeuralNetwork

    private double[][] _weights;
    private double[] _biases;
    public NeuralLayer(uint ammountOfNeurons, Random random)
    {
        if (ammountOfNeurons <= 0)
            throw new ArgumentException("You cannot create a Neural Layer with no input neurons.", "InputCount");
        else if (random == null)
            throw new ArgumentException("The randomizer cannot be set to null.", "Randomizer");

        _random = random;
        _neurons = new Neuron[ammountOfNeurons];
        InitializeWeights();
        InitializeBiases();
    }
    public NeuralLayer(NeuralLayer neuralLayer)
    {
        if (neuralLayer == null)
        {
            throw new ArgumentNullException("Neural layer has not been provided");
        }

        CloneNeuralLayer(neuralLayer);
    }
    private void InitializeBiases()
    {
        _biases = new double[_neurons.Length];
        for (int i = 0; i < _biases.Length; i++)
        {
            _biases[i] = _random.NextDouble() - 0.5;
        }
    }
    private void InitializeWeights()
    {
        _weights = new double[_neurons.Length][];

        for (int i = 0; i < _weights.Length; i++)
        {
            _weights[i] = new double[_weights.Length];
            for (int j = 0; j < _weights.Length; j++)
            {
                _weights[i][j] = _random.NextDouble() - 0.5;
            }
        }
    }

    private void CloneNeuralLayer(NeuralLayer neuralLayer)
    {
        _random = neuralLayer._random;
        _weights = new double[neuralLayer._weights.Length][];
        _neurons = new Neuron[neuralLayer._neurons.Length];
        _biases = new double[neuralLayer._biases.Length];
        for (int i = 0; i < _weights.Length; i++)
        {
            _biases[i] = neuralLayer._biases[i];
            _weights[i] = new double[neuralLayer._weights[0].Length];
            for (int j = 0; j < _weights[i].Length; j++)
            {
                _weights[i][j] = neuralLayer._weights[i][j];
            }
        }
    }
    public double[] PropagateForward(double[] neurons)
    {
        for (int i = 0; i < _weights.Length; i++)
        {
            var neuronValue = 0.0;
            for (int j = 0; j < _weights[i].Length; j++)
            {
                neuronValue += _weights[i][j] * neurons[j];
            }
            neuronValue += _biases[i];
            var neuron = new Neuron(neuronValue, ReLU);

            _neurons[i] = neuron;
        }

        return _neurons.Select(n => n.Value).ToArray();
    }
    private double ReLU(double value)
    {
        return value < 0 ? 0 : value;
    }
    public void Mutate(double MutationProbablity, double MutationAmount)
    {
        for (int i = 0; i < _weights.Length; i++)
        {
            for (int j = 0; j < _weights[i].Length; j++)
            {
                if (_random.NextDouble() < MutationProbablity)
                    _weights[i][j] = _random.NextDouble() * (MutationAmount * 2) - MutationAmount;
            }
        }
    }
}