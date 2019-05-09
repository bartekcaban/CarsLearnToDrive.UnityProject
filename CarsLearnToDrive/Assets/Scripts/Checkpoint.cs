using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] string _layerHitName = "CarCollider";

    private List<string> _increasedFitnessGuids = new List<string>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)// LayerMask.NameToLayer(_layerHitName))
        {
            Car carComponent = other.transform.parent.GetComponent<Car>();
            string carGuid = carComponent.TheGuid;

            if (!_increasedFitnessGuids.Contains(carGuid))
            {
                _increasedFitnessGuids.Add(carGuid);
                carComponent.CheckpointHit();
            }
        }
    }
}
