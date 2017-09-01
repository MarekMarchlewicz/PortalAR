using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    [SerializeField]
    private Transform cameraTransform;

    private Transform m_Transform;

    private void Awake()
    {
        m_Transform = GetComponent<Transform>();
    }

	private void Update()
    {
        if(cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }

        Vector3 newOrientation = Quaternion.LookRotation(cameraTransform.position - m_Transform.position).eulerAngles;
        newOrientation.x = 0f;
        newOrientation.z = 0f;

        m_Transform.rotation = Quaternion.Euler(newOrientation);
    }
}
