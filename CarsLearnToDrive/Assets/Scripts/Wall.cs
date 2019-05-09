using UnityEngine;

public class Wall : MonoBehaviour
{
    [SerializeField] string _layerHitName = "CarCollider";

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 8) //LayerMask.NameToLayer(_layerHitName))
        {
            collision.transform.GetComponent<Car>().WallHit();
        }
    }
}
