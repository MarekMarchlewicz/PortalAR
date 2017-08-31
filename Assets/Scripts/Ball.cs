using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Ball : MonoBehaviour, ITeleportable
{
    private Rigidbody m_Rigidbody;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    public void MoveTo(Transform target)
    {
        Debug.Log("MoveTo");
        m_Rigidbody.position = target.position;

        m_Rigidbody.velocity = target.forward * m_Rigidbody.velocity.magnitude;
    }
}
