using UnityEngine;

public class InteractableIndicator : MonoBehaviour
{
    [Header("Settings")]
    public GameObject indicatorPrefab;
    public float interactDistance = 2.5f;
    public float lookAngleThreshold = 20f;

    public Vector3 indicatorOffset = new Vector3(0f, 0.2f, 0f);

    private GameObject indicatorInstance;
    private Transform mainCamera;

    private void Start()
    {
        if (Camera.main != null) mainCamera = Camera.main.transform;

        if (indicatorPrefab != null)
        {
            indicatorInstance = Instantiate(indicatorPrefab, transform);
            indicatorInstance.transform.localPosition = indicatorOffset;
            indicatorInstance.SetActive(false);
        }
    }

    private void Update()
    {
        if (indicatorInstance == null || mainCamera == null) return;

        if (PauseManager.Instance != null && PauseManager.Instance.isPaused)
        {
            indicatorInstance.SetActive(false);
            return;
        }

        float distance = Vector3.Distance(mainCamera.position, transform.position);

        if (distance <= interactDistance)
        {
            Vector3 dirToTarget = (transform.position - mainCamera.position).normalized;
            float angle = Vector3.Angle(mainCamera.forward, dirToTarget);

            if (angle <= lookAngleThreshold)
            {
                indicatorInstance.SetActive(true);
                return;
            }
        }

        indicatorInstance.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.92f, 0.016f, 0.5f);
        Vector3 gizmoPos = transform.TransformPoint(indicatorOffset);
        Gizmos.DrawSphere(gizmoPos, 0.1f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, gizmoPos);
    }
    private void OnDisable()
    {
        if (indicatorInstance != null)
        {
            indicatorInstance.SetActive(false);
        }
    }
}
