using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SpawnPoint : NetworkBehaviour
{
    [ReadOnly]
    [SyncVar]
    public bool IsOccupied;

    private readonly IList<PlayerController> occupants = new List<PlayerController>();

    public void OccupantLeft(PlayerController occupant)
    {
        occupants.Remove(occupant);
        IsOccupied = occupants.Count > 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        var controller = other.GetComponentInParent<PlayerController>();
        if (controller != null)
        {
            occupants.Add(controller);
            IsOccupied = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var controller = other.GetComponentInParent<PlayerController>();
        if (controller != null)
        {
            occupants.Remove(controller);
            IsOccupied = occupants.Count > 0;
        }
    }
}
