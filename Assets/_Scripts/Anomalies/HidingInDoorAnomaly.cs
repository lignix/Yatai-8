using System.Collections;
using UnityEngine;

public class HidingInDoorAnomaly : MonoBehaviour
{
    private bool hasTriggered = false;
    public GameObject student;
    
    [Tooltip("Vitesse de déplacement de l'étudiante")]
    public float moveSpeed = 2f; 

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            if (student != null)
            {
                StartCoroutine(SmoothMoveRoutine());
            }
            hasTriggered = true;
        }
    }

    private IEnumerator SmoothMoveRoutine()
    {
        Vector3 startPos = student.transform.position;
        Vector3 targetPos = startPos + new Vector3(0f, 0f, 1f);

        while (Vector3.Distance(student.transform.position, targetPos) > 0.01f)
        {
            student.transform.position = Vector3.MoveTowards(student.transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        student.transform.position = targetPos;
    }
}