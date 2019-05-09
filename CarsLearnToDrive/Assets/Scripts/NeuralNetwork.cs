using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class NeuralNetwork
{
    public uint[] Topology // Returns the topology in the form of an array
    {
        get
        {
            uint[] Result = new uint[_theTopology.Count];
            _theTopology.CopyTo(Result, 0);
            return Result;
        }
    }

    private ReadOnlyCollection<uint> _theTopology; // Contains the topology of the NeuralNetwork
    private NeuralLayer[] _layers; // Contains the all the layers of the NeuralNetwork
    private Random _theRandomizer; // It is the Random instance used to mutate the NeuralNetwork

    public NeuralNetwork(uint[] Topology, int? Seed = 0)
    {
        // Validation Checks
        if (Topology.Length < 2)
            throw new ArgumentException("A Neural Network cannot contain less than 2 Layers.",
                                        "Topology");

        for (int i = 0; i < Topology.Length; i++)
        {
            if (Topology[i] < 1)
                throw new ArgumentException("A single layer of neurons must contain at least one neuron.", "Topology");
        }

        // Initialize Randomizer
        if (Seed.HasValue)
            _theRandomizer = new Random(Seed.Value);
        else
            _theRandomizer = new Random();

        // Set Topology
        _theTopology = new List<uint>(Topology).AsReadOnly();

        // Initialize Layers
        _layers = new NeuralLayer[_theTopology.Count - 1];

        // Set the Layers
        for (int i = 0; i < _layers.Length; i++)
        {
            _layers[i] = new NeuralLayer(_theTopology[i], _theRandomizer);
        }
    }

    public NeuralNetwork(NeuralNetwork Main)
    {
        // Initialize Randomizer
        _theRandomizer = new Random(Main._theRandomizer.Next());

        // Set Topology
        _theTopology = Main._theTopology;

        // Initialize Layers
        _layers = new NeuralLayer[_theTopology.Count - 1];

        // Set the Layers
        for (int i = 0; i < _layers.Length; i++)
        {
            _layers[i] = new NeuralLayer(Main._layers[i]);
        }
    }

    public double[] FeedForward(double[] Input)
    {
        if (Input == null)
            throw new ArgumentException("The input array cannot be set to null.", "Input");
        else if (Input.Length != _theTopology[0])
            throw new ArgumentException
            ("The input array's length does not match the number of neurons in the input layer.",
             "Input");

        double[] Output = Input;

        for (int i = 0; i < _layers.Length; i++)
        {
            Output = _layers[i].PropagateForward(Output);
        }

        return Output;
    }

    public void Mutate(double MutationProbablity = 0.3, double MutationAmount = 2.0)
    {
        for (int i = 0; i < _layers.Length; i++)
        {
            _layers[i].Mutate(MutationProbablity, MutationAmount);
        }
    }
}

