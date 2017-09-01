using UnityEngine;

public interface ITeleportable
{
    void MoveTo(Transform transform);

    void UpdateScale(Vector3 newScale);

    float GetDistance(Vector3 position);
}
