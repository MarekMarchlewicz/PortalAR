using UnityEngine;
using System.Collections.Generic;

public class Portal : MonoBehaviour
{
    [SerializeField]
    private List<Portal> connectedPortals;

    [SerializeField]
    private Color effectColor = Color.white;

    [SerializeField]
    private PortalTrigger innerTrigger, outerTrigger;

    [SerializeField]
    private Transform releaseTransform;

    private List<ITeleportable> teleportableObjectsNearby = new List<ITeleportable>();

    public void Teleport(ITeleportable teleportable)
    {
        Debug.Log("Teleport " + gameObject.name);

        teleportableObjectsNearby.Add(teleportable);
        teleportable.MoveTo(releaseTransform);
    }

    private void Awake()
    {
        innerTrigger.OnEntered += OnInnerTriggerEntered;
        outerTrigger.OnEntered += OnOuterTriggerEntered;
        outerTrigger.OnExited += OnOuterTriggerExited;

        foreach(ParticleSystem particle in GetComponentsInChildren<ParticleSystem>())
        {
            particle.startColor = effectColor;
        }
    }

    private void OnInnerTriggerEntered(ITeleportable teleportable)
    {
        if (connectedPortals.Count < 1)
            return;


        Debug.Log("OnInnerTriggerEntered " + gameObject.name);
        connectedPortals[Random.Range(0, connectedPortals.Count)].Teleport(teleportable);

        teleportableObjectsNearby.Remove(teleportable);
    }

    private void OnOuterTriggerEntered(ITeleportable teleportable)
    {
        if (teleportableObjectsNearby.Contains(teleportable))
            return;

        Debug.Log("OnOuterTriggerEntered " + gameObject.name);
        teleportableObjectsNearby.Add(teleportable);
    }

    private void OnOuterTriggerExited(ITeleportable teleportable)
    {
        Debug.Log("OnOuterTriggerExited " + gameObject.name);
        teleportableObjectsNearby.Remove(teleportable);
    }
}
