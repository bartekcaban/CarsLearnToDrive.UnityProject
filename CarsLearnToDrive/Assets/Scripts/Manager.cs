using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    public static Manager Singleton = null; // The current EvolutionManager Instance

    [SerializeField] int CarCount = 100; // The number of cars per generation
    [SerializeField] GameObject CarPrefab; // The Prefab of the car to be created for each instance
    [SerializeField] Text GenerationNumberText; // Some text to write the generation number

    int GenerationCount = 0; // The current generation number

    List<Car> Cars = new List<Car>(); // This list of cars currently alive

    NeuralNetwork BestNeuralNetwork = null; // The best NeuralNetwork currently available
    int BestFitness = -1; // The FItness of the best NeuralNetwork ever created

    // On Start
    private void Start()
    {
        if (Singleton == null) // If no other instances were created
            Singleton = this; // Make the only instance this one
        else
            gameObject.SetActive(false); // There is another instance already in place. Make this one inactive.

        BestNeuralNetwork = new NeuralNetwork(Car.NextNetwork); // Set the BestNeuralNetwork to a random new network

        StartGeneration();
    }

    // Sarts a whole new generation
    void StartGeneration()
    {
        GenerationCount++; // Increment the generation count
        //GenerationNumberText.text = "Generation: " + GenerationCount; // Update generation text
        Debug.Log(GenerationCount);
        Debug.Log(BestFitness);

        for (int i = 0; i < CarCount; i++)
        {
            if (i == 0)
                Car.NextNetwork = BestNeuralNetwork; // Make sure one car uses the best network
            else
            {
                Car.NextNetwork = new NeuralNetwork(BestNeuralNetwork); // Clone the best neural network and set it to be for the next car
                Car.NextNetwork.Mutate(); // Mutate it
            }

            Cars.Add(Instantiate(CarPrefab, transform.position, Quaternion.identity, transform).GetComponent<Car>()); // Instantiate a new car and add it to the list of cars
        }
    }

    // Gets called by cars when they die
    public void CarDead(Car DeadCar, int Fitness)
    {
        Cars.Remove(DeadCar); // Remove the car from the list
        Destroy(DeadCar.gameObject); // Destroy the dead car

        if (Fitness > BestFitness) // If it is better that the current best car
        {
            BestNeuralNetwork = DeadCar.TheNetwork; // Make sure it becomes the best car
            BestFitness = Fitness; // And also set the best fitness
        }

        if (Cars.Count <= 0) // If there are no cars left
            StartGeneration(); // Create a new generation
    }
}
