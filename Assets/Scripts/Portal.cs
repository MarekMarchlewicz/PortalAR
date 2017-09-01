using UnityEngine;
using System.Collections.Generic;

public class Portal : MonoBehaviour
{
    [SerializeField]
    private List<Portal> connectedPortals;

    [SerializeField]
    private Color effectColor = Color.white;

    [SerializeField]
    private PortalTrigger triggerArea;

    [SerializeField]
    private Transform releaseTransform;

    [SerializeField]
    private float shrinkingDistance;

    [SerializeField]
    private Color shrinkingDistanceDebugColor = Color.blue;

    [SerializeField]
    private float teleportDistance;

    [SerializeField]
    private Color teleportDistanceDebugColor = Color.red;

    private List<ITeleportable> teleportableObjectsNearby = new List<ITeleportable>();

    private Transform m_Transform;

    public void Teleport(ITeleportable teleportable)
    {        
        teleportable.MoveTo(releaseTransform);
    }

    private void Awake()
    {
        m_Transform = GetComponent<Transform>();

        triggerArea.OnEntered += OnTriggerEntered;
        triggerArea.OnExited += OnTriggerExited;

        foreach(ParticleSystem particle in GetComponentsInChildren<ParticleSystem>())
        {
            particle.startColor = effectColor;
        }
    }

    private void OnTriggerEntered(ITeleportable teleportable)
    {
        teleportableObjectsNearby.Add(teleportable);
    }

    private void OnTriggerExited(ITeleportable teleportable)
    {
        teleportableObjectsNearby.Remove(teleportable);
    }

    private void Update()
    {
        for(int i = 0; i < teleportableObjectsNearby.Count; i++)
        {
            ITeleportable teleportable = teleportableObjectsNearby[i];

            float distance = teleportable.GetDistance(m_Transform.position);

            if (connectedPortals.Count > 0 && distance < teleportDistance)
            {
                connectedPortals[Random.Range(0, connectedPortals.Count)].Teleport(teleportable);

                teleportableObjectsNearby.Remove(teleportable);
            }
            else
            {
                float scale = (distance - teleportDistance) / shrinkingDistance;

                teleportable.UpdateScale(Vector3.one * scale);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = shrinkingDistanceDebugColor;
        Gizmos.DrawWireSphere(transform.position, shrinkingDistance);
        Gizmos.color = teleportDistanceDebugColor;
        Gizmos.DrawWireSphere(transform.position, teleportDistance);
    }
}
