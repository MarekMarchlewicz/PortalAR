using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody), typeof(DragBehaviour))]
public class Ball : MonoBehaviour, ITeleportable, ISelectable
{
    [SerializeField]
    private MeshRenderer selectableRender;

    [SerializeField]
    private float outlineWidth = 0.035f;

    private Rigidbody m_Rigidbody;
    private Transform m_Transform;
    private DragBehaviour m_DragBehaviour;

    private Material selectionMaterial;
    private int selectionColorId = Shader.PropertyToID("_OutlineColor");
    private int selectionWidthId = Shader.PropertyToID("_OutlineWidth");

    private bool isSelected = false;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Transform = GetComponent<Transform>();

        m_DragBehaviour = GetComponent<DragBehaviour>();
        m_DragBehaviour.OnDragStart += OnDragStart;
        m_DragBehaviour.OnDragStop += OnDragStop;

        selectionMaterial = selectableRender.material;
    }

    private void OnDragStart(DragBehaviour dragBehaviour)
    {
        StopAllCoroutines();
        StartCoroutine(ChangeOutlineAlpha(1f, 1f));
    }

    private void OnDragStop(DragBehaviour dragBehaviour)
    {
        StopAllCoroutines();
        StartCoroutine(ChangeOutlineAlpha(0f, 1f));
    }

    private IEnumerator ChangeOutlineAlpha(float newAlpha, float time)
    {
        Color currentColor = selectionMaterial.GetColor(selectionColorId);
        float currentAlpha = currentColor.a;

        float startTime = Time.time;

        while(Mathf.Abs(currentAlpha - newAlpha) > 0.01f)
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
        if (m_DragBehaviour.IsDragged)
            return;

        m_Rigidbody.position = target.position;

        m_Rigidbody.velocity = target.forward * m_Rigidbody.velocity.magnitude;
    }

    public void UpdateScale(float newScale)
    {
        m_Transform.localScale = Vector3.one * newScale;

        selectionMaterial.SetFloat(selectionWidthId, outlineWidth * newScale);
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
