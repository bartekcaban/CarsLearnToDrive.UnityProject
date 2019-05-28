using UnityEngine;

public class Camera : MonoBehaviour
{
    private Vector3 _smoothPosVelocity;
    private Vector3 _smoothRotVelocity;

    void FixedUpdate()
    {
        Car BestCar = transform.GetChild(0).GetComponent<Car>();

        for (int i = 1; i < transform.childCount; i++)
        {
            Car CurrentCar = transform.GetChild(i).GetComponent<Car>();

            if (CurrentCar.Score > BestCar.Score)
            {
                BestCar = CurrentCar;
            }
        }

        Transform BestCarCamPos = BestCar.transform.GetChild(0);

        //transform.position = Vector3.SmoothDamp(transform.position, BestCarCamPos.position, ref _smoothPosVelocity, 0.7f);

        //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(BestCar.transform.position - transform.position),  0.1f);
    }
}
