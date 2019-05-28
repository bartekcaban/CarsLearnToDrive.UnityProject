using System;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private List<Guid> _increasedFitnessGuids = new List<Guid>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.GetComponent<Car>() != null)
        {
            Car carComponent = other.transform.parent.GetComponent<Car>();
            Guid carGuid = carComponent.Id;

            if (!_increasedFitnessGuids.Contains(carGuid))
            {
                _increasedFitnessGuids.Add(carGuid);
                carComponent.CheckpointHit();
            }
        }
    }
}
