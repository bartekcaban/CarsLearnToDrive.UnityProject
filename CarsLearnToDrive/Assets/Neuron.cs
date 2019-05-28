
public class Neuron
{
    public delegate double ActivationFunction(double value);
    public double Value { get; private set; }
    public Neuron(double value)
    {
        Value = value;
    }

    public Neuron(double value, ActivationFunction activationFunction)
    {
        Value = activationFunction(value);
    }
}

