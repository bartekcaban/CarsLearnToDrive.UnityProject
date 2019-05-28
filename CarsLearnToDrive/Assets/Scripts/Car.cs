using System;
using System.Collections;
using UnityEngine;

public class Car : MonoBehaviour
{
    [SerializeField] bool UseUserInput = false; 
    public LayerMask sensorMask;
    private float sameScoreDeathCount = 2f;

    public static NeuralNetwork NextNetwork = new NeuralNetwork(new uint[] { 6, 4, 3, 2 }, null); 

    public Guid Id { get; private set; } 

    public int Score { get; private set; } 

    public NeuralNetwork TheNetwork { get; private set; } 

    Rigidbody TheRigidbody; 
    LineRenderer TheLineRenderer;

    private void Awake()
    {
        Id = Guid.NewGuid();

        TheNetwork = NextNetwork;
        NextNetwork = new NeuralNetwork(NextNetwork.Topology, null); // Make sure the Next Network is reassigned to avoid having another car use the same network

        TheRigidbody = GetComponent<Rigidbody>(); // Assign Rigidbody
        TheLineRenderer = GetComponent<LineRenderer>(); // Assign LineRenderer

        StartCoroutine(IsNotImproving()); // Start checking if the score stayed the same for a lot of time

        TheLineRenderer.positionCount = 18; // Make sure the line is long enough
    }

    private void Update()
    {
        if (UseUserInput) // If we're gonna use user input
            Move(Input.GetAxisRaw("Vertical"), Input.GetAxisRaw("Horizontal")); // Moves the car according to the input
        else // if we're gonna use a neural network
        {
            float Vertical;
            float Horizontal;

            GetNeuralInputAxis(out Vertical, out Horizontal);

            Move(Vertical, Horizontal); // Moves the car
        }
    }

    // Casts all the rays, puts them through the NeuralNetwork and outputs the Move Axis
    void GetNeuralInputAxis(out float Vertical, out float Horizontal)
    {
        double[] NeuralInput = new double[NextNetwork.Topology[0]];

        // Cast forward, back, right and left
        NeuralInput[0] = CastRay(transform.forward, Vector3.forward, 1) / 4;
        NeuralInput[1] = CastRay(-transform.forward, -Vector3.forward, 3) / 4;
        NeuralInput[2] = CastRay(transform.right, Vector3.right, 5) / 4;
        NeuralInput[3] = CastRay(-transform.right, -Vector3.right, 7) / 4;

        // Cast forward-right and forward-left
        float SqrtHalf = Mathf.Sqrt(0.5f);
        NeuralInput[4] = CastRay(transform.right * SqrtHalf + transform.forward * SqrtHalf, Vector3.right * SqrtHalf + Vector3.forward * SqrtHalf, 9) / 4;
        NeuralInput[5] = CastRay(transform.right * SqrtHalf + -transform.forward * SqrtHalf, Vector3.right * SqrtHalf + -Vector3.forward * SqrtHalf, 13) / 4;
        //NeuralInput[6] = CastRay(-transform.right * SqrtHalf + transform.forward * SqrtHalf, -Vector3.right * SqrtHalf + Vector3.forward * SqrtHalf, 15) / 4;
        //NeuralInput[7] = CastRay(-transform.right * SqrtHalf + -transform.forward * SqrtHalf, -Vector3.right * SqrtHalf + -Vector3.forward * SqrtHalf, 17) / 4;

        // Feed through the network
        double[] NeuralOutput = TheNetwork.FeedForward(NeuralInput);

        // Get Vertical Value
        if (NeuralOutput[0] <= 0.25f)
            Vertical = -1;
        else if (NeuralOutput[0] >= 0.75f)
            Vertical = 1;
        else
            Vertical = 0;

        // Get Horizontal Value
        if (NeuralOutput[1] <= 0.25f)
            Horizontal = -1;
        else if (NeuralOutput[1] >= 0.75f)
            Horizontal = 1;
        else
            Horizontal = 0;

        // If the output is just standing still, then move the car forward
        if (Vertical == 0 && Horizontal == 0)
            Vertical = 1;
    }

    // Checks each few seconds if the car didn't make any improvement
    IEnumerator IsNotImproving()
    {
        while (true)
        {
            int OldFitness = Score; // Save the initial fitness
            yield return new WaitForSeconds(sameScoreDeathCount); // Wait for some time
            if (OldFitness == Score) // Check if the fitness didn't change yet
                WallHit(); // Kill this car
        }
    }

    // Casts a ray and makes it visible through the line renderer
    double CastRay(Vector3 RayDirection, Vector3 LineDirection, int LinePositionIndex)
    {
        float Length = 8; // Maximum length of each ray

        RaycastHit Hit;
        if (Physics.Raycast(transform.position, RayDirection, out Hit, Length, sensorMask)) // Cast a ray
        {
            float Dist = Vector3.Distance(Hit.point, transform.position); // Get the distance of the hit in the line
            TheLineRenderer.SetPosition(LinePositionIndex, Dist * LineDirection); // Set the position of the line

            return Dist; // Return the distance
        }
        else
        {
            TheLineRenderer.SetPosition(LinePositionIndex, LineDirection * Length); // Set the distance of the hit in the line to the maximum distance

            return Length; // Return the maximum distance
        }
    }

    // The main function that moves the car.
    public void Move(float v, float h)
    {
        TheRigidbody.velocity = transform.right * v * 4;
        TheRigidbody.angularVelocity = transform.up * h * 3;
    }

    // This function is called through all the checkpoints when the car hits any.
    public void CheckpointHit()
    {
        Score++; // Increase Fitness/Score
    }

    // Called by walls when hit by the car
    public void WallHit()
    {
        Manager.Singleton.CarDead(this); // Tell the Evolution Manager that the car is dead
        gameObject.SetActive(false); // Make sure the car is inactive
    }
}
