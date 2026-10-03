using UnityEngine;

public class IndicatorAnim : MonoBehaviour
{
    public float bobSpeed = 5f;
    public float bobHeight = 0.1f;
    
    private Vector3 startLocalPos;
    private Transform mainCam;

    private void Start()
    {
        startLocalPos = transform.localPosition;
        if (Camera.main != null) mainCam = Camera.main.transform;
    }

    private void Update()
    {
        float newY = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.localPosition = new Vector3(startLocalPos.x, startLocalPos.y + newY, startLocalPos.z);

        if (mainCam != null)
        {
            transform.LookAt(transform.position + mainCam.forward);
        }
    }
}