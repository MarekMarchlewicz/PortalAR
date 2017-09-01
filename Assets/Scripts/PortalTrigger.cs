using UnityEngine;
using System;

[RequireComponent(typeof(Collider))]
public class PortalTrigger : MonoBehaviour
{
    public Action<ITeleportable> OnEntered, OnExited;

    private void OnTriggerEnter(Collider collider)
    {
        ITeleportable teleportable = collider.GetComponent<ITeleportable>();

        if(teleportable != null && OnEntered != null)
        {
            OnEntered(teleportable);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        ITeleportable teleportable = collider.GetComponent<ITeleportable>();

        if (teleportable != null && OnExited != null)
        {
            OnExited(teleportable);
        }
    }
}
