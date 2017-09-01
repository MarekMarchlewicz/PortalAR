using UnityEngine;

public interface ITeleportable
{
    void MoveTo(Transform transform);

    void UpdateScale(float newScale);

    float GetDistance(Vector3 position);
}
