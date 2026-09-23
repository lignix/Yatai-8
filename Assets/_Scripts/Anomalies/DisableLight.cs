using UnityEngine;

public class DisableLight : MonoBehaviour
{
    public Light lightSource;

    private void OnEnable()
    {
        if (lightSource != null)
        {
            lightSource.enabled = false;
        }
    }
    private void OnDisable()
    {
        if (lightSource != null)
        {
            lightSource.enabled = true;
        }
    }
}
