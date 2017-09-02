using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class DragBehaviour : MonoBehaviour
{
    public event System.Action<DragBehaviour> OnDragStart, OnDragUpdate, OnDragStop;

    [SerializeField]
    private LayerMask raycastMask = -1;

    [SerializeField]
    private float maxRayDistance = 300f;

    [SerializeField]
    private float speed = 5f;
    
    private Transform m_Transform;
    private Rigidbody m_Rigidbody;
    private Collider m_Collider;

    private int touchId = -1;
    private Camera dragCamera;
    private const float defaultCameraDepth = 4f;
    
    private Vector3? lastPosition;
    private const int VELOCITIES_BUFFER = 7;
    private Queue<Vector3> dragVelocities = new Queue<Vector3>(VELOCITIES_BUFFER);
    private RaycastHit[] rayHitBuffer;

    public bool IsDragged { get { return touchId >=0; } }
    
    private void Awake()
    {
        m_Transform = GetComponent<Transform>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Collider = GetComponent<Collider>();

        rayHitBuffer = new RaycastHit[10];
    }

    public void StartDragging(int newTouchId, Camera newDragCamera)
    {
        #if UNITY_STANDALONE || UNITY_EDITOR
        touchId = 1;
        #else
        touchId = newTouchId;
        #endif

        dragCamera = newDragCamera;
                
        if(m_Rigidbody != null)
        {
            m_Rigidbody.isKinematic = true;
        }

        UpdateDragging(true);

        if (OnDragStart != null)
        {
            OnDragStart(this);
        }
    }

    public void StopDragging()
    {
        if (m_Rigidbody != null && dragVelocities.Count > VELOCITIES_BUFFER / 2)
        {
            Vector3 releaseVelocity = Vector3.zero;
            foreach(Vector3 dragVelocity in dragVelocities)
            {
                releaseVelocity += dragVelocity;
            }
            releaseVelocity /= dragVelocities.Count;

            m_Rigidbody.isKinematic = false;
            m_Rigidbody.velocity = releaseVelocity;

            dragVelocities.Clear();
        }

        if (OnDragStop != null)
        {
            OnDragStop(this);
        }

        touchId = -1;
    }

    private void UpdateDragging(bool immediately = false)
    {
        Vector2? touch = GetTouchPosition();

        if (touch.HasValue)
        {
            int hits = Physics.RaycastNonAlloc(dragCamera.ScreenPointToRay(touch.Value), rayHitBuffer, maxRayDistance, raycastMask);

            float lerp = immediately ? 1 : Time.deltaTime * speed;

            Vector3? newPosition = null;

            if (hits > 0)
            {
                newPosition = rayHitBuffer[0].point;
            }

            if (!newPosition.HasValue)
            {
                newPosition = dragCamera.ScreenToWorldPoint(new Vector3(touch.Value.x, touch.Value.y, defaultCameraDepth));
            }

            MoveTo(newPosition.Value, lerp);

            if(OnDragUpdate != null)
            {
                OnDragUpdate(this);
            }
        }
        else
        {
            StopDragging();
        }
    }

    private void Update()
    {
        if (IsDragged)
        {
            UpdateDragging();
        }
        else
        {
            Ray ray;
            RaycastHit hit;
            #if UNITY_STANDALONE || UNITY_EDITOR
            if (Input.GetMouseButton(0))
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if(m_Collider.Raycast(ray, out hit, 1000f))
                {
                    StartDragging(0, Camera.main);

                    return;
                }
            }
            #else
            for(int i = 0; i < Input.touchCount; i++)
            {
                ray = Camera.main.ScreenPointToRay(Input.touches[i].position);

                if (m_Collider.Raycast(ray, out hit, 1000f))
                {
                    StartDragging(0, Camera.main);

                    return;
                }
            }
            #endif
        }
    }

    private Vector2? GetTouchPosition()
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            return Input.mousePosition;
        }
#else
        for (int t = 0; t < Input.touchCount; t++)
        {
            if (Input.touches[t].fingerId == touchId.Value)
            {
                return Input.touches[t].position;
            }
        }
#endif

        return null;
    }

    private void MoveTo(Vector3 position, float lerp)
    {
        m_Transform.position = Vector3.Lerp(m_Transform.position, position, lerp);

        if(lastPosition.HasValue)
        {
            if (dragVelocities.Count > VELOCITIES_BUFFER)
                dragVelocities.Dequeue();

            dragVelocities.Enqueue((m_Transform.position - lastPosition.Value) / Time.deltaTime);
        }

        lastPosition = m_Transform.position;
    }
}
