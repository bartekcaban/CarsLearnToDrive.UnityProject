using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    public static Manager Singleton = null; // The current EvolutionManager Instance

    [SerializeField] int CarCount = 100; // The number of cars per generation
    [SerializeField] GameObject CarPrefab; // The Prefab of the car to be created for each instance
    public Text geneartionText;
    public Text bestScoreText;

    private int numberOfGenerations = 0; // The current generation number

    List<Car> Cars = new List<Car>(); // This list of cars currently alive

    NeuralNetwork BestNeuralNetwork = null; // The best NeuralNetwork currently available
    private int bestScore = -1; 

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
        numberOfGenerations++; // Increment the generation count
        geneartionText.text = $"Generation: {numberOfGenerations}";

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
    public void CarDead(Car car)
    {
        if (car.Score > bestScore) // If it is better that the current best car
        {
            BestNeuralNetwork = car.TheNetwork; // Make sure it becomes the best car
           
            SetBestScore(car.Score);
        }

        Cars.Remove(car); // Remove the car from the list
        Destroy(car.gameObject); // Destroy the dead car

        if (!Cars.Any())
        {
            StartGeneration();
        }   
    }
    private void SetBestScore(int newBestScore)
    {
        bestScore = newBestScore;
        bestScoreText.text = $"Najlepszy wynik: {bestScore}";
    }
}
