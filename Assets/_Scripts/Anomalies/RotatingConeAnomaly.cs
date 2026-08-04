using UnityEngine;

public class RotatingConeAnomaly : MonoBehaviour
{
    public float rotationSpeed = 90f;

    void FixedUpdate()
    {
        transform.Rotate(Vector3.up.normalized * rotationSpeed * Time.fixedDeltaTime, Space.Self);
    }
}
