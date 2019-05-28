using UnityEngine;

public class Wall : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Car>() != null)
        {
            collision.transform.GetComponent<Car>().WallHit();
        }
    }
}
