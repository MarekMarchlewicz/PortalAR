using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody), typeof(DragBehaviour))]
public class Ball : MonoBehaviour, ITeleportable, ISelectable
{
    [SerializeField]
    private MeshRenderer selctableRender;

    private Rigidbody m_Rigidbody;
    private Transform m_Transform;

    private Material selectionMaterial;
    private int selectionColorId = Shader.PropertyToID("_OutlineColor");

    private bool isSelected = false;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Transform = GetComponent<Transform>();

        selectionMaterial = selctableRender.material;

        DragBehaviour dragBehaviour = GetComponent<DragBehaviour>();
        dragBehaviour.OnDragStart += OnDragStart;
        dragBehaviour.OnDragStop += OnDragStop;
    }

    private void OnDragStart(DragBehaviour dragBehaviour)
    {
        StopAllCoroutines();
        StartCoroutine(ChangeOutlineAlpha(1f, 0.5f));
    }

    private void OnDragStop(DragBehaviour dragBehaviour)
    {
        StopAllCoroutines();
        StartCoroutine(ChangeOutlineAlpha(0f, 0.5f));
    }

    private IEnumerator ChangeOutlineAlpha(float newAlpha, float time)
    {
        Color currentColor = selectionMaterial.GetColor(selectionColorId);
        float currentAlpha = currentColor.a;

        float startTime = Time.time;

        while(Mathf.Abs(currentAlpha - newAlpha) < 0.01f)
        {
            currentAlpha = Mathf.Lerp(currentAlpha, newAlpha, (Time.time - startTime) / time);
            currentColor.a = currentAlpha;
            selectionMaterial.SetColor(selectionColorId, currentColor);

            yield return new WaitForEndOfFrame();
        }

        currentColor.a = newAlpha;
        selectionMaterial.SetColor(selectionColorId, currentColor);
    }

    public void MoveTo(Transform target)
    {
        m_Rigidbody.position = target.position;

        m_Rigidbody.velocity = target.forward * m_Rigidbody.velocity.magnitude;
    }

    public void UpdateScale(Vector3 newScale)
    {
        m_Transform.localScale = newScale;
    }

    public float GetDistance(Vector3 position)
    {
        return Vector3.Distance(position, m_Transform.position);
    }

    public void Select(bool selected)
    {
        isSelected = selected;

        Color newColor = selectionMaterial.GetColor(selectionColorId);
        newColor.a = isSelected ? 1f : 0f;

        selectionMaterial.SetColor(selectionColorId, newColor);
    }
}
